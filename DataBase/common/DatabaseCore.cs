using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using DataBase.common.interfaces;
using DataBase.common.objects;

namespace DataBase.common
{
    public abstract class DatabaseCore : IDatabase
    {
        private string ms_error = string.Empty;
        private volatile object m_lock = new object();
        private List<DbCommand> mc_commands = new List<DbCommand>();

        protected IConnectionInfo mcnt_info = null;
        protected DbConnection msc_con = null;

        #region Abstract Methods
        public abstract DbParameter CreateParameter(DBColumn column);
        public abstract DbParameter CreateParameter(string name, object value);
        public abstract DbCommand CreateCommand(string sql, List<DbParameter> parameters = null);
        public abstract bool TableExists(string as_table);
        public abstract long GenerateSequence(string sequence);
        #endregion

        #region Static Methods
        public static T Convert2<T>(object val, T def = default(T))
        {
            var result = false;
            return Convert2<T>(val, out result, def);
        }
        public static T Convert2<T>(object val, out bool result, T def = default(T))
        {
            result = false;
            if (val == null)
            {
                result = typeof(T).Name.ToUpper().StartsWith("NULLABLE") || typeof(T).IsClass;
                return result ? default(T) : def;
            }
            else
            {
                object value = null;
                try
                {
                    if (val.GetType() == typeof(T))
                    {
                        value = val;
                    }else if (typeof(T).IsEnum)
                    {
                        value = Convert.ToInt32(val);
                        value = (result = (Enum.IsDefined(typeof(T), value))) ? (T)value : default(T);
                    }
                    else
                    {
                        switch (typeof(T).ToString())
                        {
                            case "System.Boolean":
                                value = Convert.ToBoolean(val);
                                break;
                            case "System.Char":
                                value = Convert.ToChar(val);
                                break;
                            case "System.DateTime":
                                value = Convert.ToDateTime(val);
                                break;
                            case "System.Int16":
                                value = Convert.ToInt16(val);
                                break;
                            case "System.Int32":
                                value = Convert.ToInt32(val);
                                break;
                            case "System.Int64":
                                value = Convert.ToInt64(val);
                                break;
                            case "System.Decimal":
                                value = Convert.ToDecimal(val);
                                break;
                            case "System.String":
                                value = Convert.ToString(val);
                                break;
                            case "System.Object":
                                value = val;
                                break;
                            default:
                                value = (T)val;
                                break;
                        }
                    }
                }
                catch { }

                result = (value != null);
                return result ? (T)value : def;
            }
            
        }
        public static T GetValueFormDataRow<T>(DataRow row, string column, T def = default(T))
        {
            if (row == null || string.IsNullOrWhiteSpace(column)) return def;
            return Convert2<T>(row[column.Trim()], def);
        }
        #endregion

        #region Member Methods
        public string LastError
        {
            get { return ms_error; }
        }
        protected string WriteError(string as_method, string as_error)
        {
            string ls_id = this.GetType().ToString();

            ms_error = string.Format("{0}:{1}:{2}", ls_id, as_method.Trim(), as_error);
            return ms_error;
        }
        protected void ClearError()
        {
            ms_error = string.Empty;
        }
        protected long Sequence(string sql)
        {
            long def = -1;
            if (string.IsNullOrWhiteSpace(sql)) return def;
            return this.RetrieveValue<long>(CreateCommand(sql), def);
        }
        #endregion

        #region IDatabase Members
        public void SetDBAccessor2(TableEntity entity)
        {
            if (entity != null) entity.SetDBAccessor(this);
        }
        public DataTable Retrieve(DbCommand command)
        {
            ClearError();

            if (command == null) return null;

            DataTable ldt_result = null;
            string ls_query = string.Empty;
            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();

                using (command)
                {
                    ls_query = command.CommandText;
                    if (string.IsNullOrEmpty(ls_query)) return null;
                    ls_query = ls_query.Trim();
                    if (!ls_query.ToUpper().StartsWith("SELECT")) return null;

                    command.Connection = msc_con;
                    using (DbDataReader lsdr_reader = command.ExecuteReader())
                    {
                        ldt_result = new DataTable();
                        ldt_result.Load(lsdr_reader);
                    }
                }
                return ldt_result;
            }
            catch (System.Exception lex_err)
            {
                WriteError("Retrieve", lex_err.Message);
                return null;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }
        public List<T> Retrieve<T>(DbCommand command, bool ignoreCase = true) where T : new()
        {
            List<T> result = new List<T>();
            DataTable data = null;
            if (command != null && (data = Retrieve(command)) != null && data.Rows.Count > 0)
            {
                var dict = new Dictionary<string, System.Reflection.PropertyInfo>();
                var properties = (typeof(T)).GetProperties();
                foreach (DataColumn col in data.Columns)
                {
                    var name = ignoreCase ? col.ColumnName.Trim().ToUpper() : col.ColumnName;
                    var info = ignoreCase ? properties.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals(name)) : properties.FirstOrDefault(p => p.Name.Equals(name));
                    if (info != null) dict.Add(col.ColumnName, info);
                }
                if (dict.Count > 0)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        var ent = new T();
                        dict.Keys.ToList().ForEach(key =>
                        {
                            dict[key].SetValue(ent, row[key], null);
                        });
                        result.Add(ent);
                    }
                }
            }
            return result;
        }
        public List<T> RetrieveEntity<T>(DbCommand command) where T : TableEntity, new()
        {
            return RetrieveEntity<T>(command, true);
        }
        public List<T> RetrieveEntity<T>() where T : TableEntity, new()
        {
            return RetrieveEntity<T>(null, null);
        }
        public List<T> RetrieveEntity<T>(Clause clause) where T : TableEntity, new()
        {
            return RetrieveEntity<T>(clause, null);
        }
        public List<T> RetrieveEntity<T>(Sort sort) where T : TableEntity, new()
        {
            return RetrieveEntity<T>(null, sort);
        }
        public List<T> RetrieveEntity<T>(Clause clause, Sort sort) where T : TableEntity, new()
        {
            var sql = new StringBuilder();
            var entity = new T();
            sql.AppendLine(entity.SQLTableSelect);

            List<DbParameter> parameters = null;
            string txtClause = null;
            if (clause != null) clause.Export(this, out txtClause, out parameters);
            if (!string.IsNullOrWhiteSpace(txtClause)) sql.AppendLine(" WHERE " + txtClause);

            string txtSort = null;
            if (sort != null) sort.Export(out txtSort);
            if (!string.IsNullOrWhiteSpace(txtSort)) sql.AppendLine(" ORDER BY " + txtSort);

            var command = CreateCommand(sql.ToString(), parameters);
            return RetrieveEntity<T>(command, false);
        }
        public T RetrieveValue<T>(DbCommand command, T def = default(T))
            //where T : struct
        {
            ClearError();

            if (command == null) return def;

            try
            {
                var value = def;
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                using (command)
                {
                    var ls_query = command.CommandText;
                    if (string.IsNullOrEmpty(ls_query)) return def;
                    ls_query = ls_query.Trim();
                    if (!ls_query.ToUpper().StartsWith("SELECT")) return def;

                    command.Connection = msc_con;
                    value = Convert2<T>(command.ExecuteScalar());
                }
                return value;
            }
            catch (Exception lex_err)
            {
                WriteError("RetrieveValue", lex_err.Message);
                return def;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }
        public long CountInTable(string table, Clause clause = null)
        {
            ClearError();
            if (string.IsNullOrWhiteSpace(table)) return -1;
            var sql = new StringBuilder();
            sql.AppendLine(string.Format("SELECT COUNT(1) FROM {0}", table.Trim().ToUpper()));

            List<DbParameter> parameters = null;
            string txtClause = null;
            if (clause != null) clause.Export(this, out txtClause, out parameters);
            if (!string.IsNullOrWhiteSpace(txtClause)) sql.AppendLine(" WHERE " + txtClause);

            return RetrieveValue<long>(CreateCommand(sql.ToString(), parameters));
        }
        public long ExecuteSQLCommand(DbCommand command)
        {
            ClearError();

            if (command == null) return 0;
            long ll_ret = 0;
            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();

                using (DbTransaction lst_trans = msc_con.BeginTransaction())
                {
                    try
                    {
                        using (command)
                        {
                            command.Connection = msc_con;
                            command.Transaction = lst_trans;
                            ll_ret = command.ExecuteNonQuery();
                        }

                        lst_trans.Commit();
                    }
                    catch (Exception err)
                    {
                        WriteError("ExecuteSQLCommand", err.Message);
                        if (lst_trans != null)
                        {
                            lst_trans.Rollback();
                        }
                        return -1;
                    }
                }

                return ll_ret <= 0 ? 0 : ll_ret;

            }
            catch (System.Exception lex_err)
            {
                WriteError("ExecuteSQLCommand", lex_err.Message);
                return -1;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }
        public long ExecuteSQLCommand(List<DbCommand> commands)
        {
            ClearError();

            if (commands == null || commands.Count == 0) return 0;
            long ll_ret = 0;

            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                using (DbTransaction lst_trans = msc_con.BeginTransaction())
                {
                    try
                    {
                        foreach (DbCommand command in commands)
                        {
                            if (command == null) throw new System.Exception("Command object is invalidate.");
                            using (command)
                            {
                                command.Connection = msc_con;
                                command.Transaction = lst_trans;
                                var v = command.ExecuteNonQuery();
                                if (v > 0) ll_ret++;
                            }
                        }

                        lst_trans.Commit();
                    }
                    catch (Exception err)
                    {
                        WriteError("ExecuteSQLCommand", err.Message);

                        if (lst_trans != null)
                        {
                            lst_trans.Rollback();
                        }
                        return -1;
                    }
                }

                return ll_ret;

            }
            catch (System.Exception lecp_err)
            {
                WriteError("ExecuteSQLCommand", lecp_err.Message);
                return -1;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }


        public void InsertEntity(params TableEntity[] list)
        {
            if (list == null || list.Length <= 0) return;
            foreach (TableEntity ent in list) if (ent != null) AddCommands(ent.GetInsertCommands(this));
        }
        public void UpdateEntity(params TableEntity[] list)
        {
            if (list == null || list.Length <= 0) return;
            foreach (TableEntity ent in list) if (ent != null) AddCommands(ent.GetUpdateCommands(this));
        }
        public void SaveEntity(params TableEntity[] list)
        {
            if (list == null || list.Length <= 0) return;
            foreach (TableEntity ent in list) if (ent != null) AddCommands(ent.GetSaveCommands(this));
        }
        public void DeleteEntity(params TableEntity[] list)
        {
            if (list == null || list.Length <= 0) return;
            foreach (TableEntity ent in list) if (ent != null) AddCommands(ent.GetDeleteCommands(this));
        }
        public long Commit()
        {
            long result = 0;
            lock (m_lock)
            {
                result = ExecuteSQLCommand(mc_commands);
                mc_commands.Clear();
            }
            return result;
        }
        #endregion

        #region Private Methods
        private void AddCommands(List<DbCommand> commands)
        {
            if(commands == null || commands.Count <= 0)return;
            lock (m_lock)
            {
                mc_commands.AddRange(commands);
            }
        }
        internal List<T> RetrieveEntity<T>(DbCommand command, bool needfresh) where T : TableEntity, new()
        {
            var result = new List<T>();
            var data = Retrieve(command);
            if (data != null && data.Rows.Count > 0)
            {
                var ticks = DateTime.Now.Ticks;
                foreach (DataRow row in data.Rows)
                {
                    var ent = new T();
                    ent.SetDBAccessor(this);
                    if (needfresh)
                    {
                        ent.SetEntity(row);
                        ent.Fresh();
                    }
                    else
                    {
                        ent.SetEntity(row, ticks);
                    }
                    result.Add(ent);
                }
            }
            return result;
        }
        #endregion

        public void Dispose()
        {
            ClearError();
            try
            {
                if (msc_con != null)
                {
                    if (!msc_con.State.Equals(ConnectionState.Closed)) msc_con.Close();
                    msc_con.Dispose();
                }
            }
            catch (System.Exception lept_err)
            {
                WriteError("Dispose", lept_err.Message);
            }
            finally
            {
                msc_con = null;
            }
        }

        ~DatabaseCore()
        {
            mcnt_info = null;
            Dispose();
        }
    }
}
