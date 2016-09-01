using System;
using System.Text;
using System.Threading;
using Task.common.enums;
using Task.common.messages;

namespace Task
{
    public class Worker
    {
        private CancellationTokenSource cts = null;
        private TaskCore task = null;
        public TaskCore Task
        {
            get
            {
                return task;
            }
        }

        public string id { get; private set; }
        public WorkerStatus status { get; private set; }
        private int sleep { get; set; }

        public Worker(string id, TaskCore task, int sleep = 1)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new Exception(string.Format(Message.MSG_ERR_PARM_IS_NULL_OR_EMPTY, "id"));
            if (task == null) throw new Exception(string.Format(Message.MSG_ERR_PARM_IS_NULL_OR_EMPTY, "task"));
            this.id = id.Trim();
            this.task = task;
            this.status = WorkerStatus.stopped;
            this.sleep = sleep <= 0 ? 1 : sleep;
        }

        public void Start()
        {
            new Thread(() =>
            {
                RESULT result0 = RESULT.NONE;
                StringBuilder messager0 = null;
                RESULT result1 = RESULT.NONE;
                StringBuilder messager1 = null;
                RESULT result2 = RESULT.NONE;
                StringBuilder messager2 = null;

                try
                {
                    #region 1. Starting...
                    try
                    {
                        if (this.status == WorkerStatus.running)
                        {
                            task._Started(RESULT.NONE, null);
                            return;
                        }
                        else
                        {
                            this.status = WorkerStatus.running;
                            cts = new CancellationTokenSource();
                            messager1 = new StringBuilder();
                            result1 = task._Initial(messager1);
                            task._Started(result1, messager1.ToString());
                        }
                    }
                    catch (Exception ex1)
                    {
                        result1 = RESULT.SYSERR1;
                        if (messager1 == null) messager1 = new StringBuilder();
                        messager1.AppendLine("SYSERR1:");
                        messager1.AppendLine(ex1.Message);
                        messager1.AppendLine(ex1.StackTrace);
                        task._Started(RESULT.SYSERR1, messager1.ToString());
                    }
                    if (result1 != RESULT.OK)
                    {
                        //this.status = WorkerStatus.stopped;
                        return;
                    }
                    #endregion

                    #region 2.Running...
                    while (!cts.IsCancellationRequested)
                    {
                        try
                        {
                            messager2 = new StringBuilder();
                            result2 = task._Process(messager2);
                        }
                        catch (Exception ex2)
                        {
                            result2 = RESULT.SYSERR2;
                            if (messager2 == null) messager2 = new StringBuilder();
                            messager2.AppendLine("SYSERR2:");
                            messager2.AppendLine(ex2.Message);
                            messager2.AppendLine(ex2.StackTrace);
                        }
                        finally
                        {
                            task._Completed(result2, messager2 == null ? string.Empty : messager2.ToString());
                            Thread.Sleep(sleep);
                        }
                    }
                    #endregion

                    result0 = RESULT.OK;
                }
                catch (Exception ex0)
                {
                    result0 = RESULT.SYSERR0;
                    if (messager0 == null) messager0 = new StringBuilder();
                    messager0.AppendLine("SYSERR0:");
                    messager0.AppendLine(ex0.Message);
                    messager0.AppendLine(ex0.StackTrace);
                }
                finally
                {
                    task._Stopped(result0, messager0 == null ? string.Empty : messager0.ToString());
                    this.status = WorkerStatus.stopped;
                }
            }).Start();
        }

        public void Stop()
        {
            StringBuilder messager0 = null;
            try
            {
                if (status == WorkerStatus.stopped)
                {
                    task._Stopped(RESULT.NONE, null);
                    return;
                }
                else
                {
                    if (cts != null) cts.Cancel();
                    if (task != null) task._Stop();
                }
            }
            catch (Exception ex0)
            {
                if (messager0 == null) messager0 = new StringBuilder();
                messager0.AppendLine("Exception:");
                messager0.AppendLine(ex0.Message);
                messager0.AppendLine(ex0.StackTrace);
                task._Stopped(RESULT.SYSERR0, messager0.ToString());
            }
            finally
            {
                //this.status = WorkerStatus.stopped;
            }
        }

    }
}
