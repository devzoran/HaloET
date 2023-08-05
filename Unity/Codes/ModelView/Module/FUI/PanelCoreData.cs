namespace ET
{
    [ComponentOf(typeof(FUIEntity))]
    public class PanelCoreData : Entity, IAwake
    {
        public UIPanelType panelType = UIPanelType.Normal;
    }
}