using ObjMapping.consts;
using ObjMapping.enums;
using System;
using System.Linq;

namespace ObjMapping.tools
{
    internal class MemberInfo<T>
        where T:new()
    {
        public string Name { get { return info.Name; } }
        public Type TYPE { get { return typeof(T); } }
        //public System.Reflection.MemberTypes MemberType { get { return info.MemberType; } }
        public object Value
        {
            get
            {
                return Inst != null ? GetValue() : null;
            }
        }
        public MemberType MemberType
        {
            get
            {
                Type type = null;
                switch (info.MemberType)
                {
                    case System.Reflection.MemberTypes.Field:
                        var field = (System.Reflection.FieldInfo)this.info;
                        type = field.FieldType;
                        break;
                    case System.Reflection.MemberTypes.Property:
                        var property = (System.Reflection.PropertyInfo)this.info;
                        type = property.PropertyType;
                        break;
                }
                return Consts.Convert2MemberType(type);
            }
        }

        private System.Reflection.MemberInfo info { get; set; }
        private T Inst { get; set; }

        

        public MemberInfo(System.Reflection.MemberInfo info, T obj = default(T))
        {
            if (info == null) throw new NullReferenceException();
            //if(!TYPE.Equals(info.DeclaringType)) throw new InvalidCastException();
            switch (info.MemberType)
            {
                case System.Reflection.MemberTypes.Field:
                    this.info = TYPE.GetField(info.Name);
                    break;
                case System.Reflection.MemberTypes.Property:
                    this.info = TYPE.GetProperty(info.Name);
                    break;
                default:
                    throw new InvalidCastException();
            }
            if (this.info == null) throw new NullReferenceException();

            Inst = obj;
        }

        internal void Reset(T obj = default(T))
        {
            Inst = obj;
        }
        public void SetValue(object value)
        {
            if (Inst == null) throw new NullReferenceException();
            switch (info.MemberType)
            {
                case System.Reflection.MemberTypes.Field:
                    ((System.Reflection.FieldInfo)this.info).SetValue(Inst, value);
                    break;
                case System.Reflection.MemberTypes.Property:
                    ((System.Reflection.PropertyInfo)this.info).SetValue(Inst, value);
                    break;
            }
        }
        public object GetValue()
        {
            if (Inst == null) throw new NullReferenceException();
            object value = null;
            switch (info.MemberType)
            {
                case System.Reflection.MemberTypes.Field:
                    value = ((System.Reflection.FieldInfo)this.info).GetValue(Inst);
                    break;
                case System.Reflection.MemberTypes.Property:
                    value = ((System.Reflection.PropertyInfo)this.info).GetValue(Inst);
                    break;
            }
            return value;
        }

        public override bool Equals(object obj)
        {
            var info = obj as MemberInfo<T>;

            return info != null ? Name.Equals(info.Name) : false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public MemberInfo<T> Clone()
        {
            return new MemberInfo<T>(this.info);
        }
    }
}
