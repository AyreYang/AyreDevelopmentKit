using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using DataBase.common;
using DataBase.common.messages;
using DataBase.common.objects;

namespace DataBase.mssqlserver
{
    sealed public class DatabaseAccessor : DatabaseCore
    {
        private const string PREFIX_PARM = "@";

        public DatabaseAccessor(string host, string database, string user, string password)
        {
            mcnt_info = new ConnectionInfo(host, database, user, password);
            msc_con = new SqlConnection(mcnt_info.DBConString);
        }

        public override bool TableExists(string as_table)
        {
            if (string.IsNullOrWhiteSpace(as_table)) return false;
            var count = CountInTable("SysObjects", new Clause("type = 'U' AND name = {name}").AddParam("name", as_table.Trim()));
            return count > 0;
        }

        public override DbParameter CreateParameter(DBColumn column)
        {
            if (column == null) return null;
            return new SqlParameter(PREFIX_PARM + column.ID, column.Value);
        }
        public override DbParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(PREFIX_PARM + name.Trim().ToUpper(), value);
        }
        public override DbCommand CreateCommand(string sql, List<DbParameter> parameters)
        {
            var command = new SqlCommand(sql);
            if (parameters != null && parameters.Count > 0) parameters.ForEach(param => command.Parameters.Add(param));
            return command;
        }

        public override long GenerateSequence(string sequence)
        {
            var sql = string.Empty;
            return this.Sequence(sql);
        }
    }
}
