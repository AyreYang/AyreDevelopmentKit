using System;

namespace ObjMapping.Tools
{
    internal class TypePair
    {
        public Type Type1 { get; private set; }
        public Type Type2 { get; private set; }

        private TypePair(Type T1, Type T2)
        {
            Type1 = T1;
            Type2 = T2;
        }

        public static TypePair Create<T1, T2>()
        {
            return new TypePair(typeof(T1), typeof(T2));
        }
        public static TypePair Create(Type T1, Type T2)
        {
            return (T1 != null && T2 != null) ? new TypePair(T1, T2) : null;
        }

        public override bool Equals(object obj)
        {
            var pair = obj as TypePair;
            return pair != null && pair.Type1.Equals(this.Type1) && pair.Type2.Equals(this.Type2);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
