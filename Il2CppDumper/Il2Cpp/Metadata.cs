using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Il2CppDumper
{
    public sealed class Metadata : BinaryStream
    {
        public Il2CppGlobalMetadataHeader header;
        public Il2CppImageDefinition[] imageDefs;
        public Il2CppAssemblyDefinition[] assemblyDefs;
        public Il2CppTypeDefinition[] typeDefs;
        public Il2CppMethodDefinition[] methodDefs;
        public Il2CppParameterDefinition[] parameterDefs;
        public Il2CppFieldDefinition[] fieldDefs;
        private readonly Dictionary<int, Il2CppFieldDefaultValue> fieldDefaultValuesDic;
        private readonly Dictionary<int, Il2CppParameterDefaultValue> parameterDefaultValuesDic;
        public Il2CppPropertyDefinition[] propertyDefs;
        public Il2CppCustomAttributeTypeRange[] attributeTypeRanges;
        public Il2CppCustomAttributeDataRange[] attributeDataRanges;
        private readonly Dictionary<Il2CppImageDefinition, Dictionary<uint, int>> attributeTypeRangesDic;
        public Il2CppStringLiteral[] stringLiterals;
        private readonly Il2CppMetadataUsageList[] metadataUsageLists;
        private readonly Il2CppMetadataUsagePair[] metadataUsagePairs;
        public int[] attributeTypes;
        public int[] interfaceIndices; // TypeIndex, indexed by interfacesStart
        public readonly Il2CppInterfaceOffsetPair[] interfaceOffsetPairs;
        public Dictionary<Il2CppMetadataUsage, SortedDictionary<uint, uint>> metadataUsageDic;
        public long metadataUsagesCount;
        public int[] nestedTypeIndices;
        public Il2CppEventDefinition[] eventDefs;
        public Il2CppGenericContainer[] genericContainers;
        public Il2CppFieldRef[] fieldRefs;
        public Il2CppGenericParameter[] genericParameters;
        public Il2CppConstraintIndex[] constraintIndices;
        public uint[] vtableMethods;
        public Il2CppRGCTXDefinition[] rgctxEntries;
        // Version 108+
        public Il2CppTokenRangePair[] rgctxRanges;
        public Il2CppRGCTXDefinition[] rgctxValues;

        private readonly Dictionary<uint, string> stringCache = new();

        public Metadata(Stream stream) : base(stream)
        {
            var sanity = ReadUInt32();
            if (sanity != 0xFAB11BAF)
            {
                throw new InvalidDataException("ERROR: Metadata file supplied is not valid metadata file.");
            }
            var version = ReadInt32();
            if (version < 0 || version > 1000)
            {
                throw new InvalidDataException("ERROR: Metadata file supplied is not valid metadata file.");
            }
            if (version < 16 || version > 108 || (version > 39 && version < 106))
            {
                throw new NotSupportedException($"ERROR: Metadata file supplied is not a supported version[{version}].");
            }
            Version = version;
            header = ReadClass<Il2CppGlobalMetadataHeader>(0);
            if (version == 24)
            {
                if (header.stringLiteralOffset == 264)
                {
                    Version = 24.2;
                    header = ReadClass<Il2CppGlobalMetadataHeader>(0);
                }
                else
                {
                    imageDefs = ReadMetadataClassArray<Il2CppImageDefinition>(header.imagesOffset, header.imagesSize);
                    if (imageDefs.Any(x => x.token != 1))
                    {
                        Version = 24.1;
                    }
                }
            }

            InitIndexSizes();

            imageDefs = Version < 38
                ? ReadMetadataClassArray<Il2CppImageDefinition>(header.imagesOffset, header.imagesSize)
                : ReadMetadataClassArray<Il2CppImageDefinition>(header.images.offset, (int)header.images.size);
            if (Version == 24.2 && header.assembliesSize / 68 < imageDefs.Length)
            {
                Version = 24.4;
            }
            var v241Plus = false;
            if (Version == 24.1 && header.assembliesSize / 64 == imageDefs.Length)
            {
                v241Plus = true;
            }
            if (v241Plus)
            {
                Version = 24.4;
            }
            assemblyDefs = Version < 38
                ? ReadMetadataClassArray<Il2CppAssemblyDefinition>(header.assembliesOffset, header.assembliesSize)
                : ReadMetadataClassArray<Il2CppAssemblyDefinition>(header.assemblies.offset, (int)header.assemblies.size);
            if (v241Plus)
            {
                Version = 24.1;
            }
            typeDefs = Version < 38
                ? ReadMetadataClassArray<Il2CppTypeDefinition>(header.typeDefinitionsOffset, header.typeDefinitionsSize)
                : ReadMetadataClassArray<Il2CppTypeDefinition>(header.typeDefinitions.offset, (int)header.typeDefinitions.size);
            methodDefs = Version < 38
                ? ReadMetadataClassArray<Il2CppMethodDefinition>(header.methodsOffset, header.methodsSize)
                : ReadMetadataClassArray<Il2CppMethodDefinition>(header.methods.offset, (int)header.methods.size);
            parameterDefs = Version < 38
                ? ReadMetadataClassArray<Il2CppParameterDefinition>(header.parametersOffset, header.parametersSize)
                : ReadMetadataClassArray<Il2CppParameterDefinition>(header.parameters.offset, (int)header.parameters.size);
            fieldDefs = Version < 38
                ? ReadMetadataClassArray<Il2CppFieldDefinition>(header.fieldsOffset, header.fieldsSize)
                : ReadMetadataClassArray<Il2CppFieldDefinition>(header.fields.offset, (int)header.fields.size);
            var fieldDefaultValues = Version < 38
                ? ReadMetadataClassArray<Il2CppFieldDefaultValue>(header.fieldDefaultValuesOffset, header.fieldDefaultValuesSize)
                : ReadMetadataClassArray<Il2CppFieldDefaultValue>(header.fieldDefaultValues.offset, (int)header.fieldDefaultValues.size);
            var parameterDefaultValues = Version < 38
                ? ReadMetadataClassArray<Il2CppParameterDefaultValue>(header.parameterDefaultValuesOffset, header.parameterDefaultValuesSize)
                : ReadMetadataClassArray<Il2CppParameterDefaultValue>(header.parameterDefaultValues.offset, (int)header.parameterDefaultValues.size);
            fieldDefaultValuesDic = fieldDefaultValues.ToDictionary(x => x.fieldIndex);
            parameterDefaultValuesDic = parameterDefaultValues.ToDictionary(x => x.parameterIndex);
            propertyDefs = Version < 38
                ? ReadMetadataClassArray<Il2CppPropertyDefinition>(header.propertiesOffset, header.propertiesSize)
                : ReadMetadataClassArray<Il2CppPropertyDefinition>(header.properties.offset, (int)header.properties.size);
            interfaceIndices = Version < 38
                ? ReadClassArray<int>(header.interfacesOffset, header.interfacesSize / 4)
                : ReadIndexArray(IndexKind.Type, header.interfaces.offset, (int)header.interfaces.size);
            interfaceOffsetPairs = Version < 38
                ? ReadMetadataClassArray<Il2CppInterfaceOffsetPair>((uint)header.interfaceOffsetsOffset, header.interfaceOffsetsSize)
                : ReadMetadataClassArray<Il2CppInterfaceOffsetPair>(header.interfaceOffsets.offset, (int)header.interfaceOffsets.size);
            nestedTypeIndices = Version < 38
                ? ReadClassArray<int>(header.nestedTypesOffset, header.nestedTypesSize / 4)
                : ReadClassArray<int>(header.nestedTypes.offset, header.nestedTypes.size / 4);
            eventDefs = Version < 38
                ? ReadMetadataClassArray<Il2CppEventDefinition>(header.eventsOffset, header.eventsSize)
                : ReadMetadataClassArray<Il2CppEventDefinition>(header.events.offset, (int)header.events.size);
            genericContainers = Version < 38
                ? ReadMetadataClassArray<Il2CppGenericContainer>(header.genericContainersOffset, header.genericContainersSize)
                : ReadMetadataClassArray<Il2CppGenericContainer>(header.genericContainers.offset, (int)header.genericContainers.size);
            genericParameters = Version < 38
                ? ReadMetadataClassArray<Il2CppGenericParameter>(header.genericParametersOffset, header.genericParametersSize)
                : ReadMetadataClassArray<Il2CppGenericParameter>(header.genericParameters.offset, (int)header.genericParameters.size);
            constraintIndices = Version < 38
                ? ReadMetadataClassArray<Il2CppConstraintIndex>(header.genericParameterConstraintsOffset, header.genericParameterConstraintsSize)
                : ReadMetadataClassArray<Il2CppConstraintIndex>(header.genericParameterConstraints.offset, (int)header.genericParameterConstraints.size);
            vtableMethods = Version < 38
                ? ReadClassArray<uint>(header.vtableMethodsOffset, header.vtableMethodsSize / 4)
                : ReadClassArray<uint>(header.vtableMethods.offset, header.vtableMethods.size / 4);
            stringLiterals = Version < 38
                ? ReadMetadataClassArray<Il2CppStringLiteral>(header.stringLiteralOffset, header.stringLiteralSize)
                : ReadMetadataClassArray<Il2CppStringLiteral>(header.stringLiterals.offset, (int)header.stringLiterals.size);
            if (Version > 16)
            {
                fieldRefs = Version < 38
                    ? ReadMetadataClassArray<Il2CppFieldRef>(header.fieldRefsOffset, header.fieldRefsSize)
                    : ReadMetadataClassArray<Il2CppFieldRef>(header.fieldRefs.offset, (int)header.fieldRefs.size);
                if (Version < 27)
                {
                    metadataUsageLists = ReadMetadataClassArray<Il2CppMetadataUsageList>(header.metadataUsageListsOffset, header.metadataUsageListsCount);
                    metadataUsagePairs = ReadMetadataClassArray<Il2CppMetadataUsagePair>(header.metadataUsagePairsOffset, header.metadataUsagePairsCount);

                    ProcessingMetadataUsage();
                }
            }
            if (Version > 20 && Version < 29)
            {
                attributeTypeRanges = ReadMetadataClassArray<Il2CppCustomAttributeTypeRange>(header.attributesInfoOffset, header.attributesInfoCount);
                attributeTypes = ReadClassArray<int>(header.attributeTypesOffset, header.attributeTypesCount / 4);
            }
            if (Version >= 29)
            {
                attributeDataRanges = Version < 38
                    ? ReadMetadataClassArray<Il2CppCustomAttributeDataRange>(header.attributeDataRangeOffset, header.attributeDataRangeSize)
                    : ReadMetadataClassArray<Il2CppCustomAttributeDataRange>(header.attributeDataRanges.offset, (int)header.attributeDataRanges.size);
            }
            if (Version > 24)
            {
                attributeTypeRangesDic = new Dictionary<Il2CppImageDefinition, Dictionary<uint, int>>();
                foreach (var imageDef in imageDefs)
                {
                    var dic = new Dictionary<uint, int>();
                    attributeTypeRangesDic[imageDef] = dic;
                    var end = imageDef.customAttributeStart + imageDef.customAttributeCount;
                    for (int i = imageDef.customAttributeStart; i < end; i++)
                    {
                        if (Version >= 29)
                        {
                            dic.Add(attributeDataRanges[i].token, i);
                        }
                        else
                        {
                            dic.Add(attributeTypeRanges[i].token, i);
                        }
                    }
                }
            }
            if (Version <= 24.1)
            {
                rgctxEntries = ReadMetadataClassArray<Il2CppRGCTXDefinition>(header.rgctxEntriesOffset, header.rgctxEntriesCount);
            }
            if (Version >= 108)
            {
                rgctxRanges = ReadMetadataClassArray<Il2CppTokenRangePair>(header.rgctxRanges.offset, (int)header.rgctxRanges.size);
                rgctxValues = ReadRGCTXValues();
            }
        }

        // Keep in sync with SerializedIndexSizes in vm/GlobalMetadata.cpp
        private void InitIndexSizes()
        {
            if (Version < 38)
            {
                return;
            }
            // TypeIndex size comes from the binary's typesCount, infer it from the stride of the interfaces section instead
            if (header.interfaces.count > 0)
            {
                IndexSizes[(int)IndexKind.Type] = (int)(header.interfaces.size / header.interfaces.count);
            }
            else if (header.fields.count > 0)
            {
                IndexSizes[(int)IndexKind.Type] = (int)(header.fields.size / header.fields.count) - 8;
            }
            IndexSizes[(int)IndexKind.TypeDefinition] = GetIndexSize((int)header.typeDefinitions.count);
            IndexSizes[(int)IndexKind.GenericContainer] = GetIndexSize((int)header.genericContainers.count);
            if (Version >= 39)
            {
                IndexSizes[(int)IndexKind.Parameter] = GetIndexSize((int)header.parameters.count);
            }
            if (Version >= 106)
            {
                IndexSizes[(int)IndexKind.GenericParameter] = GetIndexSize((int)header.genericParameters.count);
                IndexSizes[(int)IndexKind.Interfaces] = GetIndexSize((int)header.interfaceOffsets.count);
                IndexSizes[(int)IndexKind.Event] = GetIndexSize((int)header.events.count);
                IndexSizes[(int)IndexKind.Property] = GetIndexSize((int)header.properties.count);
                IndexSizes[(int)IndexKind.NestedType] = GetIndexSize((int)header.nestedTypes.count);
                IndexSizes[(int)IndexKind.Field] = GetIndexSize((int)header.fields.count);
                IndexSizes[(int)IndexKind.DefaultValueData] = GetIndexSize((int)header.fieldAndParameterDefaultValueData.count);
                IndexSizes[(int)IndexKind.Method] = GetIndexSize((int)header.methods.count);
            }
            if (Version >= 108)
            {
                var genericMethodCount = header.methodSpecsOnGenericType.count + header.genericMethodSpecsOnType.count + header.methodSpecs.count;
                IndexSizes[(int)IndexKind.GenericMethod] = GetIndexSize((int)genericMethodCount);
                IndexSizes[(int)IndexKind.AdjustorThunk] = GetIndexSize((int)header.genericMethodFunctionsDefinitionsWithAdjustor.count);
                // GenericInstIndex size comes from the binary's genericInstsCount, infer it from the method spec stride instead
                var methodWidth = IndexSizes[(int)IndexKind.Method];
                if (header.methodSpecs.count > 0)
                {
                    IndexSizes[(int)IndexKind.GenericInst] = ((int)(header.methodSpecs.size / header.methodSpecs.count) - methodWidth) / 2;
                }
                else if (header.genericMethodSpecsOnType.count > 0)
                {
                    IndexSizes[(int)IndexKind.GenericInst] = (int)(header.genericMethodSpecsOnType.size / header.genericMethodSpecsOnType.count) - methodWidth;
                }
                else if (header.methodSpecsOnGenericType.count > 0)
                {
                    IndexSizes[(int)IndexKind.GenericInst] = (int)(header.methodSpecsOnGenericType.size / header.methodSpecsOnGenericType.count) - methodWidth;
                }
            }
        }

        public static int GetIndexSize(int count)
        {
            if (count <= 0xFF) return 1;
            if (count <= 0xFFFF) return 2;
            return 4;
        }

        private T[] ReadMetadataClassArray<T>(uint addr, int count) where T : new()
        {
            return ReadClassArray<T>(addr, count / SizeOf(typeof(T)));
        }

        // Version 108+: uint8 type + uint32 data, stride 5
        private Il2CppRGCTXDefinition[] ReadRGCTXValues()
        {
            var count = (int)header.rgctxValues.count;
            var stride = count > 0 ? (int)(header.rgctxValues.size / header.rgctxValues.count) : 5;
            var result = new Il2CppRGCTXDefinition[count];
            for (var i = 0; i < count; i++)
            {
                Position = header.rgctxValues.offset + (ulong)(i * stride);
                var type = ReadByte();
                var data = ReadInt32();
                result[i] = new Il2CppRGCTXDefinition
                {
                    type_post29 = type,
                    data = new Il2CppRGCTXDefinitionData { rgctxDataDummy = data }
                };
            }
            return result;
        }

        // Version 108+: the three method spec tables form one GenericMethodIndex space in this order
        public (Il2CppMethodSpec[] methodSpecs, Il2CppGenericMethodFunctionsDefinitions[] genericMethodTable) ReadGenericTables(long genericMethodPointersCount, long invokerPointersCount)
        {
            IndexSizes[(int)IndexKind.MethodPointerTable] = GetIndexSize((int)genericMethodPointersCount);
            IndexSizes[(int)IndexKind.InvokerTable] = GetIndexSize((int)invokerPointersCount);
            var expectedStride = IndexSizes[(int)IndexKind.GenericMethod] + IndexSizes[(int)IndexKind.MethodPointerTable] + IndexSizes[(int)IndexKind.InvokerTable];
            CheckStride(header.genericMethodFunctionsDefinitions, expectedStride, "genericMethodFunctionsDefinitions");
            CheckStride(header.genericMethodFunctionsDefinitionsWithAdjustor, expectedStride + IndexSizes[(int)IndexKind.AdjustorThunk], "genericMethodFunctionsDefinitionsWithAdjustor");

            var onGenericType = ReadMetadataClassArray<Il2CppMethodSpecOnGenericType>(header.methodSpecsOnGenericType.offset, (int)header.methodSpecsOnGenericType.size);
            var genericOnType = ReadMetadataClassArray<Il2CppGenericMethodSpecOnType>(header.genericMethodSpecsOnType.offset, (int)header.genericMethodSpecsOnType.size);
            var both = ReadMetadataClassArray<Il2CppMetadataMethodSpec>(header.methodSpecs.offset, (int)header.methodSpecs.size);
            var methodSpecs = new Il2CppMethodSpec[onGenericType.Length + genericOnType.Length + both.Length];
            var n = 0;
            foreach (var spec in onGenericType)
            {
                methodSpecs[n++] = new Il2CppMethodSpec { methodDefinitionIndex = spec.methodDefinitionIndex, classIndexIndex = spec.classIndexIndex, methodIndexIndex = -1 };
            }
            foreach (var spec in genericOnType)
            {
                methodSpecs[n++] = new Il2CppMethodSpec { methodDefinitionIndex = spec.methodDefinitionIndex, classIndexIndex = -1, methodIndexIndex = spec.methodIndexIndex };
            }
            foreach (var spec in both)
            {
                methodSpecs[n++] = new Il2CppMethodSpec { methodDefinitionIndex = spec.methodDefinitionIndex, classIndexIndex = spec.classIndexIndex, methodIndexIndex = spec.methodIndexIndex };
            }

            var plain = ReadMetadataClassArray<Il2CppMetadataGenericMethodFunctions>(header.genericMethodFunctionsDefinitions.offset, (int)header.genericMethodFunctionsDefinitions.size);
            var withAdjustor = ReadMetadataClassArray<Il2CppMetadataGenericMethodFunctionsWithAdjustor>(header.genericMethodFunctionsDefinitionsWithAdjustor.offset, (int)header.genericMethodFunctionsDefinitionsWithAdjustor.size);
            var table = new Il2CppGenericMethodFunctionsDefinitions[plain.Length + withAdjustor.Length];
            n = 0;
            foreach (var entry in plain)
            {
                table[n++] = new Il2CppGenericMethodFunctionsDefinitions
                {
                    genericMethodIndex = entry.genericMethodIndex,
                    indices = new Il2CppGenericMethodIndices { methodIndex = entry.methodIndex, invokerIndex = entry.invokerIndex, adjustorThunk = -1 }
                };
            }
            foreach (var entry in withAdjustor)
            {
                table[n++] = new Il2CppGenericMethodFunctionsDefinitions
                {
                    genericMethodIndex = entry.genericMethodIndex,
                    indices = new Il2CppGenericMethodIndices { methodIndex = entry.methodIndex, invokerIndex = entry.invokerIndex, adjustorThunk = entry.adjustorThunkIndex }
                };
            }

            return (methodSpecs, table);
        }

        private static void CheckStride(Il2CppSectionMetadata section, int expectedStride, string name)
        {
            if (section.count > 0 && section.size / section.count != expectedStride)
            {
                throw new InvalidDataException($"ERROR: Unexpected {name} stride {section.size / section.count}, expected {expectedStride}.");
            }
        }

        public Dictionary<uint, Il2CppRGCTXDefinition[]> GetImageRGCTXs(Il2CppImageDefinition imageDef)
        {
            var result = new Dictionary<uint, Il2CppRGCTXDefinition[]>();
            var end = imageDef.rgctxRangesStart + imageDef.rgctxRangesCount;
            for (var i = imageDef.rgctxRangesStart; i < end; i++)
            {
                var range = rgctxRanges[i];
                var defs = new Il2CppRGCTXDefinition[range.range.length];
                Array.Copy(rgctxValues, range.range.start, defs, 0, range.range.length);
                result[range.token] = defs;
            }
            return result;
        }

        public bool GetFieldDefaultValueFromIndex(int index, out Il2CppFieldDefaultValue value)
        {
            return fieldDefaultValuesDic.TryGetValue(index, out value);
        }

        public bool GetParameterDefaultValueFromIndex(int index, out Il2CppParameterDefaultValue value)
        {
            return parameterDefaultValuesDic.TryGetValue(index, out value);
        }

        public uint GetDefaultValueFromIndex(int index)
        {
            return (uint)((Version < 38 ? header.fieldAndParameterDefaultValueDataOffset : header.fieldAndParameterDefaultValueData.offset) + index);
        }

        public string GetStringFromIndex(uint index)
        {
            if (!stringCache.TryGetValue(index, out var result))
            {
                result = ReadStringToNull((Version <= 35 ? header.stringOffset : header.strings.offset) + index);
                stringCache.Add(index, result);
            }
            return result;
        }

        public int GetCustomAttributeIndex(Il2CppImageDefinition imageDef, int customAttributeIndex, uint token)
        {
            if (Version > 24)
            {
                if (attributeTypeRangesDic[imageDef].TryGetValue(token, out var index))
                {
                    return index;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return customAttributeIndex;
            }
        }

        public string GetStringLiteralFromIndex(uint index)
        {
            var currentLiteral = stringLiterals[index];
            int stringLength = 0;
            if (index < stringLiterals.Length - 1)
            {
                var nextLiteral = stringLiterals[index + 1];
                stringLength = nextLiteral.dataIndex - currentLiteral.dataIndex;
            }
            else
            {
                // Last string, read to end of section
                stringLength = (Version < 38 ? header.stringLiteralDataSize : (int)header.stringLiteralData.size) - currentLiteral.dataIndex;
            }

            Position = (uint)((Version < 38 ? header.stringLiteralDataOffset : header.stringLiteralData.offset) + currentLiteral.dataIndex);
            return Encoding.UTF8.GetString(ReadBytes(stringLength));
        }

        private void ProcessingMetadataUsage()
        {
            metadataUsageDic = new Dictionary<Il2CppMetadataUsage, SortedDictionary<uint, uint>>();
            for (uint i = 1; i <= 6; i++)
            {
                metadataUsageDic[(Il2CppMetadataUsage)i] = new SortedDictionary<uint, uint>();
            }
            foreach (var metadataUsageList in metadataUsageLists)
            {
                for (int i = 0; i < metadataUsageList.count; i++)
                {
                    var offset = metadataUsageList.start + i;
                    if (offset >= metadataUsagePairs.Length)
                    {
                        continue;
                    }
                    var metadataUsagePair = metadataUsagePairs[offset];
                    var usage = GetEncodedIndexType(metadataUsagePair.encodedSourceIndex);
                    var decodedIndex = GetDecodedMethodIndex(metadataUsagePair.encodedSourceIndex);
                    metadataUsageDic[(Il2CppMetadataUsage)usage][metadataUsagePair.destinationIndex] = decodedIndex;
                }
            }
            //metadataUsagesCount = metadataUsagePairs.Max(x => x.destinationIndex) + 1;
            metadataUsagesCount = metadataUsageDic.Max(x => x.Value.Select(y => y.Key).DefaultIfEmpty().Max()) + 1;
        }

        public static uint GetEncodedIndexType(uint index)
        {
            return (index & 0xE0000000) >> 29;
        }

        // Version 108 removed kIl2CppMetadataUsageIl2CppType, later kinds shift down by one
        public Il2CppMetadataUsage DecodeUsage(uint encodedIndex)
        {
            var usage = GetEncodedIndexType(encodedIndex);
            if (Version >= 108 && usage >= 2)
            {
                usage++;
            }
            return (Il2CppMetadataUsage)usage;
        }

        public uint GetDecodedMethodIndex(uint index)
        {
            if (Version >= 27)
            {
                return (index & 0x1FFFFFFEU) >> 1;
            }
            return index & 0x1FFFFFFFU;
        }

        public int SizeOf(Type type)
        {
            var size = 0;
            foreach (var i in type.GetFields())
            {
                var attr = (VersionAttribute)Attribute.GetCustomAttribute(i, typeof(VersionAttribute));
                if (attr != null)
                {
                    if (Version < attr.Min || Version > attr.Max)
                        continue;
                }
                var indexAttribute = i.GetCustomAttribute<IndexAttribute>();
                var fieldType = i.FieldType;
                if (indexAttribute != null)
                {
                    size += IndexSizes[(int)indexAttribute.Kind];
                }
                else if (fieldType.IsPrimitive)
                {
                    size += GetPrimitiveTypeSize(fieldType.Name);
                }
                else if (fieldType.IsEnum)
                {
                    var e = fieldType.GetField("value__").FieldType;
                    size += GetPrimitiveTypeSize(e.Name);
                }
                else if (fieldType.IsArray)
                {
                    var arrayLengthAttribute = i.GetCustomAttribute<ArrayLengthAttribute>();
                    size += arrayLengthAttribute.Length;
                }
                else
                {
                    size += SizeOf(fieldType);
                }
            }
            return size;
        }

        static int GetPrimitiveTypeSize(string name)
        {
            return name switch
            {
                "Int32" or "UInt32" => 4,
                "Int16" or "UInt16" => 2,
                "Byte" => 1,
                _ => 0,
            };
        }
    }
}
