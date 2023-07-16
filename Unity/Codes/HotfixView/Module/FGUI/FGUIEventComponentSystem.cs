using System;

namespace ET
{
    [ObjectSystem]
    public class FGUIEventComponentAwakeSystem : AwakeSystem<FGUIEventComponent>
    {
        public override void Awake(FGUIEventComponent self)
        {
            FGUIEventComponent.Instance = self;
            self.Awake();
        }
    }
    
    [ObjectSystem]
    public class FGUIEventComponentDestroySystem : DestroySystem<FGUIEventComponent>
    {
        public override void Destroy(FGUIEventComponent self)
        {
            self.UIEventHandlers.Clear();
            self.PanelIdInfoDict.Clear();
            self.PanelTypeInfoDict.Clear();
            self.isClicked = false;
            FGUIEventComponent.Instance = null;
        }
    }
    
    [FriendClass(typeof(FGUIEventComponent))]
    public static class FGUIEventComponentSystem
    {
        public static void Awake(this FGUIEventComponent self)
        {
            self.UIEventHandlers.Clear();
            foreach (Type v in EventSystem.Instance.GetTypes(typeof(FGUIEventAttribute)))
            {
                FGUIEventAttribute attr = v.GetCustomAttributes(typeof(FGUIEventAttribute), false)[0] as FGUIEventAttribute;
                self.UIEventHandlers.Add(attr.PanelId, Activator.CreateInstance(v) as IFGUIEventHandler);
                self.PanelIdInfoDict.Add(attr.PanelId, attr.PanelInfo);
                self.PanelTypeInfoDict.Add(attr.PanelId.ToString(), attr.PanelInfo);
            }
        }
        
        public static IFGUIEventHandler GetUIEventHandler(this FGUIEventComponent self, PanelId panelId)
        {
            if (self.UIEventHandlers.TryGetValue(panelId, out IFGUIEventHandler handler))
            {
                return handler;
            }
            Log.Error($"panelId : {panelId} is not have any uiEvent");
            return null;
        }

        public static PanelInfo GetPanelInfo(this FGUIEventComponent self, PanelId panelId)
        {
            if (self.PanelIdInfoDict.TryGetValue(panelId, out PanelInfo panelInfo))
            {
                return panelInfo;
            }
            Log.Error($"panelId : {panelId} is not have any panelInfo");
            return default;
        }
    }
}