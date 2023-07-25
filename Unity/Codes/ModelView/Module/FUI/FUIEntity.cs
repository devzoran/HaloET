using FairyGUI;

namespace ET
{
    // [EnableMethod]
    [ChildType(typeof(FUIComponent))]
    public class FUIEntity : Entity, IAwake, IAwake<ShowPanelData>
    {
        public bool IsPreLoad
        {
            get
            {
                return this.GComponent != null;
            }
        }
        
        public PanelId PanelId
        {
            get
            {
                if (this.panelId == PanelId.Invalid)
                {
                    Log.Error("panel id is " + PanelId.Invalid);
                }
                return this.panelId;
            }
            set { this.panelId = value; }
        }
      
        private PanelId panelId = PanelId.Invalid;
        
        public UIPanelType panelType = UIPanelType.Normal;

        public GComponent GComponent { get; set; }
        
        public PanelCoreData PanelCoreData { get; set; }
    }
}