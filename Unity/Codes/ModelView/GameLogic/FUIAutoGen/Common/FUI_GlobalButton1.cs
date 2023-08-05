/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Common
{
	public partial class FUI_GlobalButton1: GButton
	{
		public GGraph n1;
		public const string URL = "ui://1zi7ydboo73h0";

		public static FUI_GlobalButton1 CreateInstance()
		{
			return (FUI_GlobalButton1)UIPackage.CreateObject("Common", "GlobalButton1");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n1 = (GGraph)GetChildAt(1);
		}
	}
}
