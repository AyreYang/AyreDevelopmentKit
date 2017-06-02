using ObjMapping.Enums;
using System.Collections.Generic;

namespace ObjMapping.Tools
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

        public void Add(params string[] members)
        {
            if (members == null || members.Length <= 0) return;
            foreach (var member in members) if (!List.Contains(member)) List.Add(member);
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(string member)
        {
            return List.Contains(member);
        }

        public List<string> GetList()
        {
            var list = new List<string>();
            List.ForEach(nm => list.Add(nm));
            return list;
        }
    }
}
