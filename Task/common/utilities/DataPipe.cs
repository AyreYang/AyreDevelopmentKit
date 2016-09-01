using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Task.common.utilities
{
    public class DataPipe
    {
        private const int DEFAULT_DATA_COUNT_MAX = 100;

        private string id = Guid.NewGuid().ToString();
        private volatile object m_lock = new object();
        private ConcurrentQueue<TransData> m_queue = new ConcurrentQueue<TransData>();
        public int MaxCount { get; private set; }
        public int DataCount
        {
            get
            {
                var count = 0;
                lock (m_lock)
                {
                    count = m_queue.Count;
                }
                return count;
            }
        }
        public bool IsFull
        {
            get
            {
                return DataCount >= MaxCount;
            }
        }
        public string ID
        {
            get { return this.id; }
        }

        public DataPipe(int count = 0)
        {
            MaxCount = (count > 0) ? count : DEFAULT_DATA_COUNT_MAX;
        }

        private bool In(TransData data)
        {
            bool lb_ret = false;
            if (data != null)
            {
                lock (m_lock)
                {
                    if (m_queue.Count < MaxCount)
                    {
                        m_queue.Enqueue(data);
                        lb_ret = true;
                    }
                }
            }
            return lb_ret;
        }
        private TransData Out()
        {
            TransData ltd_data = null;
            lock (m_lock)
            {
                m_queue.TryDequeue(out ltd_data);
            }
            return ltd_data;
        }

        public T CreatePlug<T>()
            where T : Plug
        {
            object plug = null;

            if (typeof(T) == typeof(PlugIn)) plug = new PlugIn(this, this.In);
            if (typeof(T) == typeof(PlugOut)) plug = new PlugOut(this, this.Out);

            return (plug == null) ? null : (T)plug;
        }

        private long GetMemSize(object data)
        {
            if (data == null) return 0;
            var formatter = new BinaryFormatter();
            var rems = new MemoryStream();
            formatter.Serialize(rems, data);
            return rems.Length;
        } 
    }
}
