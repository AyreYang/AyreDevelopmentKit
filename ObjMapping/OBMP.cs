using ObjMapping.enums;
using ObjMapping.interfaces;
using ObjMapping.tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjMapping
{
    public static class OBMP
    {
        private static Dictionary<Guid, IStrategy> strategies = new Dictionary<Guid, IStrategy>();

        public static IStrategy CreateSTRG<T1, T2>(MappingMode mode = MappingMode.All)
            where T1 : new()
            where T2 : new()
        {
            var strategy = new Strategy<T1, T2>(mode);
            strategies.Add(strategy.Key, strategy);
            return strategy;
        }

        public static T2 MapTo<T1, T2>(this T1 src, IStrategy strategy = null)
            where T1 : new()
            where T2 : new()
        {
            if (!Strategy<T1, T2>.IsValidType) return default(T2);

            var dest = new T2();
            Map<T1, T2>(src, dest, strategy);
            return dest;
        }

        public static T2 MapTo<T1, T2>(this T1 src, Guid? key, bool auto = true, bool @default = true)
            where T1 : new()
            where T2 : new()
        {
            if (!Strategy<T1, T2>.IsValidType) return default(T2);

            var dest = new T2();
            var strategy = SearchStrategy<T1, T2>(key, auto, @default);
            if (strategy != null) Map<T1, T2>(src, dest, strategy);
            return dest;
        }

        public static void Map<T1, T2>(T1 src, T2 dest, Guid? key, bool auto = true, bool @default = true)
            where T1:new()
            where T2:new()
        {
            if (src == null || dest == null) return;
            if (!Strategy<T1, T2>.IsValidType) return;

            var strategy = SearchStrategy<T1, T2>(key, auto, @default);
            if (strategy != null) Map<T1, T2>(src, dest, strategy);
        }

        public static void Map<T1, T2>(T1 src, T2 dest, IStrategy strategy = null)
            where T1 : new()
            where T2 : new()
        {
            if (src == null || dest == null) return;
            if (!Strategy<T1, T2>.IsValidType) return;

            IStrategy strtgy = (strategy as Strategy<T1, T2>) != null ? strategy : new Strategy<T1, T2>(MappingMode.All).CareAll();
            (strtgy as Strategy<T1, T2>).GetMappingList().ForEach(pair => pair.SetInstance(src, dest).Mapping());
        }

        private static Strategy<T1, T2> SearchStrategy<T1, T2>(Guid? key = null, bool auto = true, bool @default = true)
            where T1 : new()
            where T2 : new()
        {
            var strategy = key.HasValue && strategies.ContainsKey(key.Value) ? strategies[key.Value] as Strategy<T1, T2> : null;
            if(strategy == null && auto)
            {
                strategy = strategies.Values.FirstOrDefault(strtgy => (strtgy as Strategy<T1, T2>) != null) as Strategy<T1, T2>;
            }
            if (strategy == null && @default)
            {
                strategy = new Strategy<T1, T2>(MappingMode.All);
            }
            return strategy;
        }

    }
}
