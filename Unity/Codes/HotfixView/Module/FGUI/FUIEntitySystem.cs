using FairyGUI;

namespace ET
{
    [ObjectSystem]
    public class FUIEntityAwakeSystem : AwakeSystem<FUIEntity, ShowPanelData>
    {
        public override void Awake(FUIEntity self, ShowPanelData data)
        {
            self.AddComponent(data);
        }
    }
    
    public static class FUIEntitySystem
    {
        public static void SetRoot(this FUIEntity self, GComponent rootGComponent)
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