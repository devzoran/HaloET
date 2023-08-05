/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Login
{
	public partial class FUI_LoginPanel: GComponent
	{
		public ET.Common.FUI_TextInputUnderLine inputTextUsername;
		public ET.Common.FUI_TextInputUnderLine inputTextPassword;
		public ET.Common.FUI_GlobalButton1 btnLogin;
		public ET.Common.FUI_GlobalButton2 btnSignUp;
		public const string URL = "ui://q2mxp9ee10vfh0";

		public static FUI_LoginPanel CreateInstance()
		{
			return (FUI_LoginPanel)UIPackage.CreateObject("Login", "LoginPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			inputTextUsername = (ET.Common.FUI_TextInputUnderLine)GetChildAt(0);
			inputTextPassword = (ET.Common.FUI_TextInputUnderLine)GetChildAt(1);
			btnLogin = (ET.Common.FUI_GlobalButton1)GetChildAt(2);
			btnSignUp = (ET.Common.FUI_GlobalButton2)GetChildAt(3);
		}
	}
}
