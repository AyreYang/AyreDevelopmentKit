using ObjMapping.enums;
using ObjMapping.tools;
using System;
using System.Collections.Generic;

namespace ObjMapping.interfaces
{
    public interface  IStrategy
    {
        Guid Key { get; }
        MappingMode Mode { get; }
        IStrategy AddMap(string from, string to, Func<object, object, cnvt> converter = null);
        IStrategy ResetFilterList();
        IStrategy ResetCareList(params string[] fields);
        IStrategy ResetIgnoreList(params string[] fields);
        IStrategy AddCareList(params string[] fields);
        IStrategy AddIgnoreList(params string[] fields);

        IStrategy CareAll();
        IStrategy IgnoreAll();

    }
}
