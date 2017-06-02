using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjMapping.Tools
{
    internal class StrategyContainer
    {
        private static object locker = new object();
        private static StrategyContainer inst { get; set; }
        public static StrategyContainer Inst {
            get
            {
                if(inst == null)
                {
                    lock (locker)
                    {
                        if(inst == null) inst = new StrategyContainer();
                    }
                }
                return inst;
            }
        }

        public int Count
        {
            get
            {
                return Strategies.Count;
            }
        }

        private Dictionary<Guid, Strategy> Strategies { get; set; }
        private Dictionary<TypePair, Guid> Keys { get; set; }
        private StrategyContainer()
        {
            Strategies = new Dictionary<Guid, Strategy>();
            Keys = new Dictionary<TypePair, Guid>();
        }

        public void Add<T1, T2>(Strategy strategy)
        {
            if (strategy == null) return;
            lock (locker)
            {
                var pair = GetPairKey<T1, T2>();
                if(pair != null)
                {
                    var kkey = pair.Value.Key;
                    var vkey = pair.Value.Value;
                    Strategies.Remove(vkey);
                    Keys.Remove(kkey);
                }
                Strategies.Add(strategy.Key, strategy);
                Keys.Add(TypePair.Create<T1, T2>(), strategy.Key);
            }
        }
        public void Add(Strategy strategy)
        {
            if (strategy == null) return;
            lock (locker)
            {
                if (Strategies.ContainsKey(strategy.Key)) Strategies.Remove(strategy.Key);
                Strategies.Add(strategy.Key, strategy);
            }
        }
        public Strategy Get<T1, T2>()
        {
            var key = GetKey<T1, T2>();
            return key.HasValue ? Strategies[key.Value].Clone() : null;
        }
        public Strategy Get(Guid key)
        {
            return Strategies.ContainsKey(key) ? Strategies[key].Clone() : null;
        }
        public Strategy Get(Type T1, Type T2)
        {
            var key = GetKey(T1, T2);
            return key.HasValue ? Strategies[key.Value].Clone() : null;
        }
        public bool Contains<T1, T2>()
        {
            var temp = TypePair.Create<T1, T2>();
            return Keys.Any(pair => pair.Key.Equals(temp));
        }
        public bool Contains(Guid key)
        {
            return Strategies.ContainsKey(key);
        }
        private Guid? GetKey<T1, T2>()
        {
            var kvpair = GetPairKey<T1, T2>();
            return kvpair.HasValue ? (Guid?)(kvpair.Value.Value) : null;
        }
        private Guid? GetKey(Type T1, Type T2)
        {
            var kvpair = GetPairKey(T1, T2);
            return kvpair.HasValue ? (Guid?)(kvpair.Value.Value) : null;
        }
        private KeyValuePair<TypePair, Guid>? GetPairKey<T1, T2>()
        {
            var temp = TypePair.Create<T1, T2>();
            KeyValuePair<TypePair, Guid>? kvpair = null;
            if (Keys.Any(pair => pair.Key.Equals(temp)))
            {
                kvpair = Keys.First(pair => pair.Key.Equals(temp));
            }
            return kvpair;
        }
        private KeyValuePair<TypePair, Guid>? GetPairKey(Type T1, Type T2)
        {
            var temp = TypePair.Create(T1, T2);
            KeyValuePair<TypePair, Guid>? kvpair = null;
            if (temp != null && Keys.Any(pair => pair.Key.Equals(temp)))
            {
                kvpair = Keys.First(pair => pair.Key.Equals(temp));
            }
            return kvpair;
        }
    }
}
