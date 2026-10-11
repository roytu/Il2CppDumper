# @category _Unity

# -*- coding: utf-8 -*-
import json
import os

from java.math import BigInteger
from ghidra.app.util.cparser.C import CParserUtils
from ghidra.app.cmd.function import ApplyFunctionSignatureCmd
from ghidra.app.cmd.disassemble import DisassembleCommand
from ghidra.program.model.address import AddressSet
from ghidra.program.model.data import CategoryPath, DataTypeConflictHandler, EnumDataType
from ghidra.program.model.symbol.SourceType import USER_DEFINED
from ghidra.program.model.symbol import SymbolUtilities
from ghidra.util.exception import CancelledException

processFields = [
    "ScriptEnum",
    "ScriptMethod",
    "ScriptString",
    "ScriptMetadata",
    "ScriptMetadataMethod",
    "Addresses",
]

functionManager = currentProgram.getFunctionManager()
listing = currentProgram.getListing()
symbolTable = currentProgram.getSymbolTable()
baseAddress = currentProgram.getImageBase()
isArm32 = currentProgram.getLanguage().getProcessor().toString() == "ARM"
tmode = currentProgram.getProgramContext().getRegister("TMode") if isArm32 else None

def ask_file(title, index):
    # script arguments make the script runnable headless:
    #   analyzeHeadless ... -import libil2cpp.so -noanalysis -postScript ghidra_with_struct.py script.json il2cpp_ghidra.h
    args = getScriptArgs()
    if len(args) > index:
        return args[index]
    if isRunningHeadless():
        return None
    try:
        return askFile(title, "Open").getAbsolutePath()
    except CancelledException:
        return None

def get_addr(addr):
    if isArm32 and addr & 1:
        # Thumb method pointers carry the mode bit; the context is locked once code exists there
        address = baseAddress.add(addr - 1)
        if listing.getInstructionAt(address) is None:
            currentProgram.getProgramContext().setValue(tmode, address, address, BigInteger.ONE)
        return address
    return baseAddress.add(addr)

def set_name(addr, name, namespace=None):
    name = name.replace(' ', '-')
    createLabel(addr, name, namespace, True, USER_DEFINED)

namespaces = {}

def get_namespace(path):
    # "image/namespace/outer type/type" -> that chain of Ghidra namespaces, created on first use
    namespace = namespaces.get(path)
    if namespace is None:
        parent, _, name = path.rpartition("/")
        parentNamespace = get_namespace(parent) if parent else currentProgram.getGlobalNamespace()
        namespace = symbolTable.getOrCreateNameSpace(parentNamespace, SymbolUtilities.replaceInvalidChars(name, True), USER_DEFINED)
        namespaces[path] = namespace
    return namespace

def set_type(addr, type):
    # Requires types (il2cpp.h) to be imported first
    newType = type.replace("*"," *").replace("  "," ").strip()
    dataTypes = getDataTypes(newType)
    addrType = None
    if len(dataTypes) == 0:
        if newType == newType[:-2] + " *":
            baseType = newType[:-2]
            dataTypes = getDataTypes(baseType)
            if len(dataTypes) == 1:
                dtm = currentProgram.getDataTypeManager()
                pointerType = dtm.getPointer(dataTypes[0])
                addrType = dtm.addDataType(pointerType, None)
    elif len(dataTypes) > 1:
        print("Conflicting data types found for type " + type + "(parsed as '" + newType + "')")
        return
    else:
        addrType = dataTypes[0]
    if addrType is None:
        print("Could not identify type " + type + "(parsed as '" + newType + "')")
    else:
        try:
            createData(addr, addrType)
        except:
            print("Warning: unable to set type (CodeUnitInsertionException)")


def make_function(start):
    func = getFunctionAt(start)
    if func is None:
        try:
            createFunction(start, None)
        except:
            print("Warning: Unable to create function")

def set_sig(addr, name, sig):
    try:
        typeSig = CParserUtils.parseSignature(None, currentProgram, sig, False)
    except:
        print("Warning: Unable to parse")
        print(sig)
        print("Attempting to modify...")
        # try to fix by renaming the parameters
        try:
            newSig = sig.replace(", ","ext, ").replace(")","ext)")
            typeSig = CParserUtils.parseSignature(None, currentProgram, newSig, False)
        except:
            print("Warning: also unable to parse")
            print(newSig)
            print("Skipping.")
            return
    if typeSig is not None:
        try:
            typeSig.setName(name.replace(' ', '-'))
            ApplyFunctionSignatureCmd(addr, typeSig, USER_DEFINED, False, True).applyTo(currentProgram)
        except:
            print("Warning: unable to set Signature. ApplyFunctionSignatureCmd() Failed.")

f = ask_file("script.json from Il2cppdumper", 0)
if f is None:
    raise CancelledException()
data = json.loads(open(f, 'rb').read().decode('utf-8'))
methodAddresses = []

header = ask_file("il2cpp_ghidra.h from Il2CppDumper (cancel to keep the current types)", 1)
if header is not None:
    if "ScriptEnum" in data and "ScriptEnum" in processFields:
        # the header leaves the enums undeclared because the C parser cannot size one; created first, in the
        # header's category, the parser resolves their names to enums of the right width
        monitor.setMessage("Enums")
        enumCategory = CategoryPath("/" + os.path.basename(header))
        for scriptEnum in data["ScriptEnum"]:
            enum = EnumDataType(enumCategory, scriptEnum["Name"], scriptEnum["Size"])
            for member, value in scriptEnum["Members"].items():
                enum.add(member, value)
            currentProgram.getDataTypeManager().addDataType(enum, DataTypeConflictHandler.REPLACE_HANDLER)
    monitor.setMessage("Parsing " + header)
    results = CParserUtils.parseHeaderFiles(None, [header], None, [], currentProgram.getDataTypeManager(), monitor)
    if not results.successful():
        print(results.getFormattedParseMessage(None))
        raise RuntimeError("il2cpp_ghidra.h did not parse, see the messages above")

if "ScriptMethod" in data and "ScriptMethod" in processFields:
    scriptMethods = data["ScriptMethod"]
    monitor.initialize(len(scriptMethods))
    monitor.setMessage("Methods")
    for scriptMethod in scriptMethods:
        addr = get_addr(scriptMethod["Address"])
        name = scriptMethod["Name"]
        namespace = None
        if "Namespace" in scriptMethod:
            # the method goes into image/namespace/type namespaces under its own name
            namespace = get_namespace(scriptMethod["Namespace"])
            name = name[name.rfind("$$") + 2:]
        set_name(addr, name, namespace)
        methodAddresses.append(addr)
        monitor.incrementProgress(1)

if "ScriptString" in data and "ScriptString" in processFields:
    index = 1
    scriptStrings = data["ScriptString"]
    monitor.initialize(len(scriptStrings))
    monitor.setMessage("Strings")
    for scriptString in scriptStrings:
        addr = get_addr(scriptString["Address"])
        value = scriptString["Value"]
        value_sanitized = SymbolUtilities.replaceInvalidChars(value, True)
        name = f"StringLiteral_{str(index)}_{value_sanitized[:64]}"
        createLabel(addr, name, True, USER_DEFINED)
        setEOLComment(addr, value)
        index += 1
        monitor.incrementProgress(1)

if "ScriptMetadata" in data and "ScriptMetadata" in processFields:
    scriptMetadatas = data["ScriptMetadata"]
    monitor.initialize(len(scriptMetadatas))
    monitor.setMessage("Metadata")
    for scriptMetadata in scriptMetadatas:
        addr = get_addr(scriptMetadata["Address"])
        name = scriptMetadata["Name"]
        set_name(addr, name)
        setEOLComment(addr, name)
        monitor.incrementProgress(1)
        if scriptMetadata["Signature"]:
            set_type(addr, scriptMetadata["Signature"])

if "ScriptMetadataMethod" in data and "ScriptMetadataMethod" in processFields:
    scriptMetadataMethods = data["ScriptMetadataMethod"]
    monitor.initialize(len(scriptMetadataMethods))
    monitor.setMessage("Metadata Methods")
    for scriptMetadataMethod in scriptMetadataMethods:
        addr = get_addr(scriptMetadataMethod["Address"])
        name = scriptMetadataMethod["Name"]
        methodAddr = get_addr(scriptMetadataMethod["MethodAddress"])
        set_name(addr, name)
        setEOLComment(addr, name)
        monitor.incrementProgress(1)

if "Addresses" in data and "Addresses" in processFields:
    addresses = data["Addresses"]
    monitor.initialize(len(addresses))
    monitor.setMessage("Addresses")
    for index in range(len(addresses) - 1):
        start = get_addr(addresses[index])
        make_function(start)
        monitor.incrementProgress(1)

if "ScriptMethod" in data and "ScriptMethod" in processFields:
    scriptMethods = data["ScriptMethod"]
    for scriptMethod in scriptMethods:
        addr = get_addr(scriptMethod["Address"])
        sig = scriptMethod["Signature"][:-1]
        name = scriptMethod["Name"]
        if "Namespace" in scriptMethod:
            name = name[name.rfind("$$") + 2:]
        set_sig(addr, name, sig)

# Disassemble all methods
for methodAddress in methodAddresses:
    # start, restrictedSet, followFlow
    cmd = DisassembleCommand(
            methodAddress,              # address
            None,                       # restrictedSet
            True                        # followFlow
    )
    cmd.enableCodeAnalysis(False)
    cmd.applyTo(currentProgram, monitor)

print('Script finished!')
