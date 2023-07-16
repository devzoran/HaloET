using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    [FriendClass(typeof(StateWait))]
    public static class StateWaitSystem
    {
        [ObjectSystem]
        public class StateWaitDestroySystem: DestroySystem<StateWait>
        {
            public override void Destroy(StateWait self)
            {
                if (self == null || self.IsDisposed) return;
                
                foreach (HashSet<object> hashsetV in self.tcss.Values.ToArray())
                {
                    foreach (var v in hashsetV)
                    {
                        ((IDestroyRun) v).SetResult();
                    }
                }

                self.objs.Clear();
                self.tcss.Clear();
            }
        }

        private interface IDestroyRun
        {
            void SetResult();
        }

        private class ResultCallback<K>: IDestroyRun where K : struct, IWaitType
        {
            private ETTask<K> tcs;

            public ResultCallback()
            {
                this.tcs = ETTask<K>.Create(true);
            }

            public bool IsDisposed
            {
                get
                {
                    return this.tcs == null;
                }
            }

            public ETTask<K> Task => this.tcs;

            public void SetResult(K k)
            {
                var t = tcs;
                this.tcs = null;
                t.SetResult(k);
            }

            public void SetResult()
            {
                var t = tcs;
                this.tcs = null;
                t.SetResult(new K() { Error = WaitTypeError.Destroy });
            }
        }

        private static async ETTask SetResultFrame<T>(this StateWait self, object tcs) where T : struct, IWaitType
        {
            await TimerComponent.Instance.WaitFrameAsync();
            
            Type type = typeof (T);
            if (self.objs.TryGetValue(type, out object val))
            {
                ((ResultCallback<T>) tcs).SetResult((T)val);
            }
        }
        
        public static async ETTask<T> Wait<T>(this StateWait self, ETCancellationToken cancellationToken = null) where T : struct, IWaitType
        {
            ResultCallback<T> tcs = new ResultCallback<T>();
            Type type = typeof (T);

            if (self.objs.TryGetValue(type, out object val))
            {
                self.SetResultFrame<T>(tcs).Coroutine();
                return await tcs.Task;
            }

            if (!self.tcss.TryGetValue(type, out HashSet<object> hashSet))
            {
                self.tcss[type] = hashSet = new HashSet<object>();
            }
            
            hashSet.Add(tcs);

            void CancelAction()
            {
                self.Notify(new T() { Error = WaitTypeError.Cancel });
            }

            T ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs.Task;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);    
            }
            return ret;
        }
        
        
        public static async ETTask<T> Wait<T>(this StateWait self, int timeout, ETCancellationToken cancellationToken = null) where T : struct, IWaitType
        {
            ResultCallback<T> tcs = new ResultCallback<T>();
            Type type = typeof (T);

            if (self.objs.TryGetValue(type, out object val))
            {
                self.SetResultFrame<T>(tcs).Coroutine();
                return await tcs.Task;
            }

            if (!self.tcss.TryGetValue(type, out HashSet<object> hashSet))
            {
                self.tcss[type] = hashSet = new HashSet<object>();
            }

            hashSet.Add(tcs);
            
            async ETTask WaitTimeout()
            {
                await TimerComponent.Instance.WaitAsync(timeout, cancellationToken);
                if (cancellationToken.IsCancel())
                {
                    return;
                }
                if (tcs.IsDisposed)
                {
                    return;
                }
                self.Notify(new T() { Error = WaitTypeError.Timeout });
            }
            
            WaitTimeout().Coroutine();
            
            void CancelAction()
            {
                self.Notify(new T() { Error = WaitTypeError.Cancel });
            }
            
            T ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs.Task;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);    
            }
            return ret;
        }


        /// <summary>
        /// 发送通知, 10秒内都可以Await 到该通知消息,超时则自动清除通知
        /// </summary>
        /// <param name="self"></param>
        /// <param name="obj"></param>
        /// <param name="timeOutMs">  小于0 时, 将永不超时 </param>
        /// <typeparam name="T"></typeparam>
        public static void Notify<T>(this StateWait self, T obj, int timeOutMs = 10000) where T : struct, IWaitType
        {
            Type type = typeof (T);
            self.objs[type] = obj;

            if (timeOutMs > 0)
            {
                self.RemoveObj(type, timeOutMs).Coroutine();
            }
            
            if (!self.tcss.TryGetValue(type, out HashSet<object> hashSet))
            {
                return;
            }

            self.tcss.Remove(type);

            foreach (var v in hashSet)
            {
                ((ResultCallback<T>) v).SetResult(obj);
            }
        }

        private static async ETTask RemoveObj(this StateWait self, Type type, int waitTime)
        {
            await TimerComponent.Instance.WaitAsync(waitTime);
            self.objs.Remove(type);
        }

        /// <summary>
        /// 移除掉当前缓存的通知消息
        /// 慎用
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        public static void Remove<T>(this StateWait self) where T : struct, IWaitType
        {
            self.objs.Remove(typeof(T));
        }
    }

    [ComponentOf]
    public class StateWait: Entity, IAwake, IDestroy
    {
        public Dictionary<Type, object> objs = new Dictionary<Type, object>();
        public Dictionary<Type, HashSet<object>> tcss = new Dictionary<Type, HashSet<object>>();
    }
}