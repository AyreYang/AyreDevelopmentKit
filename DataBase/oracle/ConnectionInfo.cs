using System;
using DataBase.common.interfaces;
using DataBase.common.messages;

namespace DataBase.oracle
{
    public class ConnectionInfo : IConnectionInfo
    {
        private const int defport = 1521;
        //private const string pattern = "DATA SOURCE=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = 1530)))(CONNECT_DATA = (SERVICE_NAME ={1})));PERSIST SECURITY INFO=True;USER ID={2};Password={3}";
        private const string pattern = "Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (HOST = {0}) (PROTOCOL = TCP) (PORT = {1})) ) (CONNECT_DATA = (SID = {2})));User Id={3};Password={4};";

        public string Host { get; private set; }
        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public int Port { get; private set; }

        public ConnectionInfo(string host, string database, string schema, string user, string password, int port)
        {
            if (string.IsNullOrEmpty(host)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Host"));
            if (string.IsNullOrEmpty(database)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Database"));
            if (string.IsNullOrEmpty(user)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.User"));
            if (string.IsNullOrEmpty(password)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Password"));

            Host = host.Trim();
            Port = (port > 0) ? port : defport;
            Database = database.Trim();
            Schema = (schema ?? string.Empty).Trim().ToUpper();
            User = user.Trim();
            Password = password.Trim();
        }

        public ConnectionInfo(string host, string database, string user, string password, int port) : this(host, database, null, user, password, port) { }
        public ConnectionInfo(string host, string database, string user, string password) : this(host, database, null, user, password, 0) { }
        public ConnectionInfo(string host, string database, string schema, string user, string password) : this(host, database, schema, user, password, 0) { }

        public string DBConString
        {
            get { return string.Format(pattern, Host, Port, Database, User, Password); }
        }
    }
}
