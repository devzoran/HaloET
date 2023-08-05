/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Common
{
	public partial class FUI_GlobalButton2: GButton
	{
		public GGraph n1;
		public const string URL = "ui://1zi7ydboo73h1";

		public static FUI_GlobalButton2 CreateInstance()
		{
			return (FUI_GlobalButton2)UIPackage.CreateObject("Common", "GlobalButton2");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n1 = (GGraph)GetChildAt(1);
		}
	}
}
