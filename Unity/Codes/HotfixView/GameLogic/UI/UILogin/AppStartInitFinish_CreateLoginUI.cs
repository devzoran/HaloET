

namespace ET
{
	public class AppStartInitFinish_CreateLoginUI: AEventAsync<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			// UIHelper.Create(args.ZoneScene, UIType.UILogin, UILayer.Mid).Coroutine();
			CommonBinder.BindAll();
			FUIComponent fguiComponent = args.ZoneScene.GetComponent<FUIComponent>();
			await fguiComponent.ShowPanelAsync<LoginPanel>();
		}
	}
}
