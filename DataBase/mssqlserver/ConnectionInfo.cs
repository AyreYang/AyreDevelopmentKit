using System;
using DataBase.common.interfaces;
using DataBase.common.messages;

namespace DataBase.mssqlserver
{
    public class ConnectionInfo : IConnectionInfo
    {
        private const string pattern = "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};";

        public string Host { get; private set; }
        public string Database { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }

        public ConnectionInfo(string host, string database, string user, string password)
        {
            if (string.IsNullOrEmpty(host)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Host"));
            if (string.IsNullOrEmpty(database)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Database"));
            if (string.IsNullOrEmpty(user)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.User"));
            if (string.IsNullOrEmpty(password)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Password"));

            Host = host.Trim();
            Database = database.Trim();
            User = user.Trim();
            Password = password.Trim();
        }

        #region IConnectionInfo Members

        public string DBConString
        {
            get { return string.Format(pattern, Host, Database, User, Password); }
        }

        #endregion
    }
}
