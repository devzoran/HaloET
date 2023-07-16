namespace ET
{
    public interface IFGUIEventHandler
    {
        /// <summary>
        /// UI实体加载后,初始化窗口数据
        /// </summary>
        /// <param name="fguiEntity"></param>
        void OnInitPanelCoreData(FGUIEntity fguiEntity);
        
        /// <summary>
        /// UI实体加载后，初始化业务逻辑数据
        /// </summary>
        /// <param name="fguiEntity"></param>
        void OnInitComponent(FGUIEntity fguiEntity);
        
        /// <summary>
        /// 注册UI业务逻辑事件
        /// </summary>
        /// <param name="fguiEntity"></param>
        void OnRegisterUIEvent(FGUIEntity fguiEntity);

        /// <summary>
        /// 打开UI窗口的业务逻辑
        /// </summary>
        /// <param name="fguiEntity"></param>
        /// <param name="contextData"></param>
        void OnShow(FGUIEntity fguiEntity, Entity contextData = null);
        
        /// <summary>
        /// 隐藏UI窗口的业务逻辑
        /// </summary>
        /// <param name="fguiEntity"></param>
        void OnHide(FGUIEntity fguiEntity);

        /// <summary>
        /// 完全关闭销毁UI窗口之前的业务逻辑，用于完全释放UI相关对象
        /// </summary>
        /// <param name="fguiEntity"></param>
        void BeforeUnload(FGUIEntity fguiEntity);


        /// <summary>
        /// 当显示红点时
        /// </summary>
        void OnRedPoint(FGUIEntity fguiEntity, int systemId, bool show);
    }
}