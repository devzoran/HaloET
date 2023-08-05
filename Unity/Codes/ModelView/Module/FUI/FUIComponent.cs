using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    [ChildType(typeof(FUIEntity))]
    public class FUIComponent : Entity, IAwake, IDestroy
    {
        public List<PanelId> VisiblePanelsQueue = new(10);
        
        public Dictionary<int, FUIEntity> AllPanelsDic = new(10);
        
        public List<PanelId> FUIEntitylistCached = new(10);
        
        public Dictionary<int, FUIEntity> VisiblePanelsDic = new();
        
        public Stack<PanelId> HidePanelsStack = new(10);

        // 每个 UIPakcage 对应的 Asset 地址。
        public MultiMap<string, string> UIPackageLocations = new();
    }
}