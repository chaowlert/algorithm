using System;
using System.Collections.Generic;
using System.Threading;

namespace Chaow.Threading
{
    public static class ActionExt
    {
        public static Action<T> Create<T>(T t, Action<T> tResult)
        {
            return tResult;
        }

        public static Action<T1, T2> Create<T1, T2>(T1 t1, T2 t2, Action<T1, T2> tResult)
        {
            return tResult;
        }

        public static Action<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3, Action<T1, T2, T3> tResult)
        {
            return tResult;
        }

        public static Action<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4, Action<T1, T2, T3, T4> tResult)
        {
            return tResult;
        }

        public static Action<T> When<T>(this Action<T> action, Func<T, bool> predicate, Action<T> alternative)
        {
            return arg =>
            {
                if (predicate(arg))
                    alternative(arg);
                else
                    action(arg);
            };
        }

        public static Action<T1, T2> When<T1, T2>(this Action<T1, T2> action, Func<T1, T2, bool> predicate, Action<T1, T2> alternative)
        {
            return (arg1, arg2) =>
            {
                if (predicate(arg1, arg2))
                    alternative(arg1, arg2);
                else
                    action(arg1, arg2);
            };
        }

        public static Action<T1, T2, T3> When<T1, T2, T3>(this Action<T1, T2, T3> action, Func<T1, T2, T3, bool> predicate, Action<T1, T2, T3> alternative)
        {
            return (arg1, arg2, arg3) =>
            {
                if (predicate(arg1, arg2, arg3))
                    alternative(arg1, arg2, arg3);
                else
                    action(arg1, arg2, arg3);
            };
        }

        public static Action<T1, T2, T3, T4> When<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, Func<T1, T2, T3, T4, bool> predicate, Action<T1, T2, T3, T4> alternative)
        {
            return (arg1, arg2, arg3, arg4) =>
            {
                if (predicate(arg1, arg2, arg3, arg4))
                    alternative(arg1, arg2, arg3, arg4);
                else
                    action(arg1, arg2, arg3, arg4);
            };
        }

        public static Action Pass<T>(this Action<T> action, T arg1)
        {
            return () => action(arg1);
        }

        public static Action Pass<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            return () => action(arg1, arg2);
        }

        public static Action Pass<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            return () => action(arg1, arg2, arg3);
        }

        public static Action Pass<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return () => action(arg1, arg2, arg3, arg4);
        }

        public static Worker Run(this Action action)
        {
            var w = new Worker(action);
            ThreadPool.QueueUserWorkItem(Worker.run, w);
            return w;
        }

        public static Worker Delay(this Action action, int milliseconds)
        {
            var w = new Worker(action);
            var t = new Timer(Worker.run, w, milliseconds, -1);
            if (!w.RegisterFinalizeAction(t.Dispose))
                t.Dispose();
            return w;
        }

        public static Worker Delay(this Action action, TimeSpan timeSpan)
        {
            return Delay(action, (int)timeSpan.TotalMilliseconds);
        }

        public static Worker UnsafeTimeout(this Action action, int milliseconds)
        {
            var actionWorker = action.Run();
            var cancelWorker = new Worker(actionWorker.UnsafeAbort);
            if (actionWorker.RegisterFinalizeAction(cancelWorker.Cancel))
            {
                var t = new Timer(Worker.run, cancelWorker, milliseconds, -1);
                if (!cancelWorker.RegisterFinalizeAction(t.Dispose))
                    t.Dispose();
            }
            return actionWorker;
        }

        public static Worker UnsafeTimeout(this Action action, TimeSpan timeSpan)
        {
            return UnsafeTimeout(action, (int)timeSpan.TotalMilliseconds);
        }

        public static Worker Timeout(this Action action, int milliseconds)
        {
            var actionWorker = action.Run();
            var cancelWorker = new Worker(actionWorker.Cancel);
            if (actionWorker.RegisterFinalizeAction(cancelWorker.Cancel))
            {
                var t = new Timer(Worker.run, cancelWorker, milliseconds, -1);
                if (!cancelWorker.RegisterFinalizeAction(t.Dispose))
                    t.Dispose();
            }
            return actionWorker;
        }

        public static Worker Timeout(this Action action, TimeSpan timeSpan)
        {
            return Timeout(action, (int)timeSpan.TotalMilliseconds);
        }

        public static Worker Periodical(this Action action, int milliseconds)
        {
            var w = new Worker(action, true);
            var t = new Timer(Worker.run, w, milliseconds, milliseconds);
            if (!w.RegisterFinalizeAction(t.Dispose))
                t.Dispose();
            return w;
        }

        public static Worker Periodical(this Action action, TimeSpan timeSpan)
        {
            return Periodical(action, (int)timeSpan.TotalMilliseconds);
        }

        public static bool JoinAll(this IEnumerable<Worker> workers)
        {
            return JoinAll(workers, -1, false);
        }

        public static bool JoinAll(this IEnumerable<Worker> workers, int milliseconds)
        {
            return JoinAll(workers, milliseconds, false);
        }

        public static bool JoinAll(this IEnumerable<Worker> workers, TimeSpan timeSpan)
        {
            return JoinAll(workers, timeSpan, false);
        }

        public static bool JoinAll(this IEnumerable<Worker> workers, int milliseconds, bool exitContext)
        {
            var locker = new object();
            var count = 0;
            Action action = () =>
            {
                lock (locker)
                {
                    count--;
                    if (count == 0)
                        Monitor.Pulse(locker);
                }
            };
            lock (locker)
            {
                foreach (var w in workers)
                {
                    if (w.RegisterFinalizeAction(action))
                        count++;
                }
                if (count > 0)
                    Monitor.Wait(locker, milliseconds, exitContext);
            }
            return true;
        }

        public static bool JoinAll(this IEnumerable<Worker> workers, TimeSpan timeSpan, bool exitContext)
        {
            return JoinAll(workers, (int)timeSpan.TotalMilliseconds, exitContext);
        }

        public static int JoinAny(this IEnumerable<Worker> workers)
        {
            return JoinAny(workers, -1, false);
        }

        public static int JoinAny(this IEnumerable<Worker> workers, int milliseconds)
        {
            return JoinAny(workers, milliseconds, false);
        }

        public static int JoinAny(this IEnumerable<Worker> workers, TimeSpan timeSpan)
        {
            return JoinAny(workers, timeSpan, false);
        }

        public static int JoinAny(this IEnumerable<Worker> workers, int milliseconds, bool exitContext)
        {
            var result = -1;
            var locker = new object();
            Action<int> action = i =>
            {
                lock (locker)
                {
                    if (result == -1)
                    {
                        result = i;
                        Monitor.Pulse(locker);
                    }
                }
            };
            lock (locker)
            {
                var index = 0;
                foreach (var w in workers)
                {
                    if (!w.RegisterFinalizeAction(action.Pass(index)))
                    {
                        result = index;
                        break;
                    }
                    index++;
                }
                if (index > 0 && result == -1)
                    Monitor.Wait(locker, milliseconds, exitContext);
            }
            return result;
        }

        public static int JoinAny(this IEnumerable<Worker> workers, TimeSpan timeSpan, bool exitContext)
        {
            return JoinAny(workers, (int)timeSpan.TotalMilliseconds, exitContext);
        }
    }

    public enum WorkerStatus
    {
        Alive = 0,
        Periodical = 1,
        NotifyCancel = 2,
        NotifyAbort = 3,
        Closed = 4,
    }

    public class Worker
    {
        //fields
        public static readonly Worker Empty = new Worker();
        [ThreadStatic] public static Worker Current;
        readonly Action _action;
        readonly object _locker = new object();
        int _abortWait;
        volatile Exception _exception;
        Action _finalizeAction;
        volatile WorkerStatus _status;
        Thread _thread;

        //static fields

        //constructors
        Worker()
        {
        }

        internal Worker(Action action)
        {
            _action = action;
        }

        internal Worker(Action action, bool isPeriodical)
        {
            _action = action;
            if (isPeriodical)
                _status = WorkerStatus.Periodical;
        }

        public WorkerStatus Status
        {
            get { return _status; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        //static properties
        public static bool IsCanceled
        {
            get
            {
                var worker = Current;
                if (worker == null)
                    return false;
                return worker._status >= WorkerStatus.NotifyCancel;
            }
        }

        //private methods
        void close()
        {
            if (_status == WorkerStatus.Closed)
                return;
            if (_finalizeAction != null)
            {
                try
                {
                    _finalizeAction();
                }
                catch
                {
                }
            }
            _status = WorkerStatus.Closed;
        }

        //public methods
        public void Cancel()
        {
            if (_status >= WorkerStatus.NotifyCancel)
                return;

            lock (_locker)
            {
                if (_thread == null)
                    close();
                else if (_status < WorkerStatus.NotifyCancel)
                    _status = WorkerStatus.NotifyCancel;
            }
        }

        public void UnsafeAbort()
        {
            if (_status == WorkerStatus.Closed)
                return;

            lock (_locker)
            {
                if (_thread != null)
                {
                    if (Thread.VolatileRead(ref _abortWait) > 0)
                    {
                        if (_status < WorkerStatus.NotifyAbort)
                            _status = WorkerStatus.NotifyAbort;
                        return;
                    }
                    _thread.Abort();
                    _thread = null;
                }
                close();
            }
        }

        public bool Join()
        {
            return Join(-1, false);
        }

        public bool Join(int milliseconds)
        {
            return Join(milliseconds, false);
        }

        public bool Join(TimeSpan timeSpan)
        {
            return Join(timeSpan, false);
        }

        public bool Join(int milliseconds, bool exitContext)
        {
            bool result;
            var locker = new object();
            Action action = () =>
            {
                lock (locker)
                    Monitor.Pulse(locker);
            };
            lock (locker)
            {
                result = !RegisterFinalizeAction(action);
                if (!result)
                    result = Monitor.Wait(locker, milliseconds, exitContext);
            }
            return result;
        }

        public bool Join(TimeSpan timeSpan, bool exitContext)
        {
            return Join((int)timeSpan.TotalMilliseconds, exitContext);
        }

        public bool RegisterFinalizeAction(Action action)
        {
            lock (_locker)
            {
                if (_status == WorkerStatus.Closed)
                    return false;
                _finalizeAction += action;
            }
            return true;
        }

        //internal static methods
        internal static void run(object obj)
        {
            var w = (Worker)obj;
            var hasWorker = false;
            try
            {
                lock (w._locker)
                {
                    if (w._status == WorkerStatus.Closed || w._thread != null)
                        return;
                    hasWorker = true;
                    w._thread = Thread.CurrentThread;
                    Current = w;
                }
                w._action();
                AbortWait(w);
            }
            catch (Exception ex)
            {
                w._exception = ex;
                AbortWait(w);
            }
            finally
            {
                if (hasWorker)
                {
                    lock (w._locker)
                    {
                        w._thread = null;
                        if (w._status != WorkerStatus.Periodical)
                            w.close();
                        Current = null;
                    }
                }
            }
        }

        //public static method
        public static void AbortWait()
        {
            AbortWait(Current);
        }

        public static void AbortWait(Worker worker)
        {
            if (worker == null)
                return;
            Interlocked.Increment(ref worker._abortWait);
        }

        public static void AbortSet()
        {
            AbortSet(Current);
        }

        public static void AbortSet(Worker worker)
        {
            if (worker == null)
                return;
            var i = Interlocked.Decrement(ref worker._abortWait);
            if (i < 0)
                Interlocked.Increment(ref worker._abortWait);
            else if (i == 0 && worker._status == WorkerStatus.NotifyAbort)
                worker.UnsafeAbort();
        }
    }
}