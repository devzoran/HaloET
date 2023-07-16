namespace ET
{
    public static class ConfirmPanelSystem
    {
        public static void Awake(this ConfirmPanel self)
        {
            
        }

        public static void RegisterUIEvent(this ConfirmPanel self)
        {
            self.Panel.title.text = "CONFIRM BOX";
            self.Panel.btnOk.AddListnerAsync(self.OnBtnOKClick);
            self.Panel.btnCancel.AddListnerAsync(self.OnBtnCancelClick);
        }
        
        public static void OnShow(this ConfirmPanel self, Entity contextData = null)
        {
            
        }

        public static void OnHide(this ConfirmPanel self)
        {

        }

        public static void BeforeUnload(this ConfirmPanel self)
        {

        }
        
        public static void OnRedPoint(this ConfirmPanel self, int systemId, bool show)
        {
			
        }

        private static async ETTask OnBtnOKClick(this ConfirmPanel self)
        {
            self.Panel.title.text = "OK Click~";
        }

        private static async ETTask OnBtnCancelClick(this ConfirmPanel self)
        {
            self.Panel.title.text = "Cancel Click~";
        }
    }
}