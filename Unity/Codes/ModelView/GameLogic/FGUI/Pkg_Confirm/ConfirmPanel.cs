using ET.Pkg_Confirm;
using FairyGUI;

namespace ET
{
    [ComponentOf(typeof(FGUIEntity))]
    public class ConfirmPanel : Entity, IAwake
    {
        private FGUI_ConfirmPanel _panel;

        public FGUI_ConfirmPanel Panel
        {
            get => this._panel ??= (FGUI_ConfirmPanel)this.GetParent<FGUIEntity>().GComponent;
        }
    }
}