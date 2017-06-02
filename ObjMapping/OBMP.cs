using ObjMapping.Enums;
using ObjMapping.Interfaces;
using ObjMapping.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjMapping
{
    public static class OBMP
    {
        private static Dictionary<Guid, IStrategy> strategies = new Dictionary<Guid, IStrategy>();

        public static IStrategy CreateSTRG(MappingMode mode = MappingMode.All)
        {
            var strategy = new Strategy(mode);
            strategies.Add(strategy.Key, strategy);
            return strategy;
        }

        public static T MapTo<T>(this object src, IStrategy strategy = null)
            where T : new()
        {
            var dest = new T();
            Map(src, dest, strategy);
            return dest;
        }

        public static T MapTo<T>(this object src, Guid? key, bool auto = true, bool @default = true)
            where T : new()
        {
            var dest = new T();
            var strategy = SearchStrategy(key, auto, @default);
            if (strategy != null) Map(src, dest, strategy);
            return dest;
        }

        public static void Map(object src, object dest, Guid? key, bool auto = true, bool @default = true)
        {
            if (src == null || dest == null) return;

            var strategy = SearchStrategy(key, auto, @default);
            if (strategy != null) Map(src, dest, strategy);
        }

        public static void Map(object src, object dest, IStrategy strategy = null)
        {
            if (src == null || dest == null) return;

            IStrategy strtgy = (strategy as Strategy) != null ? strategy : new Strategy(MappingMode.All).CareAll();
            (strtgy as Strategy).Mapping(src, dest);
        }

        private static Strategy SearchStrategy(Guid? key = null, bool auto = true, bool @default = true)
        {
            var strategy = key.HasValue && strategies.ContainsKey(key.Value) ? strategies[key.Value] as Strategy : null;
            if(strategy == null && auto)
            {
                strategy = strategies.Values.FirstOrDefault(strtgy => (strtgy as Strategy) != null) as Strategy;
            }
            if (strategy == null && @default)
            {
                strategy = new Strategy(MappingMode.All);
            }
            return strategy;
        }

    }
}
