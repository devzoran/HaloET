using FairyGUI;

namespace ET
{
    //FGUI 3DLoader显示模型/预制的管理组件
    [ComponentOf]
    public class FGUIPrefabLoaderComponent: Entity, IAwake, IAwake<GGraph, string>, IDestroy
    {
        public string AssetLocationName;
        public GoWrapper Wrapper;
        public GGraph Graph;
    }
}