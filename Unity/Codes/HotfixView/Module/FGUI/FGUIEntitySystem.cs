using FairyGUI;

namespace ET
{
    [ObjectSystem]
    public class FUIEntityAwakeSystem : AwakeSystem<FGUIEntity, ShowPanelData>
    {
        public override void Awake(FGUIEntity self, ShowPanelData data)
        {
            self.AddComponent(data);
        }
    }
    
    public static class FGUIEntitySystem
    {
        public static void SetRoot(this FGUIEntity self, GComponent rootGComponent)
        {
            if(self.GComponent == null)
            {
                Log.Error($"FUIEntity {self.PanelId} GComponent is null!!!");
                return;
            }
            if(rootGComponent == null)
            {
                Log.Error($"FUIEntity {self.PanelId} rootGComponent is null!!!");
                return;
            }
            rootGComponent.AddChild(self.GComponent);
        }
    }
}