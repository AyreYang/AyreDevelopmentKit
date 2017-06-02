using ObjMapping.Enums;
using ObjMapping.Interfaces;
using System;

namespace ObjMapping.Tools
{
    internal class Pair
    {
        public string Member1 { get; private set; }
        public string Member2 { get; private set; }
        public Func<IContext, Result> FConverter { get; private set; }

        public static Pair Create(string mem1, string mem2, Func<IContext, Result> converter = null)
        {
            return (string.IsNullOrEmpty(mem1) || string.IsNullOrEmpty(mem2)) ? null : new Pair(mem1, mem2, converter);
        }
        private Pair(string mem1, string mem2, Func<IContext, Result> converter = null)
        {
            if (string.IsNullOrEmpty(mem1) || string.IsNullOrEmpty(mem2)) throw new NullReferenceException();

            Member1 = mem1;
            Member2 = mem2;
            FConverter = converter;
        }

        public void Mapping(object obj1, object obj2)
        {
            try
            {
                var mi1 = MemberInfo.Create(Member1, obj1);
                var mi2 = MemberInfo.Create(Member2, obj2);
                if (mi1 != null && mi2 != null)
                {
                    var cnvtr = FConverter != null ?
                                FConverter(new Context(mi1, mi2))
                                : ((mi1.MemberType == MemberType.Simple && mi2.MemberType == MemberType.Simple) ?
                                new Result() { Mapping = true, Value = mi1.Value } : new Result() { Mapping = false, Value = null });
                    if (cnvtr.Mapping) mi2.SetValue(cnvtr.Value);
                }
            }
            catch { }
        }
    }
}
