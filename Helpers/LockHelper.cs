using System;
using System.Threading;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    /// 同步锁帮助类
    /// </summary>
    public class LockHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LockHelper()
        {
            _LockObj = new object();
        }

        /// <summary>
        /// 锁定对象并执行方法
        /// </summary>
        /// <param name="lockTakenAction">获取锁后要执行的方法</param>
        /// <param name="shouldEnterLock">检查是否要获取锁</param>
        public void Lock(Action lockTakenAction, Func<bool> shouldEnterLock = null)
            => Lock(takeLock: () =>
            {
                bool lockTaken = false;
                Monitor.Enter(_LockObj, ref lockTaken);
                return lockTaken;
            }, lockTakenAction: lockTakenAction, shouldEnterLock: shouldEnterLock);

        /// <summary>
        /// 尝试锁定对象并执行方法
        /// </summary>
        /// <param name="lockTakenAction">获取锁后要执行的方法</param>
        /// <param name="shouldEnterLock">检查是否要获取锁</param>
        /// <param name="tryTimeout">尝试超时时间,为空表示永久等待</param>
        public void TryLock(Action lockTakenAction, Func<bool> shouldEnterLock = null, TimeSpan? tryTimeout = null)
            => Lock(takeLock: () =>
            {
                bool lockTaken = false;
                if (tryTimeout.HasValue)
                {
                    Monitor.TryEnter(_LockObj, tryTimeout.Value, ref lockTaken);
                }
                else
                {
                    Monitor.TryEnter(_LockObj, ref lockTaken);
                }
                return lockTaken;
            }, lockTakenAction: lockTakenAction, shouldEnterLock: shouldEnterLock);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="takeLock"></param>
        /// <param name="lockTakenAction"></param>
        /// <param name="shouldEnterLock"></param>
        private void Lock(Func<bool> takeLock, Action lockTakenAction, Func<bool> shouldEnterLock = null)
        {
            if (!(shouldEnterLock?.Invoke() ?? true))
            {
                return;
            }

            if (!takeLock())
            {
                return;
            }

            try
            {
                if (!(shouldEnterLock?.Invoke() ?? true))
                {
                    return;
                }

                lockTakenAction();
            }
            finally
            {
                Monitor.Exit(_LockObj);
            }
        }

        /// <summary>
        /// 同步对象
        /// </summary>
        private readonly object _LockObj = null;
    }
}