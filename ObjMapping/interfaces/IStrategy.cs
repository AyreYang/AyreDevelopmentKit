using ObjMapping.Enums;
using ObjMapping.Tools;
using System;

namespace ObjMapping.Interfaces
{
    public interface  IStrategy
    {
        Guid Key { get; }
        MappingMode Mode { get; }
        IStrategy AddMap(string from, string to, Func<IContext, Result> converter = null);
        IStrategy ResetFilterList();
        IStrategy ResetCareList(params string[] fields);
        IStrategy ResetIgnoreList(params string[] fields);
        IStrategy AddCareList(params string[] fields);
        IStrategy AddIgnoreList(params string[] fields);

        IStrategy CareAll();
        IStrategy IgnoreAll();

    }
}
