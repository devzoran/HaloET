using ET.Confirm;

namespace ET
{
	[ComponentOf(typeof(FUIEntity))]
	public class ConfirmPanel: Entity, IAwake
	{
		private FUI_ConfirmPanel _fuiConfirmPanel;

		public FUI_ConfirmPanel FUIConfirmPanel
		{
			get => _fuiConfirmPanel ??= (FUI_ConfirmPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
