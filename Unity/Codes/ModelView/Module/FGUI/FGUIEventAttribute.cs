using System;

namespace ET
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FGUIEventAttribute : BaseAttribute
    {
        public PanelId PanelId
        {
            get;
        }

        public PanelInfo PanelInfo
        {
            get;
        }

        public FGUIEventAttribute(PanelId panelId, string packageName, string componentName)
        {
            this.PanelId = panelId;
            this.PanelInfo = new PanelInfo() { PanelId = panelId, PackageName = packageName, ComponentName = componentName };
        }
    }
}