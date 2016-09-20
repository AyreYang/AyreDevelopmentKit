using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Log.common.enums
{
    public enum MODE
    {
        DEBUG, RELEASE
    }

    public enum TYPE
    {
        INFO = 1,
        PROMPT = 2,
        WARNING = 3,
        ERROR = 4
    }

    public enum LEVEL
    {
        ALL = 0,
        ONLY_INFO = 1,
        ONLY_PROMPT = 2,
        ONLY_WARNING = 3,
        ONLY_ERROR = 4,
        INFO_PROMPT = 5,
        INFO_WARNING = 6,
        INFO_ERROR = 7,
        PROMPT_WARNING = 8,
        PROMPT_ERROR = 9,
        WARNING_ERROR = 10,
        INFO_PROMPT_WARNING = 11,
        PROMPT_WARNING_ERROR = 12
    }
}
