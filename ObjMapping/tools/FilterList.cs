using ObjMapping.enums;
using System.Collections.Generic;

namespace ObjMapping.tools
{
    internal class FilterList
    {
        public FilterListType TYPE { get; private set; }
        public int Count { get { return List.Count; } }
        public bool IsEmpty { get { return List.Count <= 0; } }
        private List<string> List { get; set; }
        

        public FilterList(FilterListType type)
        {
            TYPE = type;
            List = new List<string>();
        }

        public void Add<T>(params MemberInfo<T>[] members)
            where T:new()
        {
            if (members == null || members.Length <= 0) return;
            foreach(var info in members) if(!List.Contains(info.Name)) List.Add(info.Name);
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains<T>(MemberInfo<T> member)
            where T : new()
        {
            return List.Contains(member.Name);
        }

        public List<string> GetList()
        {
            var list = new List<string>();
            List.ForEach(nm => list.Add(nm));
            return list;
        }
    }
}
