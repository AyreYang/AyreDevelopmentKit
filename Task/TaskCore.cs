using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task.common.enums;
using Task.common.messages;
using Task.common.utilities;

namespace Task
{
    public abstract class TaskCore
    {
        private readonly object m_lock_put = new object();
        private readonly object m_lock_get = new object();

        private Dictionary<string, PlugIn> Plugs4Put = new Dictionary<string, PlugIn>();
        private Dictionary<string, PlugOut> Plugs4Get = new Dictionary<string, PlugOut>();


        private void AddPlug<T>(string id, T plug) where T : Plug
        {
            if (string.IsNullOrWhiteSpace(id)) throw new Exception(string.Format(Message.MSG_ERR_PARM_IS_NULL_OR_EMPTY, "id"));
            if (plug == null) return;

            var pid = id.Trim();

            if (typeof(T) == typeof(PlugIn))
            {
                lock (m_lock_put)
                {
                    if (Plugs4Put.ContainsKey(pid)) throw new Exception(string.Format(Message.MSG_ERR_ID_EXISTS, pid));
                    Plugs4Put.Add(pid, plug as PlugIn);
                }
            }
            if (typeof(T) == typeof(PlugOut))
            {
                lock (m_lock_get)
                {
                    if (Plugs4Get.ContainsKey(pid)) throw new Exception(string.Format(Message.MSG_ERR_ID_EXISTS, pid));
                    Plugs4Get.Add(pid, plug as PlugOut);
                }
            }
        }
        public void AddGetPlug(string id, DataPipe pipe)
        {
            if (pipe != null) AddPlug<PlugOut>(id, pipe.CreatePlug<PlugOut>());
        }
        public void AddPutPlug(string id, DataPipe pipe)
        {
            if (pipe != null) AddPlug<PlugIn>(id, pipe.CreatePlug<PlugIn>());
        }

        protected T GetData<T>(string id, T def = default(T))
        {
            if(string.IsNullOrWhiteSpace(id))return def;
            var pid = id.Trim();
            TransData data = null;
            lock (m_lock_get)
            {
                data = Plugs4Get.ContainsKey(pid) ? Plugs4Get[pid].Out() : null;
            }
            return (data != null) ? data.GetData<T>() : def;
        }
        protected List<T> GetData<T>(List<string> exclude = null)
        {
            var list = new List<T>();
            lock (m_lock_get)
            {
                foreach (PlugOut plug in Plugs4Get.Values)
                {
                    if (exclude != null && exclude.Count > 0)
                    {
                        if (exclude.Any(id => id.Equals(plug.ID))) continue;
                    }
                    var data = plug.Out();
                    if (data != null) list.Add(data.GetData<T>());
                }
            }
            return list;
        }
        protected bool PutData<T>(string id, T data)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            var pid = id.Trim();
            var result = false;
            lock (m_lock_put)
            {
                if (Plugs4Put.ContainsKey(pid)) result = Plugs4Put[pid].In(new TransData(data));
            }
            return result;
        }
        protected void BroadcastData<T>(T data, List<string> exclude = null)
        {
            lock (m_lock_put)
            {
                foreach (PlugIn plug in Plugs4Put.Values)
                {
                    if (exclude != null && exclude.Count > 0)
                    {
                        if (exclude.Any(id => id.Equals(plug.ID))) continue;
                    }
                    plug.In(new TransData(data));
                }
            }
        }
        protected T FindPlug<T>(string id) where T : Plug
        {
            if (string.IsNullOrWhiteSpace(id)) throw new Exception(string.Format(Message.MSG_ERR_PARM_IS_NULL_OR_EMPTY, "id"));

            var pid = id.Trim();
            object plug = null;

            if (typeof(T) == typeof(PlugIn))
            {
                lock (m_lock_put)
                {
                    plug = (Plugs4Put.ContainsKey(pid)) ? Plugs4Put[pid] : null;
                }
            }
            if (typeof(T) == typeof(PlugOut))
            {
                lock (m_lock_get)
                {
                    plug = (Plugs4Get.ContainsKey(pid)) ? Plugs4Get[pid] : null;
                }
            }
            return (plug != null) ? (T)plug : default(T);
        }
        protected int GetPipeStatus(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return -1;
            var pid = id.Trim();
            var status = -1;
            lock (m_lock_put)
            {
                if (Plugs4Put.ContainsKey(pid)) status = Plugs4Put[pid].IsFull ? 0 : 1;
            }
            return status;
        }
        protected int GetPipeCount(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return -1;
            var pid = id.Trim();
            var count = -1;
            lock (m_lock_put)
            {
                if (Plugs4Put.ContainsKey(pid)) count = Plugs4Put[pid].DataCount;
            }
            return count;
        }

        #region Abstract Methods
        protected abstract RESULT Initial(StringBuilder messager);
        protected abstract RESULT Process(StringBuilder messager);
        protected abstract void Stop();
        protected abstract void Completed(RESULT status, string message);

        protected abstract void Started(RESULT status, string message);
        protected abstract void Stopped(RESULT status, string message);
        #endregion

        internal RESULT _Initial(StringBuilder messager)
        {
            return Initial(messager);
        }
        internal RESULT _Process(StringBuilder messager)
        {
            return Process(messager);
        }
        internal void _Stop()
        {
            Stop();
        }
        internal void _Completed(RESULT status, string message)
        {
            Completed(status, message);
        }
        internal void _Started(RESULT status, string message)
        {
            Started(status, message);
        }
        internal void _Stopped(RESULT status, string message)
        {
            Stopped(status, message);
        }
    }
}
