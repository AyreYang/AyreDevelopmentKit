#define DEBUG
using ObjMapping.Enums;
using ObjMapping.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjMapping.Tools
{
    internal class Strategy : IStrategy
    {
        private Guid key { get; set; }
        public Guid Key { get { return key; } }


        private MappingMode mode { get; set; }
        public MappingMode Mode { get { return mode; } }
        private FilterList FilterList { get; set; }
        private List<Pair> MappingList { get; set; }


        public Strategy(MappingMode mode = MappingMode.All)
        {
            this.key = Guid.NewGuid();
            this.mode = mode;
            MappingList = new List<Pair>();
            FilterList = null;
        }

        private Strategy(Strategy strategy)
        {
            this.key = strategy.Key;
            this.mode = strategy.Mode;
            this.FilterList = null;
            this.MappingList = new List<Pair>();

            if (strategy.FilterList != null) this.FilterList = strategy.FilterList.Clone();
            strategy.MappingList.ForEach(pair => this.MappingList.Add(pair.Clone()));
        }

        #region INTERFACE METHODS
        public IStrategy AddMap(string from, string to, Func<IContext, Result> converter = null)
        {
            if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var pair = Pair.Create(from, to, converter);
                if (pair != null)
                {
                    //var index = MappingList.FindIndex(m => m.Member2.Equals(pair.Member2));
                    //if (index >= 0) MappingList.RemoveAt(index);
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
                if (fields != null && fields.Length > 0) FilterList.Add(fields);
            }

            return this;
        }

        public IStrategy AddIgnoreList(params string[] fields)
        {
            if (FilterList == null) ResetFilterList(FilterListType.Ignore);
            if (FilterList.TYPE == FilterListType.Ignore)
            {

                if (fields != null && fields.Length > 0) FilterList.Add(fields);
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
        private void ResetFilterList(FilterListType type)
        {
            if (FilterList != null)
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
            }
            else
            {
                FilterList = new FilterList(type);
            }
        }
        #endregion



        public void Mapping(object obj1, object obj2)

        {
            GetMappingList(obj1, obj2).ForEach(pair => pair.Mapping(obj1, obj2));
        }
        public Strategy Clone()
        {
            return new Strategy(this);
        }

        private List<Pair> GetMappingList(object obj1, object obj2)
        {

            var list = new List<Pair>();



            var members1 = obj1.GetType().GetMembers().Where(m => m.MemberType == System.Reflection.MemberTypes.Field || m.MemberType == System.Reflection.MemberTypes.Property).ToArray();

            var members2 = obj2.GetType().GetMembers().Where(m => m.MemberType == System.Reflection.MemberTypes.Field || m.MemberType == System.Reflection.MemberTypes.Property).ToArray();

            if (FilterList != null)
            {
                switch (FilterList.TYPE)
                {
                    case FilterListType.Care:
                        foreach (var mem2 in members2)
                        {
                            if (FilterList.IsEmpty || FilterList.Contains(mem2.Name))
                            {
                                var mem1 = members1.FirstOrDefault(m => m.Name == mem2.Name);

                                if (mem1 != null)

                                {

                                    var pair = Pair.Create(mem1.Name, mem2.Name);

                                    if (pair != null) list.Add(pair);

                                }
                            }
                        }
                        break;
                    case FilterListType.Ignore:
                        if (!FilterList.IsEmpty)
                        {
                            foreach (var mem2 in members2)
                            {
                                if (!FilterList.Contains(mem2.Name))
                                {
                                    var mem1 = members1.FirstOrDefault(m => m.Name == mem2.Name);

                                    if (mem1 != null)

                                    {

                                        var pair = Pair.Create(mem1.Name, mem2.Name);

                                        if (pair != null) list.Add(pair);

                                    }
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
                    var index = list.FindIndex(itm => itm.Member2.Equals(pair.Member2));
                    if (index >= 0) list.RemoveAt(index);
                    list.Add(pair);
                }
            }

            return list;
        }
    }
}
