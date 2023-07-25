namespace ET
{
    public enum UIPanelType
    {
        /// <summary>
        /// 底层界面 (可作为背景用) 不会加入自动关闭堆栈
        /// </summary>
        Bottom,
      
        /// <summary>
        /// 主界面 (主页签)
        /// </summary>
        Normal,
      
        /// <summary>
        /// 二级界面
        /// </summary>
        SecondPanel,
      
        /// <summary>
        /// 弹出窗口
        /// </summary>
        PopUp,

        /// <summary>
        /// 固定窗口 (永不销毁/关闭) 不会加入自动关闭堆栈
        /// </summary>
        Fixed,
      
        /// <summary>
        /// 其他窗口 不会加入自动关闭堆栈
        /// </summary>
        Other,
    }
    
    [ComponentOf]
    public class ShowPanelData: Entity, IAwake
    {
        public Entity ContextData { get; set; }
    }
    
    /// <summary>
    /// 用于FUI 界面跳转时传递具体的界面信息
    /// </summary>
    public class UIParam: Entity, IAwake
    {
        public int ParamInt { get; set; }
        public long ParamLong { get; set; }
        public string ParamString { get; set; }
        public Entity ParamEntity { get; set; }
    }
}