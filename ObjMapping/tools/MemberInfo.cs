using ObjMapping.Enums;
using System;
using System.Linq;

namespace ObjMapping.Tools
{
    internal class MemberInfo
    {
        public string Name { get { return info.Name; } }
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
                return Consts.Consts.Convert2MemberType(type);
            }
        }

        private System.Reflection.MemberInfo info { get; set; }

        public object Inst { get; private set; }



        public static MemberInfo Create(string name, object obj)
        {
            MemberInfo info = null;
            var type = obj.GetType();
            var members = type.GetMember(name);
            if (members != null && members.Length > 0)
            {
                var member = members.First();
                switch (member.MemberType)
                {
                    case System.Reflection.MemberTypes.Field:
                        member = type.GetField(name);
                        break;
                    case System.Reflection.MemberTypes.Property:
                        member = type.GetProperty(name);
                        break;
                    default:
                        throw new InvalidCastException();
                }
                info = new MemberInfo(member, obj);
            }
            return info;
        }

        private MemberInfo(System.Reflection.MemberInfo info, object obj)
        {
            this.info = info;
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
    }
}
