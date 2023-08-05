namespace ET
{
	[FriendClass(typeof(LoginPanel))]
	public static class LoginPanelSystem
	{
		public static void Awake(this LoginPanel self)
		{
		}

		public static void RegisterUIEvent(this LoginPanel self)
		{
			self.FUILoginPanel.btnLogin.AddListnerAsync(self.OnBtnLoginHandler);
			self.FUILoginPanel.btnSignUp.AddListnerAsync(self.OnBtnSignUpHandler);
		}

		public static void OnShow(this LoginPanel self, Entity contextData = null)
		{
		}

		public static void OnHide(this LoginPanel self)
		{
		}

		public static void BeforeUnload(this LoginPanel self)
		{
		}
		
		private static async ETTask OnBtnLoginHandler(this LoginPanel self)
		{
			self.FUILoginPanel.inputTextUsername.inputText.text = "Login Button Clicked";
		}
		
		private static async ETTask OnBtnSignUpHandler(this LoginPanel self)
		{
			self.FUILoginPanel.inputTextUsername.inputText.text = "SignUp Button Clicked";
		}
	}
}