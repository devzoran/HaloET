using System.Collections.Generic;

namespace ET
{

    public class CDSystem : Singleton<CDSystem>
    {
        public readonly Dictionary<long, CDComponent> dicCDComponents = new Dictionary<long, CDComponent>();


        /// <summary>
        /// 获取/创建一个独立的CDComponent,用于单个模块/角色的CD统计  
        /// 备注: id == 0 表示随机ID
        /// 备注: id == -1 表示全局CD系统
        /// </summary>
        /// <param name="id"></param>
        public CDComponent GetCDComponent(long id = 0)
        {
            if (id == 0) id = IdGenerater.Instance.GenerateId();

            if (!dicCDComponents.TryGetValue(id, out var cdComponent))
            {
                cdComponent = Game.Scene.AddChildWithId<CDComponent>(id);
                dicCDComponents.Add(id, cdComponent);
            }

            return cdComponent;
        }

        /// <summary>
        /// 移除CDComponent 
        /// (等其中逻辑执行完之后才会回收)
        /// </summary>
        /// <param name="id"></param>
        public void Remove(long id)
        {
            dicCDComponents.Remove(id);
        }
    }
}