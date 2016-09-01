using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Task.common.enums
{
    public enum WorkerStatus
    {
        stopped = 0,
        running = 1
    }

    public enum RESULT
    {
        NONE = 0,
        OK = 1,
        NG = 2,
        SYSERR0 = -100,
        SYSERR1 = -101,
        SYSERR2 = -102
    }
}
