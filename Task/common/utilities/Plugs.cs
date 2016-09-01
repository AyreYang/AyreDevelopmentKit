using System;
using Task.common.messages;

namespace Task.common.utilities
{
    public abstract class Plug
    {
        private DataPipe pipe { get; set; }
        public int DataCount
        {
            get { return pipe.DataCount; }
        }
        public string ID
        {
            get
            {
                return pipe.ID;
            }
        }
        public bool IsFull
        {
            get
            {
                return pipe.IsFull;
            }
        }

        protected Plug(DataPipe pipe)
        {
            if (pipe == null) throw new Exception(string.Format(Message.MSG_ERR_PARM_IS_NULL_OR_EMPTY, "id"));
            this.pipe = pipe;
        }
    }

    public class PlugIn : Plug
    {
        private Func<TransData, bool> func = null;
        public PlugIn(DataPipe pipe, Func<TransData, bool> func)
            : base(pipe)
        {
            this.func = func;
        }

        public bool In(TransData data)
        {
            if (this.func == null) return false;
            return this.func(data);
        }
    }

    public class PlugOut : Plug
    {
        private Func<TransData> func = null;
        public PlugOut(DataPipe pipe, Func<TransData> func)
            : base(pipe)
        {
            this.func = func;
        }

        public TransData Out()
        {
            if (this.func == null) return null;
            return this.func();
        }
    }
}
