/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Pkg_Confirm
{
    public partial class FGUI_ConfirmPanel : GComponent
    {
        public GTextField title;
        public GTextField content;
        public GButton btnOk;
        public GButton btnCancel;
        public const string URL = "ui://4aaf6sltqgkn1";

        public static FGUI_ConfirmPanel CreateInstance()
        {
            return (FGUI_ConfirmPanel)UIPackage.CreateObject("Pkg_Confirm", "ConfirmPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            title = (GTextField)GetChildAt(2);
            content = (GTextField)GetChildAt(3);
            btnOk = (GButton)GetChildAt(4);
            btnCancel = (GButton)GetChildAt(5);
        }
    }
}