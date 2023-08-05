/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Confirm
{
	public partial class FUI_ConfirmPanel: GComponent
	{
		public GGraph n0;
		public GGraph n2;
		public GTextField title;
		public GGraph n4;
		public GTextField content;
		public GGroup n3;
		public ET.Confirm.FUI_Btn btnOk;
		public ET.Confirm.FUI_Btn btnCancel;
		public GGroup n5;
		public const string URL = "ui://4aaf6sltqgkn1";

		public static FUI_ConfirmPanel CreateInstance()
		{
			return (FUI_ConfirmPanel)UIPackage.CreateObject("Confirm", "ConfirmPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n0 = (GGraph)GetChildAt(0);
			n2 = (GGraph)GetChildAt(1);
			title = (GTextField)GetChildAt(2);
			n4 = (GGraph)GetChildAt(4);
			content = (GTextField)GetChildAt(5);
			n3 = (GGroup)GetChildAt(6);
			btnOk = (ET.Confirm.FUI_Btn)GetChildAt(7);
			btnCancel = (ET.Confirm.FUI_Btn)GetChildAt(8);
			n5 = (GGroup)GetChildAt(9);
		}
	}
}
