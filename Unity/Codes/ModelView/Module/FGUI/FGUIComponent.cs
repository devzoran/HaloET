using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class FGUIComponent : Entity, IAwake, IDestroy
    {
        public static FGUIComponent Instance;
        
        public List<PanelId> VisiblePanelsQueue = new List<PanelId>(10);
        
        public Dictionary<int, FGUIEntity> AllPanelsDic = new Dictionary<int, FGUIEntity>(10);
        
        public List<PanelId> FGUIEntitylistCached = new List<PanelId>(10);
        
        public QueueDictionary<int, FGUIEntity> VisiblePanelsDic = new QueueDictionary<int, FGUIEntity>();
        
        public Stack<PanelId> HidePanelsStack = new Stack<PanelId>(10);

        // 每个 UIPakcage 对应的 Asset 地址。
        public MultiMap<string, string> UIPackageLocations = new MultiMap<string, string>();
    }
}