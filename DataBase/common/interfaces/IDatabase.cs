using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System;
using DataBase.common.objects;

namespace DataBase.common.interfaces
{
    public interface IDatabase : IDisposable
    {
        DataTable Retrieve(DbCommand command);
        List<T> Retrieve<T>(Clause clause, Sort sort) where T : TableEntity, new();
        T RetrieveValue<T>(DbCommand command, T def = default(T));
            //where T : struct;
        long ExecuteSQLCommand(DbCommand command);
        long ExecuteSQLCommand(List<DbCommand> commands);
        long GenerateSequence(string sequence);

        void SetDBAccessor2(TableEntity entity);
        void InsertEntity(params TableEntity[] list);
        void UpdateEntity(params TableEntity[] list);
        void SaveEntity(params TableEntity[] list);
        void DeleteEntity(params TableEntity[] list);
        long Commit();

    }
}
