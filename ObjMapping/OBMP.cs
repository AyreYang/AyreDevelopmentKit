using ObjMapping.Enums;
using ObjMapping.Interfaces;
using ObjMapping.Tools;
using System;

namespace ObjMapping
{
    public static class OBMP
    {
        public static Guid K_MODE_ALL_CAREALL = CreateSTRG(MappingMode.All).CareAll().Key;
        public static Guid K_MODE_SELF_CAREALL = CreateSTRG(MappingMode.Self).CareAll().Key;

        public static IStrategy CreateSTRG<T1, T2>(MappingMode mode = MappingMode.All)
        {
            var strategy = new Strategy(mode);
            StrategyContainer.Inst.Add<T1, T2>(strategy);
            return strategy;
        }
        public static IStrategy CreateSTRG(MappingMode mode = MappingMode.All)
        {
            var strategy = new Strategy(mode);
            StrategyContainer.Inst.Add(strategy);
            return strategy;
        }

        public static T MapTo<T>(this object src, Guid? key = null, bool auto = true, bool @default = true)
            where T : new()
        {
            var dest = default(T);
            var strategy = key.HasValue ? 
                StrategyContainer.Inst.Get(key.Value) : 
                (auto ? StrategyContainer.Inst.Get(src.GetType(), typeof(T)) :
                    @default ? StrategyContainer.Inst.Get(K_MODE_ALL_CAREALL) : null);
            if (strategy != null) Map(src, (dest = new T()), strategy);
            return dest;
        }

        public static void Map(object src, object dest, Guid? key = null, bool auto = true, bool @default = true)
        {
            if (src == null || dest == null) return;
            var strategy = key.HasValue ?
                StrategyContainer.Inst.Get(key.Value) :
                (auto ? StrategyContainer.Inst.Get(src.GetType(), dest.GetType()) : null);
            Map(src, dest, strategy != null ? strategy : (@default ? StrategyContainer.Inst.Get(K_MODE_ALL_CAREALL) : null));
        }

        public static void Map<T1, T2>(object src, object dest, bool @default = true)
        {
            if (src == null || dest == null) return;
            var strategy = StrategyContainer.Inst.Get<T1, T2>();
            Map(src, dest, strategy != null ? strategy : (@default ? StrategyContainer.Inst.Get(K_MODE_ALL_CAREALL) : null));
        }

        private static void Map(object src, object dest, IStrategy strategy)
        {
            var strtgy = (strategy as Strategy);
            if (src == null || dest == null || strtgy == null) return;
            strtgy.Mapping(src, dest);
        }

    }
}
