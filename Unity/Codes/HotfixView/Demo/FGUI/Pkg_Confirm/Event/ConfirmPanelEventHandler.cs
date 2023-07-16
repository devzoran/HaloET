namespace ET
{
    [FriendClass(typeof (FGUIEntity))]
    [FGUIEvent(PanelId.ConfirmPanel, "Pkg_Confirm", "ConfirmPanel")]
    public class ConfirmPanelEventHandler : IFGUIEventHandler
    {
        public void OnInitPanelCoreData(FGUIEntity fguiEntity)
        {
            fguiEntity.panelType = UIPanelType.Normal;
        }

        public void OnInitComponent(FGUIEntity fguiEntity)
        {
            fguiEntity.AddComponent<ConfirmPanel>().Awake();
        }

        public void OnRegisterUIEvent(FGUIEntity fguiEntity)
        {
            fguiEntity.GetComponent<ConfirmPanel>().RegisterUIEvent();
        }

        public void OnShow(FGUIEntity fguiEntity, Entity contextData = null)
        {
            fguiEntity.GetComponent<ConfirmPanel>().OnShow(contextData);
        }

        public void OnHide(FGUIEntity fguiEntity)
        {
            fguiEntity.GetComponent<ConfirmPanel>().OnHide();
        }

        public void BeforeUnload(FGUIEntity fguiEntity)
        {
            fguiEntity.GetComponent<ConfirmPanel>().BeforeUnload();
        }

        public void OnRedPoint(FGUIEntity fguiEntity, int systemId, bool show)
        {
            fguiEntity.GetComponent<ConfirmPanel>().OnRedPoint(systemId, show);
        }
    }
}