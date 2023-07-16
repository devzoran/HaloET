using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    [FriendClass(typeof(MessageWait))]
    public static class MessageWaitSystem
    {
        [ObjectSystem]
        public class StateWaitDestroySystem: DestroySystem<MessageWait>
        {
            public override void Destroy(MessageWait self)
            {
                foreach (var hashsetV in self.tcss.Values.ToArray())
                {
                    foreach (var v in hashsetV)
                    {
                        v.SetResult(new Wait_Message() { Error  = WaitTypeError.Timeout});
                    }
                }

                self.objs.Clear();
                self.tcss.Clear();
            }
        }

        private static async ETTask SetResultFrame(this MessageWait self, ETTask<Wait_Message> tcs, string message)
        {
            await TimerComponent.Instance.WaitFrameAsync();
            
            if (self.objs.TryGetValue(message, out Wait_Message val))
            {
                tcs.SetResult(val);
            }
        }
        
        public static async ETTask<Wait_Message> Wait(this MessageWait self, string type, ETCancellationToken cancellationToken = null)
        {
            ETTask<Wait_Message> tcs = ETTask<Wait_Message>.Create();

            if (self.objs.TryGetValue(type, out Wait_Message val))
            {
                self.SetResultFrame(tcs, type).Coroutine();
                return await tcs;
            }

            if (!self.tcss.TryGetValue(type, out var hashSet))
            {
                self.tcss[type] = hashSet = new HashSet<ETTask<Wait_Message>>();
            }

            hashSet.Add(tcs);

            void CancelAction()
            {
                self.Notify(new Wait_Message() { Error = WaitTypeError.Cancel });
            }

            Wait_Message ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);    
            }
            return ret;
        }
        
        
        public static async ETTask<Wait_Message> Wait(this MessageWait self, string type, int timeout, ETCancellationToken cancellationToken = null)
        {
            var tcs = ETTask<Wait_Message>.Create();

            if (self.objs.TryGetValue(type, out Wait_Message val))
            {
                self.SetResultFrame(tcs, type).Coroutine();
                return await tcs;
            }

            if (!self.tcss.TryGetValue(type, out var hashSet))
            {
                self.tcss[type] = hashSet = new HashSet<ETTask<Wait_Message>>();
            }

            hashSet.Add(tcs);
            
            async ETTask WaitTimeout()
            {
                await TimerComponent.Instance.WaitAsync(timeout, cancellationToken);
                if (cancellationToken.IsCancel())
                {
                    return;
                }
                if (tcs.IsCompleted)
                {
                    return;
                }
                self.Notify(new Wait_Message() { Error = WaitTypeError.Timeout });
            }
            
            WaitTimeout().Coroutine();
            
            void CancelAction()
            {
                self.Notify(new Wait_Message() { Error = WaitTypeError.Cancel });
            }
            
            Wait_Message ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);    
            }
            return ret;
        }


        public static void Notify(this MessageWait self, string type, int timeOutMs = 10000, object param = null)
        {
            Wait_Message msg = new Wait_Message() { Type = type, Param = param };
            self.Notify(msg, timeOutMs);
        }

        /// <summary>
        /// 发送通知, 10秒内都可以Await 到该通知消息,超时则自动清除通知
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msg">附带参数</param>
        /// <param name="timeOutMs">  小于0 时, 将永不超时 </param>
        public static void Notify(this MessageWait self, Wait_Message msg, int timeOutMs = 10000)
        {
            self.objs[msg.Type] = msg;

            if (timeOutMs > 0)
            {
                self.RemoveObj(msg.Type, timeOutMs).Coroutine();
            }

            if (!self.tcss.TryGetValue(msg.Type, out var hashSet))
            {
                return;
            }

            self.tcss.Remove(msg.Type);
            
            foreach (var v in hashSet)
            {
                v.SetResult(msg);
            }
        }

        private static async ETTask RemoveObj(this MessageWait self, string message, int waitTime)
        {
            await TimerComponent.Instance.WaitAsync(waitTime);
            self.objs.Remove(message);
        }

        /// <summary>
        /// 移除掉当前缓存的通知消息
        /// 慎用
        /// </summary>
        public static void Remove(this MessageWait self, string message)
        {
            self.objs.Remove(message);
        }
    }

    [ComponentOf]
    public class MessageWait: Entity, IAwake, IDestroy
    {
        public Dictionary<string, Wait_Message> objs = new Dictionary<string, Wait_Message>();
        public Dictionary<string, HashSet<ETTask<Wait_Message>>> tcss = new Dictionary<string, HashSet<ETTask<Wait_Message>>>();
    }
    
    public struct Wait_Message: IWaitType
    {
        public int Error
        {
            get;
            set;
        }

        public string Type;
        public object Param;
    }

}