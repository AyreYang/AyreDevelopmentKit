using System;
using DataBase.common.interfaces;
using DataBase.common.messages;

namespace DataBase.postgresql
{
    public class ConnectionInfo : IConnectionInfo
    {
        private const int defport = 5432;
        private const string pattern = "Server={0};Port={1};uid={2};pwd={3};Database={4};Encoding=UNICODE";

        public string Host { get; private set; }
        public string Database { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public int Port { get; private set; }

        public ConnectionInfo(string host, string database, string user, string password, int port = 0)
        {
            if (string.IsNullOrEmpty(host)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Host"));
            if (string.IsNullOrEmpty(database)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Database"));
            if (string.IsNullOrEmpty(user)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.User"));
            if (string.IsNullOrEmpty(password)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Password"));

            Host = host.Trim();
            Port = (port > 0) ? port : defport;
            Database = database.Trim();
            User = user.Trim();
            Password = password.Trim();
        }

        public string DBConString
        {
            get { return string.Format(pattern, Host, Port, User, Password, Database); }
        }
    }
}
