using System;
using UnityEngine;

namespace ET
{
	[FriendClass(typeof(PanelCoreData))]
	[FriendClass(typeof(FUIEntity))]
	[FUIEvent(PanelId.ConfirmPanel, "Confirm", "ConfirmPanel")]
	public class ConfirmPanelEventHandler: IFUIEventHandler
	{
		public void OnInitPanelCoreData(FUIEntity fuiEntity)
		{
			fuiEntity.PanelCoreData.panelType = UIPanelType.Normal;
		}

		public void OnInitComponent(FUIEntity fuiEntity)
		{
			ConfirmPanel panel = fuiEntity.AddComponent<ConfirmPanel>();
			panel.Awake();
		}

		public void OnRegisterUIEvent(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<ConfirmPanel>().RegisterUIEvent();
		}

		public void OnShow(FUIEntity fuiEntity, Entity contextData = null)
		{
			fuiEntity.GetComponent<ConfirmPanel>().OnShow(contextData);
		}

		public void OnHide(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<ConfirmPanel>().OnHide();
		}

		public void BeforeUnload(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<ConfirmPanel>().BeforeUnload();
		}

		public void OnRedPoint(FUIEntity fuiEntity, int systemId, bool show)
		{

		}
	}
}