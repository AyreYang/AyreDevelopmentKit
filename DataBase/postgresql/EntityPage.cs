using System.Collections.Generic;
using DataBase.common;
using DataBase.common.enums;
using System.Data.Common;
using DataBase.common.objects;

namespace DataBase.postgresql
{
    public class EntityPage<T> : TableEntityPage<T> where T : TableEntity, new()
    {
        protected override string PageNOScript
        {
            get {
                string sort = null;
                Sort.Export(out sort);
                if (!string.IsNullOrWhiteSpace(sort))
                {
                    sort = string.Format("(trunc(((row_number() OVER (ORDER BY {0})) - 1) / {1}) + 1)", sort, PageCount);
                }
                return sort;
            }
        }
        public EntityPage(Clause clause, Sort sort, int count, DatabaseAccessor accessor) : base(clause, sort, count, accessor) { }
        //public TableEntityPage(Clause clause, Sort sort, int count, DatabaseAccessor accessor)
        //public EntityPage(DatabaseAccessor accessor, int count, KeyValuePair<string, object>[] columns = null, params KeyValuePair<string, Sort>[] sorts) : base(accessor, count, columns, sorts) { }
        //public EntityPage(DatabaseAccessor accessor, int count, List<string> clauses, List<KeyValuePair<string, object>> parameters, params KeyValuePair<string, Sort>[] sorts) : base(accessor, count, clauses, parameters, sorts) { }
    }
}
