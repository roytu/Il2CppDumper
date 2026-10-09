using System;

namespace Il2CppDumper
{
    /// <summary>
    /// Kinds of metadata indices whose serialized width (1, 2 or 4 bytes) depends on the
    /// number of elements in the table they index. Keep in sync with
    /// vm/MetadataDeserialization.h (SerializedIndexSizes) in libil2cpp.
    /// </summary>
    public enum IndexKind
    {
        Type,
        GenericParameter,
        GenericContainer,
        TypeDefinition,
        Parameter,
        Interfaces,
        Event,
        Property,
        NestedType,
        Field,
        DefaultValueData,
        Method,
        Count
    }

    /// <summary>
    /// Marks an int field as a variable-width metadata index. The width is taken from
    /// <see cref="BinaryStream.IndexSizes"/>; an all-ones value is decoded as -1.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    class IndexAttribute : Attribute
    {
        public IndexKind Kind { get; }

        public IndexAttribute(IndexKind kind)
        {
            Kind = kind;
        }
    }
}
