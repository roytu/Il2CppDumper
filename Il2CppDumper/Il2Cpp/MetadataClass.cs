using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Il2CppDumper
{

    public class Il2CppSectionMetadata
    {
        public uint offset;
        public uint size;
        public uint count;
    }

    [StructLayout(LayoutKind.Auto, Pack = 4)]
    public class Il2CppGlobalMetadataHeader
    {
        public uint sanity;
        public int version;

        [Version(Max = 35)]
        public uint stringLiteralOffset; // string data for managed code
        [Version(Max = 35)]
        public int stringLiteralSize;
        [Version(Max = 35)]
        public uint stringLiteralDataOffset;
        [Version(Max = 35)]
        public int stringLiteralDataSize;
        [Version(Max = 35)]
        public uint stringOffset; // string data for metadata
        [Version(Max = 35)]
        public int stringSize;
        [Version(Max = 35)]
        public uint eventsOffset; // Il2CppEventDefinition
        [Version(Max = 35)]
        public int eventsSize;
        [Version(Max = 35)]
        public uint propertiesOffset; // Il2CppPropertyDefinition
        [Version(Max = 35)]
        public int propertiesSize;
        [Version(Max = 35)]
        public uint methodsOffset; // Il2CppMethodDefinition
        [Version(Max = 35)]
        public int methodsSize;
        [Version(Max = 35)]
        public uint parameterDefaultValuesOffset; // Il2CppParameterDefaultValue
        [Version(Max = 35)]
        public int parameterDefaultValuesSize;
        [Version(Max = 35)]
        public uint fieldDefaultValuesOffset; // Il2CppFieldDefaultValue
        [Version(Max = 35)]
        public int fieldDefaultValuesSize;
        [Version(Max = 35)]
        public uint fieldAndParameterDefaultValueDataOffset; // uint8_t
        [Version(Max = 35)]
        public int fieldAndParameterDefaultValueDataSize;
        [Version(Max = 35)]
        public int fieldMarshaledSizesOffset; // Il2CppFieldMarshaledSize
        [Version(Max = 35)]
        public int fieldMarshaledSizesSize;
        [Version(Max = 35)]
        public uint parametersOffset; // Il2CppParameterDefinition
        [Version(Max = 35)]
        public int parametersSize;
        [Version(Max = 35)]
        public uint fieldsOffset; // Il2CppFieldDefinition
        [Version(Max = 35)]
        public int fieldsSize;
        [Version(Max = 35)]
        public uint genericParametersOffset; // Il2CppGenericParameter
        [Version(Max = 35)]
        public int genericParametersSize;
        [Version(Max = 35)]
        public uint genericParameterConstraintsOffset; // TypeIndex
        [Version(Max = 35)]
        public int genericParameterConstraintsSize;
        [Version(Max = 35)]
        public uint genericContainersOffset; // Il2CppGenericContainer
        [Version(Max = 35)]
        public int genericContainersSize;
        [Version(Max = 35)]
        public uint nestedTypesOffset; // TypeDefinitionIndex
        [Version(Max = 35)]
        public int nestedTypesSize;
        [Version(Max = 35)]
        public uint interfacesOffset; // TypeIndex
        [Version(Max = 35)]
        public int interfacesSize;
        [Version(Max = 35)]
        public uint vtableMethodsOffset; // EncodedMethodIndex
        [Version(Max = 35)]
        public int vtableMethodsSize;
        [Version(Max = 35)]
        public int interfaceOffsetsOffset; // Il2CppInterfaceOffsetPair
        [Version(Max = 35)]
        public int interfaceOffsetsSize;
        [Version(Max = 35)]
        public uint typeDefinitionsOffset; // Il2CppTypeDefinition
        [Version(Max = 35)]
        public int typeDefinitionsSize;
        [Version(Max = 24.1)]
        public uint rgctxEntriesOffset; // Il2CppRGCTXDefinition
        [Version(Max = 24.1)]
        public int rgctxEntriesCount;
        [Version(Max = 35)]
        public uint imagesOffset; // Il2CppImageDefinition
        [Version(Max = 35)]
        public int imagesSize;
        [Version(Max = 35)]
        public uint assembliesOffset; // Il2CppAssemblyDefinition
        [Version(Max = 35)]
        public int assembliesSize;
        [Version(Min = 19, Max = 24.5)]
        public uint metadataUsageListsOffset; // Il2CppMetadataUsageList
        [Version(Min = 19, Max = 24.5)]
        public int metadataUsageListsCount;
        [Version(Min = 19, Max = 24.5)]
        public uint metadataUsagePairsOffset; // Il2CppMetadataUsagePair
        [Version(Min = 19, Max = 24.5)]
        public int metadataUsagePairsCount;
        [Version(Min = 19, Max = 35)]
        public uint fieldRefsOffset; // Il2CppFieldRef
        [Version(Min = 19, Max = 35)]
        public int fieldRefsSize;
        [Version(Min = 20, Max = 35)]
        public int referencedAssembliesOffset; // int32_t
        [Version(Min = 20, Max = 35)]
        public int referencedAssembliesSize;
        [Version(Min = 21, Max = 27.2)]
        public uint attributesInfoOffset; // Il2CppCustomAttributeTypeRange
        [Version(Min = 21, Max = 27.2)]
        public int attributesInfoCount;
        [Version(Min = 21, Max = 27.2)]
        public uint attributeTypesOffset; // TypeIndex
        [Version(Min = 21, Max = 27.2)]
        public int attributeTypesCount;
        [Version(Min = 29, Max = 35)]
        public uint attributeDataOffset;
        [Version(Min = 29, Max = 35)]
        public int attributeDataSize;
        [Version(Min = 29, Max = 35)]
        public uint attributeDataRangeOffset;
        [Version(Min = 29, Max = 35)]
        public int attributeDataRangeSize;
        [Version(Min = 22, Max = 35)]
        public int unresolvedVirtualCallParameterTypesOffset; // TypeIndex
        [Version(Min = 22, Max = 35)]
        public int unresolvedVirtualCallParameterTypesSize;
        [Version(Min = 22, Max = 35)]
        public int unresolvedVirtualCallParameterRangesOffset; // Il2CppRange
        [Version(Min = 22, Max = 35)]
        public int unresolvedVirtualCallParameterRangesSize;
        [Version(Min = 23, Max = 35)]
        public int windowsRuntimeTypeNamesOffset; // Il2CppWindowsRuntimeTypeNamePair
        [Version(Min = 23, Max = 35)]
        public int windowsRuntimeTypeNamesSize;
        [Version(Min = 27, Max = 35)]
        public int windowsRuntimeStringsOffset; // const char*
        [Version(Min = 27, Max = 35)]
        public int windowsRuntimeStringsSize;
        [Version(Min = 24, Max = 35)]
        public int exportedTypeDefinitionsOffset; // TypeDefinitionIndex
        [Version(Min = 24, Max = 35)]
        public int exportedTypeDefinitionsSize;

        [Version(Min = 38)]
        public Il2CppSectionMetadata stringLiterals; // string data for managed code
        [Version(Min = 38)]
        public Il2CppSectionMetadata stringLiteralData;
        [Version(Min = 38)]
        public Il2CppSectionMetadata strings; // string data for metadata
        [Version(Min = 38)]
        public Il2CppSectionMetadata events; // Il2CppEventDefinition
        [Version(Min = 38)]
        public Il2CppSectionMetadata properties; // Il2CppPropertyDefinition
        [Version(Min = 38)]
        public Il2CppSectionMetadata methods; // Il2CppMethodDefinition
        [Version(Min = 38)]
        public Il2CppSectionMetadata parameterDefaultValues; // Il2CppParameterDefaultValue
        [Version(Min = 38)]
        public Il2CppSectionMetadata fieldDefaultValues; // Il2CppFieldDefaultValue
        [Version(Min = 38)]
        public Il2CppSectionMetadata fieldAndParameterDefaultValueData; // uint8_t
        [Version(Min = 38)]
        public Il2CppSectionMetadata fieldMarshaledSizes; // Il2CppFieldMarshaledSize
        [Version(Min = 38)]
        public Il2CppSectionMetadata parameters; // Il2CppParameterDefinition
        [Version(Min = 38)]
        public Il2CppSectionMetadata fields; // Il2CppFieldDefinition
        [Version(Min = 38)]
        public Il2CppSectionMetadata genericParameters; // Il2CppGenericParameter
        [Version(Min = 38)]
        public Il2CppSectionMetadata genericParameterConstraints; // TypeIndex
        [Version(Min = 38)]
        public Il2CppSectionMetadata genericContainers; // Il2CppGenericContainer
        [Version(Min = 38)]
        public Il2CppSectionMetadata nestedTypes; // TypeDefinitionIndex
        [Version(Min = 38)]
        public Il2CppSectionMetadata interfaces; // TypeIndex
        [Version(Min = 38)]
        public Il2CppSectionMetadata vtableMethods; // EncodedMethodIndex
        [Version(Min = 38)]
        public Il2CppSectionMetadata interfaceOffsets; // Il2CppInterfaceOffsetPair
        [Version(Min = 38)]
        public Il2CppSectionMetadata typeDefinitions; // Il2CppTypeDefinition
        [Version(Min = 106)]
        public Il2CppSectionMetadata typeInlineArrays; // Il2CppInlineArrayLength
        [Version(Min = 38)]
        public Il2CppSectionMetadata images; // Il2CppImageDefinition
        [Version(Min = 38)]
        public Il2CppSectionMetadata assemblies; // Il2CppAssemblyDefinition
        [Version(Min = 38)]
        public Il2CppSectionMetadata fieldRefs; // Il2CppFieldRef
        [Version(Min = 38)]
        public Il2CppSectionMetadata referencedAssemblies; // int32_t
        [Version(Min = 38)]
        public Il2CppSectionMetadata attributeData;
        [Version(Min = 38)]
        public Il2CppSectionMetadata attributeDataRanges;
        [Version(Min = 38)]
        public Il2CppSectionMetadata unresolvedIndirectCallParameterTypes; // TypeIndex
        [Version(Min = 38)]
        public Il2CppSectionMetadata unresolvedIndirectCallParameterRanges; // Il2CppMetadataRange
        [Version(Min = 38)]
        public Il2CppSectionMetadata windowsRuntimeTypeNames; // Il2CppWindowsRuntimeTypeNamePair
        [Version(Min = 38)]
        public Il2CppSectionMetadata windowsRuntimeStrings; // const char*
        [Version(Min = 38)]
        public Il2CppSectionMetadata exportedTypeDefinitions; // TypeDefinitionIndex
        [Version(Min = 108)]
        public Il2CppSectionMetadata methodSpecsOnGenericType; // Il2CppMethodSpecOnGenericType
        [Version(Min = 108)]
        public Il2CppSectionMetadata genericMethodSpecsOnType; // Il2CppGenericMethodSpecOnType
        [Version(Min = 108)]
        public Il2CppSectionMetadata methodSpecs; // Il2CppMethodSpec
        [Version(Min = 108)]
        public Il2CppSectionMetadata genericMethodFunctionsDefinitions; // Il2CppGenericMethodFunctionsDefinitions
        [Version(Min = 108)]
        public Il2CppSectionMetadata genericMethodFunctionsDefinitionsWithAdjustor; // Il2CppGenericMethodFunctionsDefinitionsWithAdjustor
        [Version(Min = 108)]
        public Il2CppSectionMetadata invokerIndices; // InvokerTableIndex per method, per image, in image order
        [Version(Min = 108)]
        public Il2CppSectionMetadata rgctxRanges; // Il2CppTokenRangePair (token + global start + length), per image in image order
        [Version(Min = 108)]
        public Il2CppSectionMetadata rgctxValues; // uint8 type + uint32 data, stride 5
        [Version(Min = 108)]
        public Il2CppSectionMetadata staticConstructorTypeIndices; // TypeDefinitionIndex per eager-init type, per image in image order
        [Version(Min = 110)]
        public Il2CppSectionMetadata generatedMethodTypeInfos; // Il2CppGeneratedMethodTypeInfo, sorted by typeIndex
        [Version(Min = 110)]
        public Il2CppSectionMetadata generatedMethodTokens; // uint32 per generated method, in global method-table order
    }

    public class Il2CppAssemblyDefinition
    {
        public int imageIndex;
        [Version(Min = 24.1)]
        public uint token;
        [Version(Min = 38)]
        public uint moduleToken;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Min = 20)]
        public int referencedAssemblyStart;
        [Version(Min = 20)]
        public int referencedAssemblyCount;
        public Il2CppAssemblyNameDefinition aname;
    }

    public class Il2CppAssemblyNameDefinition
    {
        public uint nameIndex;
        public uint cultureIndex;
        [Version(Max = 24.3)]
        public int hashValueIndex;
        public uint publicKeyIndex;
        public uint hash_alg;
        public int hash_len;
        public uint flags;
        public int major;
        public int minor;
        public int build;
        public int revision;
        [ArrayLength(Length = 8)]
        public byte[] public_key_token;
    }

    public class Il2CppImageDefinition
    {
        public uint nameIndex;
        public int assemblyIndex;

        [Index(IndexKind.TypeDefinition)]
        public int typeStart;
        public uint typeCount;

        [Version(Min = 24)]
        [Index(IndexKind.TypeDefinition)]
        public int exportedTypeStart;
        [Version(Min = 24)]
        public uint exportedTypeCount;

        [Index(IndexKind.Method)]
        public int entryPointIndex;
        [Version(Min = 19)]
        public uint token;

        [Version(Min = 24.1)]
        public int customAttributeStart;
        [Version(Min = 24.1)]
        public uint customAttributeCount;

        [Version(Min = 108)]
        public uint invokerIndicesStart;
        [Version(Min = 108)]
        public uint rgctxRangesStart;
        [Version(Min = 108)]
        public uint rgctxRangesCount;
        [Version(Min = 108)]
        [Index(IndexKind.TypeDefinition)]
        public int staticConstructorStart;
        [Version(Min = 108)]
        public uint staticConstructorCount;

        // Version 110+: per-image starts used to rebuild tokens, only methodStart is index-width
        [Version(Min = 110)]
        public int fieldStart;
        [Version(Min = 110)]
        public int propertyStart;
        [Version(Min = 110)]
        public int eventStart;
        [Version(Min = 110)]
        [Index(IndexKind.Method)]
        public int methodStart;
    }

    public class Il2CppTypeDefinition
    {
        public uint nameIndex;
        public uint namespaceIndex;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Index(IndexKind.Type)]
        public int byvalTypeIndex;
        [Version(Max = 24.5)]
        public int byrefTypeIndex;

        [Index(IndexKind.Type)]
        public int declaringTypeIndex;
        [Index(IndexKind.Type)]
        public int parentIndex;
        [Version(Max = 31)]
        public int elementTypeIndex; // we can probably remove this one. Only used for enums

        [Version(Max = 24.1)]
        public int rgctxStartIndex;
        [Version(Max = 24.1)]
        public int rgctxCount;

        [Index(IndexKind.GenericContainer)]
        public int genericContainerIndex;

        [Version(Max = 22)]
        public int delegateWrapperFromManagedToNativeIndex;
        [Version(Max = 22)]
        public int marshalingFunctionsIndex;
        [Version(Min = 21, Max = 22)]
        public int ccwFunctionIndex;
        [Version(Min = 21, Max = 22)]
        public int guidIndex;

        public uint flags;

        [Index(IndexKind.Field)]
        public int fieldStart;
        [Index(IndexKind.Method)]
        public int methodStart;
        [Index(IndexKind.Event)]
        public int eventStart;
        [Index(IndexKind.Property)]
        public int propertyStart;
        [Index(IndexKind.NestedType)]
        public int nestedTypesStart;
        [Index(IndexKind.Interfaces)]
        public int interfacesStart;
        public int vtableStart;
        [Index(IndexKind.Interfaces)]
        public int interfaceOffsetsStart;

        public ushort method_count;
        public ushort property_count;
        public ushort field_count;
        public ushort event_count;
        public ushort nested_type_count;
        public ushort vtable_count;
        public ushort interfaces_count;
        public ushort interface_offsets_count;

        // bitfield to portably encode boolean values as single bits
        // 01 - valuetype;
        // 02 - enumtype;
        // 03 - has_finalize;
        // 04 - has_cctor;
        // 05 - is_blittable;
        // 06 - is_import_or_windows_runtime;
        // 07-10 - One of nine possible PackingSize values (0, 1, 2, 4, 8, 16, 32, 64, or 128)
        // 11 - PackingSize is default
        // 12 - ClassSize is default
        // 13-16 - One of nine possible PackingSize values (0, 1, 2, 4, 8, 16, 32, 64, or 128) - the specified packing size (even for explicit layouts)
        // 17 - IsByRefLike (e.g. ref struct) (v106+)
        // 18 - HasInlineArray (v106+)
        // 19 - HasIDynamicInterfaceCastable (v110+)
        // 20 - HasGeneratedMethods (type has IL2CPP-injected methods) (v110+)
        public uint bitfield;
        // Version 110+: tokens are rebuilt by Metadata.SynthesizeTokens
        [Version(Min = 19, Max = 108)]
        public uint token;

        public bool IsValueType => (bitfield & 0x1) == 1;
        public bool IsEnum => ((bitfield >> 1) & 0x1) == 1;
        public bool HasGeneratedMethods => ((bitfield >> 19) & 0x1) == 1;

        public int GetEnumElementTypeIndex(double version)
        {
            // In versions < 35, elementTypeIndex exists
            // In versions >= 35, we use parentIndex instead
            var field = typeof(Il2CppTypeDefinition).GetField("elementTypeIndex");
            var versionAttributes = field.GetCustomAttributes<VersionAttribute>().ToArray();
            if (versionAttributes.Length <= 0)
            {
                return elementTypeIndex;
            }
            var versionAttribute = versionAttributes[0];

            if (versionAttribute.Min <= version && version <= versionAttribute.Max)
            {
                return elementTypeIndex;
            }

            // Field does not exist; return parentIndex instead
            return parentIndex;
        }
    }

    public class Il2CppMethodDefinition
    {
        public uint nameIndex;
        [Index(IndexKind.TypeDefinition)]
        public int declaringType;
        [Index(IndexKind.Type)]
        public int returnType;
        [Version(Min = 31)]
        public int returnParameterToken;
        [Index(IndexKind.Parameter)]
        public int parameterStart;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Index(IndexKind.GenericContainer)]
        public int genericContainerIndex;
        [Version(Max = 24.1)]
        public int methodIndex;
        [Version(Max = 24.1)]
        public int invokerIndex;
        [Version(Max = 24.1)]
        public int delegateWrapperIndex;
        [Version(Max = 24.1)]
        public int rgctxStartIndex;
        [Version(Max = 24.1)]
        public int rgctxCount;
        [Version(Max = 108)]
        public uint token;
        public ushort flags;
        public ushort iflags;
        public ushort slot;
        public ushort parameterCount;
    }

    public class Il2CppParameterDefinition
    {
        public uint nameIndex;
        public uint token;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Index(IndexKind.Type)]
        public int typeIndex;
    }

    public class Il2CppFieldDefinition
    {
        public uint nameIndex;
        [Index(IndexKind.Type)]
        public int typeIndex;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Min = 19, Max = 108)]
        public uint token;
    }

    public class Il2CppFieldDefaultValue
    {
        [Index(IndexKind.Field)]
        public int fieldIndex;
        [Index(IndexKind.Type)]
        public int typeIndex;
        [Index(IndexKind.DefaultValueData)]
        public int dataIndex;
    }

    public class Il2CppPropertyDefinition
    {
        public uint nameIndex;
        [Index(IndexKind.Method)]
        public int get;
        [Index(IndexKind.Method)]
        public int set;
        public uint attrs;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Min = 19, Max = 108)]
        public uint token;
    }

    public class Il2CppInterfaceOffsetPair
    {
        [Index(IndexKind.Type)]
        public int interfaceTypeIndex;
        public int offset;
    }


    public class Il2CppCustomAttributeTypeRange
    {
        [Version(Min = 24.1)]
        public uint token;
        public int start;
        public int count;
    }

    public class Il2CppMetadataUsageList
    {
        public uint start;
        public uint count;
    }

    public class Il2CppMetadataUsagePair
    {
        public uint destinationIndex;
        public uint encodedSourceIndex;
    }

    public class Il2CppStringLiteral
    {
        [Version(Max = 31)]
        public uint length;
        public int dataIndex;
    }

    public class Il2CppParameterDefaultValue
    {
        [Index(IndexKind.Parameter)]
        public int parameterIndex;
        [Index(IndexKind.Type)]
        public int typeIndex;
        [Index(IndexKind.DefaultValueData)]
        public int dataIndex;
    }

    public class Il2CppEventDefinition
    {
        public uint nameIndex;
        [Index(IndexKind.Type)]
        public int typeIndex;
        [Index(IndexKind.Method)]
        public int add;
        [Index(IndexKind.Method)]
        public int remove;
        [Index(IndexKind.Method)]
        public int raise;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Min = 19, Max = 108)]
        public uint token;
    }

    public class Il2CppGenericContainer
    {
        /* index of the generic type definition or the generic method definition corresponding to this container */
        public int ownerIndex; // either index into Il2CppClass metadata array or Il2CppMethodDefinition array
        [Version(Max = 39)]
        public int type_argc_v39;
        [Version(Min = 106)]
        public ushort type_argc_v106;
        /* If true, we're a generic method, otherwise a generic type definition. */
        [Version(Max = 39)]
        public int is_method;
        [Version(Min = 106)]
        public byte is_method_v106;
        /* Our type parameters. */
        [Index(IndexKind.GenericParameter)]
        public int genericParameterStart;

        public int type_argc => type_argc_v106 != 0 ? type_argc_v106 : type_argc_v39;
    }

    public class Il2CppFieldRef
    {
        [Index(IndexKind.Type)]
        public int typeIndex;
        [Index(IndexKind.Field)]
        public int fieldIndex; // local offset into type fields
    }

    public class Il2CppGenericParameter
    {
        [Index(IndexKind.GenericContainer)]
        public int ownerIndex;  /* Type or method this parameter was defined in. */
        public uint nameIndex;
        public short constraintsStart;
        public short constraintsCount;
        public ushort num;
        public ushort flags;
    }
    public class Il2CppConstraintIndex
    {
        [Index(IndexKind.Type)]
        public int index;
    }

    // Version 108+: method specs, see Metadata.ReadGenericTables
    public class Il2CppMethodSpecOnGenericType
    {
        [Index(IndexKind.Method)]
        public int methodDefinitionIndex;
        [Index(IndexKind.GenericInst)]
        public int classIndexIndex;
    }

    public class Il2CppGenericMethodSpecOnType
    {
        [Index(IndexKind.Method)]
        public int methodDefinitionIndex;
        [Index(IndexKind.GenericInst)]
        public int methodIndexIndex;
    }

    public class Il2CppMetadataMethodSpec
    {
        [Index(IndexKind.Method)]
        public int methodDefinitionIndex;
        [Index(IndexKind.GenericInst)]
        public int classIndexIndex;
        [Index(IndexKind.GenericInst)]
        public int methodIndexIndex;
    }

    public class Il2CppMetadataGenericMethodFunctions
    {
        [Index(IndexKind.GenericMethod)]
        public int genericMethodIndex;
        [Index(IndexKind.MethodPointerTable)]
        public int methodIndex;
        [Index(IndexKind.InvokerTable)]
        public int invokerIndex;
    }

    public class Il2CppMetadataGenericMethodFunctionsWithAdjustor
    {
        [Index(IndexKind.GenericMethod)]
        public int genericMethodIndex;
        [Index(IndexKind.MethodPointerTable)]
        public int methodIndex;
        [Index(IndexKind.InvokerTable)]
        public int invokerIndex;
        [Index(IndexKind.AdjustorThunk)]
        public int adjustorThunkIndex;
    }

    // Version 110+: the runtime bsearches this as a raw struct, so no index-width packing here
    public class Il2CppGeneratedMethodTypeInfo
    {
        public int typeIndex;
        public int generatedMethodStart;
        public uint generatedMethodCount;
    }

    public enum Il2CppRGCTXDataType
    {
        IL2CPP_RGCTX_DATA_INVALID,
        IL2CPP_RGCTX_DATA_TYPE,
        IL2CPP_RGCTX_DATA_CLASS,
        IL2CPP_RGCTX_DATA_METHOD,
        IL2CPP_RGCTX_DATA_ARRAY,
        IL2CPP_RGCTX_DATA_CONSTRAINED,
        IL2CPP_RGCTX_DATA_CONSTRAINED_CALL_METHOD, // 108+
        IL2CPP_RGCTX_DATA_FIELD_OFFSET_TYPE, // 108+
        IL2CPP_RGCTX_DATA_FIELD_OFFSET_FIELD, // 108+
    }

    public class Il2CppRGCTXDefinitionData
    {
        public int rgctxDataDummy;
        public int methodIndex => rgctxDataDummy;
        public int typeIndex => rgctxDataDummy;
    }

    public class Il2CppRGCTXDefinition
    {
        public Il2CppRGCTXDataType type => type_post29 == 0 ? (Il2CppRGCTXDataType)type_pre29 : (Il2CppRGCTXDataType)type_post29;
        [Version(Max = 27.1)]
        public int type_pre29;
        [Version(Min = 29)]
        public ulong type_post29;
        // inline up to 27.1 and filled from the metadata file for 108+, otherwise _data points into the binary
        [Version(Max = 27.1)]
        public Il2CppRGCTXDefinitionData data;
        [Version(Min = 27.2)]
        public ulong _data;
    }

    // Version 108+ drops kIl2CppMetadataUsageIl2CppType on disk, use Metadata.DecodeUsage
    public enum Il2CppMetadataUsage
    {
        kIl2CppMetadataUsageInvalid,
        kIl2CppMetadataUsageTypeInfo,
        kIl2CppMetadataUsageIl2CppType,
        kIl2CppMetadataUsageMethodDef,
        kIl2CppMetadataUsageFieldInfo,
        kIl2CppMetadataUsageStringLiteral,
        kIl2CppMetadataUsageMethodRef,
        kIl2CppMetadataUsageFieldRva,
    };

    public class Il2CppCustomAttributeDataRange
    {
        public uint token;
        public uint startOffset;
    }
}
