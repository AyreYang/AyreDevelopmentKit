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
        private FilterList(FilterList list)
        {
            this.TYPE = list.TYPE;
            this.List = new List<string>();
            list.List.ForEach(item => this.List.Add(item));
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

        public FilterList Clone()
        {
            return new FilterList(this);
        }
    }
}
