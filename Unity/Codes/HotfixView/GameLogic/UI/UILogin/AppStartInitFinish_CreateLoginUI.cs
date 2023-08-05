

namespace ET
{
	public class AppStartInitFinish_CreateLoginUI: AEventAsync<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			// UIHelper.Create(args.ZoneScene, UIType.UILogin, UILayer.Mid).Coroutine();
			FUIComponent fguiComponent = args.ZoneScene.GetComponent<FUIComponent>();
			
			await fguiComponent.LoadPkg("Common");
			await fguiComponent.ShowPanelAsync<Entity>(PanelId.LoginPanel);
		}
	}
}
