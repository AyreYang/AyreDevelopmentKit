using ObjMapping.Interfaces;
using System;

namespace ObjMapping.Tools
{
    internal class Context : IContext
    {
        private MemberInfo info1 { get; set; }
        private MemberInfo info2 { get; set; }

        public object Value1
        {
            get
            {
                return info1.Value;
            }
        }

        public object Value2
        {
            get
            {
                return info2.Value;
            }
        }

        public Context(MemberInfo info1, MemberInfo info2)
        {
            if (info1 == null || info2 == null) throw new NullReferenceException();
            this.info1 = info1;
            this.info2 = info2;
        }


        public T GetObject1<T>()
        {
            return (info1.Inst is T) ? (T)info1.Inst : default(T);
        }

        public T GetObject2<T>()
        {
            return (info2.Inst is T) ? (T)info2.Inst : default(T);
        }
    }
}
