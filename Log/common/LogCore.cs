using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Log.common.interfaces;
using Log.common.enums;
using Log.common.messages;

namespace Log.common
{
    public abstract class LogCore : ILog
    {
        private const string date = "yyyy-MM-dd";
        private const string pattern = "{0} {1}[{2}]:{3}";  // 0:time, 1:id, 2:type, 3:msg

        public abstract long Write(TYPE type, string message);
        public long Write(Exception exception)
        {
            if (exception == null) return 0;
            return Write(TYPE.ERROR, exception.ToString());
        }

        public LEVEL level { get; private set; }
        public string id { get; private set; }

        protected LogCore(string id, LEVEL level = LEVEL.ALL)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new Exception(string.Format(Message.MSG_ERR_PARM_IS_NULL_OR_EMPTY, "id"));
            this.id = id.Trim();
            this.level = level;
        }

        protected string BuildMessage(TYPE type, string message)
        {
            string msg = null;
            #region Filter By Level
            switch (level)
            {
                case LEVEL.ALL:
                    msg = message;
                    break;
                case LEVEL.ONLY_INFO:
                    if(type == TYPE.INFO) msg = message;
                    break;
                case LEVEL.ONLY_PROMPT:
                    if(type == TYPE.PROMPT) msg = message;
                    break;
                case LEVEL.ONLY_WARNING:
                    if(type == TYPE.WARNING) msg = message;
                    break;
                case LEVEL.ONLY_ERROR:
                    if(type == TYPE.ERROR) msg = message;
                    break;
                case LEVEL.INFO_PROMPT:
                    if(type == TYPE.INFO) msg = message;
                    if(type == TYPE.PROMPT) msg = message;
                    break;
                case LEVEL.INFO_WARNING:
                    if(type == TYPE.INFO) msg = message;
                    if(type == TYPE.WARNING) msg = message;
                    break;
                case LEVEL.INFO_ERROR:
                    if(type == TYPE.INFO) msg = message;
                    if(type == TYPE.ERROR) msg = message;
                    break;
                case LEVEL.PROMPT_WARNING:
                    if(type == TYPE.PROMPT) msg = message;
                    if(type == TYPE.WARNING) msg = message;
                    break;
                case LEVEL.PROMPT_ERROR:
                    if(type == TYPE.PROMPT) msg = message;
                    if(type == TYPE.ERROR) msg = message;
                    break;
                case LEVEL.WARNING_ERROR:
                    if(type == TYPE.WARNING) msg = message;
                    if(type == TYPE.ERROR) msg = message;
                    break;
                case LEVEL.INFO_PROMPT_WARNING:
                    if(type == TYPE.INFO) msg = message;
                    if(type == TYPE.PROMPT) msg = message;
                    if(type == TYPE.WARNING) msg = message;
                    break;
                case LEVEL.PROMPT_WARNING_ERROR:
                    if (type == TYPE.PROMPT) msg = message;
                    if (type == TYPE.WARNING) msg = message;
                    if (type == TYPE.ERROR) msg = message;
                    break;
            }
            #endregion

            if (msg == null) return null;

            var now = DateTime.Now;
            var time = string.Format("{0} {1}:{2}:{3}({4})", 
                now.ToString(date),
                now.Hour.ToString().PadLeft(2, '0'), now.Minute.ToString().PadLeft(2, '0'), now.Second.ToString().PadLeft(2, '0'),
                now.Millisecond.ToString().PadLeft(3, '0'));
            msg = string.Format(pattern, time, id, type.ToString().First(), msg);
            return msg;
        }
    }
}
