using System;
using System.Collections.Generic;

namespace ET
{
    [ComponentOf]
    public class CDComponent: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// key 为 id   val 为CD结束的时间戳
        /// </summary>
        public readonly Dictionary<long, long> dicCDTime = new Dictionary<long, long>();

        /// <summary>
        /// 待清理的cd数据
        /// </summary>
        public readonly HashSet<long> listClearKeys = new HashSet<long>();
        public long Timer;
    }
}