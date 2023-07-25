using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    [ChildType(typeof(FUIEntity))]
    public class FUIComponent : Entity, IAwake, IDestroy
    {
        public static FUIComponent Instance;
        
        public List<PanelId> VisiblePanelsQueue = new List<PanelId>(10);
        
        public Dictionary<int, FUIEntity> AllPanelsDic = new Dictionary<int, FUIEntity>(10);
        
        public List<PanelId> FUIEntitylistCached = new List<PanelId>(10);
        
        public QueueDictionary<int, FUIEntity> VisiblePanelsDic = new QueueDictionary<int, FUIEntity>();
        
        public Stack<PanelId> HidePanelsStack = new Stack<PanelId>(10);

        // 每个 UIPakcage 对应的 Asset 地址。
        public MultiMap<string, string> UIPackageLocations = new MultiMap<string, string>();
    }
}