using FairyGUI;

namespace ET
{
    [ObjectSystem]
    public class FUIEntityAwakeSystem : AwakeSystem<FUIEntity>
    {
        public override void Awake(FUIEntity self)
        {
            self.PanelCoreData = self.AddComponent<PanelCoreData>();
        }
    }
    
    [ObjectSystem]
    public class FUIEntityDestroySystem : DestroySystem<FUIEntity>
    {
        public override void Destroy(FUIEntity self)
        {
            self.PanelCoreData?.Dispose();
            self.PanelId = PanelId.Invalid;
            if (self.GComponent != null)
            {
                self.GComponent.Dispose();
                self.GComponent = null;
            }
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