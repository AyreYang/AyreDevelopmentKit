using ObjMapping.enums;
using ObjMapping.interfaces;
using System;

namespace ObjMapping.tools
{
    internal class MappingPair<T1, T2>
        where T1 : new()
        where T2 : new()
    {
        public MemberInfo<T1> Member1 { get; private set; }
        //public IMemberInfo MemberInfo1 { get { return Member1; } }

        public MemberInfo<T2> Member2 { get; private set; }
        //public IMemberInfo MemberInfo2 { get { return Member2; } }

        public Func<object, object, cnvt> FConverter { get; private set; }

        public static MappingPair<T1, T2> Create(MemberInfo<T1> mem1, MemberInfo<T2> mem2, Func<object, object, cnvt> converter = null)
        {
            return ((mem1 != null) && (mem2 != null)) ? new MappingPair<T1, T2>(mem1, mem2, converter) : null;
        }
        private MappingPair(MemberInfo<T1> mem1, MemberInfo<T2> mem2, Func<object, object, cnvt> converter = null)
        {
            if (mem1 == null || mem2 == null) throw new NullReferenceException();

            Member1 = mem1;
            Member2 = mem2;
            FConverter = converter;
        }

        public bool ToSameEnd(MappingPair<T1, T2> pair)
        {
            return pair == null ? false : this.Member2.Equals(pair.Member2);
        }
        public void Mapping()
        {
            try
            {
                var cnvtr = FConverter != null ?
                FConverter(Member1.Value, Member2.Value)
                : ((Member1.MemberType == MemberType.Simple && Member2.MemberType == MemberType.Simple) ?
                    new cnvt(true, Member1.Value) : new cnvt(false, null));
                if (cnvtr.Mapping) Member2.SetValue(cnvtr.Value);
            }
            catch { }
        }

        public MappingPair<T1, T2> Clone()
        {
            return new MappingPair<T1, T2>(Member1.Clone(), Member2.Clone(), FConverter);
        }

        public MappingPair<T1, T2> SetInstance(T1 obj1, T2 obj2)
        {
            Member1.Reset(obj1);
            Member2.Reset(obj2);
            return this;
        }
    }
}
