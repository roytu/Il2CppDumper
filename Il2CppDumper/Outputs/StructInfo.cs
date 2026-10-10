using System;
using System.Collections.Generic;

namespace Il2CppDumper
{
    public class StructInfo
    {
        public string TypeName;
        public bool IsValueType;
        public List<StructFieldInfo> Fields = new();
        public List<StructFieldInfo> StaticFields = new();
        public StructVTableMethodInfo[] VTableMethod = Array.Empty<StructVTableMethodInfo>();
        public List<StructRGCTXInfo> RGCTXs = new();
    }

    public class StructFieldInfo
    {
        public string FieldTypeName;
        public string FieldName;
        public bool IsValueType;
        public bool IsCustomType;
        public int ArrayLength;
        public bool IsUnion;
        public string Comment;
        public List<StructFieldInfo> Members; // anonymous struct or union instead of a plain field
    }

    public class FieldLayoutItem
    {
        public Il2CppFieldDefinition Definition;
        public Il2CppType Type;
        public Il2CppGenericContext Context;
        public int Offset; // relative to the first instance field, -1 when unknown
        public int Index;
        public bool FirstOfType;
        public int Packing; // StructLayout packing to apply when the offset is unknown, 0 for none
    }

    public class StructVTableMethodInfo
    {
        public string MethodName;
    }

    public class StructRGCTXInfo
    {
        public Il2CppRGCTXDataType Type;
        public string TypeName;
        public string ClassName;
        public string MethodName;
    }
}
