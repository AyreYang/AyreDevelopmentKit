using System;
using DataBase.common.enums;
using DataBase.common.objects;

namespace DataBase.common.attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DBFieldAttribute : Attribute
    {
        public string Name { get; private set; }
        public Type FieldType { get; private set; }
        public KeyType KeyType { get; private set; }
        public bool Nullable { get; private set; }
        public object DefaultValue { get; private set; }


        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Name);
            }
        }
        public DBFieldAttribute(string name, Type field, KeyType key, bool nullable, object def)
        {
            Name = (!string.IsNullOrWhiteSpace(name)) ? name.Trim().ToUpper() : string.Empty;
            FieldType = field;
            KeyType = key;
            Nullable = nullable;
            DefaultValue = def;
        }
        public DBFieldAttribute(string name, Type field, KeyType key, bool nullable) : this(name, field, key, nullable, null) { }
        public DBFieldAttribute(string name, Type field, KeyType key) : this(name, field, key, true, null) { }
        public DBFieldAttribute(string name, Type field) : this(name, field, KeyType.Normal, true, null) { }
        public DBFieldAttribute(string name, Type field, bool nullable, object def) : this(name, field, KeyType.Normal, nullable, def) { }
        public DBFieldAttribute(string name, Type field, object def) : this(name, field, KeyType.Normal, true, def) { }
        public DBFieldAttribute(string name, Type field, bool nullable) : this(name, field, KeyType.Normal, nullable, null) { }

        public DBColumn CreateDBColumn()
        {
            return new DBColumn(Name, FieldType, KeyType, Nullable, DefaultValue);
        }
    }
}
