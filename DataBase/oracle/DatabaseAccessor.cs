﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DataBase.common;
using DataBase.common.objects;
using Oracle.ManagedDataAccess.Client;

namespace DataBase.oracle
{
    sealed public class DatabaseAccessor : DatabaseCore
    {
        private const string PREFIX_PARM = ":";
        public DatabaseAccessor(string host, string database, string user, string password, int port = 0)
        {
            mcnt_info = new ConnectionInfo(host, database, user, password, port);
            msc_con = new OracleConnection(mcnt_info.DBConString);
        }

        public override bool TableExists(string as_table)
        {
            if (string.IsNullOrWhiteSpace(as_table)) return false;

            //var user = ((ConnectionInfo)mcnt_info).User.Trim();
            var count = CountInTable("ALL_TABLES", new Clause("UPPER(TABLE_NAME) = {name}").AddParam("name", as_table.Trim()));
            return count > 0;
        }


        public override DbParameter CreateParameter(DBColumn column)
        {
            if (column == null) return null;
            return new OracleParameter(PREFIX_PARM + column.ID, column.Value);
        }
        public override DbParameter CreateParameter(string name, object value)
        {
            return new OracleParameter(PREFIX_PARM + name.Trim().ToUpper(), value);
        }
        public override DbCommand CreateCommand(string sql, List<DbParameter> parameters)
        {
            var command = new OracleCommand(sql);
            if (parameters != null && parameters.Count > 0) parameters.ForEach(param => command.Parameters.Add(param));
            return command;
        }

        public override long GenerateSequence(string sequence)
        {
            var sql = (!string.IsNullOrWhiteSpace(sequence)) ? string.Format("SELECT {0}.nextval FROM DUAL", sequence.Trim().ToUpper()) : string.Empty;
            return this.Sequence(sql);
        }
    }
}
