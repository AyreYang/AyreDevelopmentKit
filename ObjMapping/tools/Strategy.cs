#define DEBUG
using ObjMapping.consts;
using ObjMapping.enums;
using ObjMapping.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjMapping.tools
{

    internal class Strategy<T1, T2> : IStrategy
        where T1:new()
        where T2:new()
    {
        private Guid key { get; set; }
        public Guid Key { get { return key; } }
        public static bool IsValidType
        {
            get
            {
                return (Consts.Convert2MemberType(typeof(T1)) == MemberType.Complex && Consts.Convert2MemberType(typeof(T2)) == MemberType.Complex);
                //return (typeof(T1).IsClass && !typeof(T1).IsGenericType && !typeof(T1).IsArray) && (typeof(T2).IsClass && !typeof(T2).IsGenericType && !typeof(T2).IsArray);
            }
        }

        private MappingMode mode { get; set; }
        public MappingMode Mode { get { return mode; } }
        private FilterList FilterList { get; set; }
        private List<MappingPair<T1, T2>> MappingList { get; set; }

        protected MemberInfo<T1>[] Members1 { get; private set; }
        protected MemberInfo<T2>[] Members2 { get; private set; }

        

        public Strategy(MappingMode mode = MappingMode.All)
        {
            if (!IsValidType) throw new InvalidCastException();

            key = Guid.NewGuid();
            this.mode = mode;
            MappingList = new List<MappingPair<T1, T2>>();
            FilterList = null;

            Members1 = GetMembers<T1>();
            Members2 = GetMembers<T2>();
        }

        #region STATIC METHODS
        private MemberInfo<T>[] GetMembers<T>()
            where T:new()
        {
            var list = new List<MemberInfo<T>>();
            typeof(T).GetMembers().Where(m =>
            (Mode == MappingMode.Self ? m.DeclaringType.Equals(typeof(T)) : true)
            &&
            (m.MemberType == System.Reflection.MemberTypes.Field || m.MemberType == System.Reflection.MemberTypes.Property))
            .ToList()
            .ForEach(m => list.Add(new MemberInfo<T>(m)));
            return list.ToArray();
        }
        #endregion

        #region INTERFACE METHODS
        public IStrategy AddMap(string from, string to, Func<object, object, cnvt> converter = null)
        {
            if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var pair = MappingPair<T1, T2>.Create(
                    Members1.FirstOrDefault(m => m.Name == from.Trim()),
                    Members2.FirstOrDefault(m => m.Name == to.Trim()), converter);
                if(pair != null)
                {
                    var index = MappingList.FindIndex(m => m.ToSameEnd(pair));
                    if (index >= 0) MappingList.RemoveAt(index);
                    MappingList.Add(pair);
                }
            }
            return this;
        }

        public IStrategy ResetFilterList()
        {
            FilterList.Clear();
            FilterList = null;
            return this;
        }

        public IStrategy ResetCareList(params string[] fields)
        {
            ResetFilterList(FilterListType.Care);
            return AddCareList(fields);
        }

        public IStrategy ResetIgnoreList(params string[] fields)
        {
            ResetFilterList(FilterListType.Ignore);
            return AddIgnoreList(fields);
        }

        public IStrategy AddCareList(params string[] fields)
        {
            if (FilterList == null) ResetFilterList(FilterListType.Care);
            if (FilterList.TYPE == FilterListType.Care)
            {
                var members = GetMembersFrom2(fields);
                if (members != null && members.Length > 0) FilterList.Add(members);
            }

            return this;
        }

        public IStrategy AddIgnoreList(params string[] fields)
        {
            if (FilterList == null) ResetFilterList(FilterListType.Ignore);
            if (FilterList.TYPE == FilterListType.Ignore)
            {
                var members = GetMembersFrom2(fields);
                if (members != null && members.Length > 0) FilterList.Add(members);
            }

            return this;
        }

        public IStrategy CareAll()
        {
            return ResetCareList();
        }

        public IStrategy IgnoreAll()
        {
            return ResetIgnoreList();
        }
        #endregion

        #region PRIVATE METHODS
        private MemberInfo<T2>[] GetMembersFrom2(params string[] fields)
        {
            if (fields == null || fields.Length <= 0) return null;
            return Members2;
        }
        private void ResetFilterList(FilterListType type)
        {
            if(FilterList != null)
            {
                if (FilterList.TYPE == type)
                {
                    FilterList.Clear();
                }
                else
                {
                    ResetFilterList();
                    FilterList = new FilterList(type);
                }
            }else
            {
                FilterList = new FilterList(type);
            }
        }
        #endregion

        internal List<MappingPair<T1, T2>> GetMappingList()
        {
            var list = new List<MappingPair<T1, T2>>();

            if(FilterList != null)
            {
                switch (FilterList.TYPE)
                {
                    case FilterListType.Care:
                        foreach (var mem2 in Members2)
                        {
                            if(FilterList.IsEmpty || FilterList.Contains(mem2))
                            {
                                var mem1 = Members1.FirstOrDefault(m => m.Name == mem2.Name);
                                var pair = MappingPair<T1, T2>.Create(mem1, mem2);
                                if (pair != null) list.Add(pair.Clone());
                            }
                        }
                        break;
                    case FilterListType.Ignore:
                        if (!FilterList.IsEmpty)
                        {
                            foreach (var mem2 in Members2)
                            {
                                if (!FilterList.Contains(mem2))
                                {
                                    var mem1 = Members1.FirstOrDefault(m => m.Name == mem2.Name);
                                    var pair = MappingPair<T1, T2>.Create(mem1, mem2);
                                    if (pair != null) list.Add(pair.Clone());
                                }
                            }
                        }
                        break;
                }
            }

            //增加Mapping绑定
            if (MappingList.Count > 0)
            {
                foreach (var pair in MappingList)
                {
                    var index = list.FindIndex(itm => itm.ToSameEnd(pair));
                    if (index >= 0) list.RemoveAt(index);
                    list.Add(pair.Clone());
                }
            }

            return list;
        }

        
    }
}
