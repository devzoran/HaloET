/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Common
{
	public partial class FUI_TextInputUnderLine: GComponent
	{
		public GTextInput inputText;
		public const string URL = "ui://1zi7ydboo73h2";

		public static FUI_TextInputUnderLine CreateInstance()
		{
			return (FUI_TextInputUnderLine)UIPackage.CreateObject("Common", "TextInputUnderLine");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			inputText = (GTextInput)GetChildAt(0);
		}
	}
}
