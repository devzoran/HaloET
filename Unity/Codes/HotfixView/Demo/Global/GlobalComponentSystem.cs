using FairyGUI;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class GlobalComponentAwakeSystem: AwakeSystem<GlobalComponent>
    {
        public override void Awake(GlobalComponent self)
        {
            GlobalComponent.Instance = self;
            
            self.Global = GameObject.Find("/Global").transform;
            self.Unit = GameObject.Find("/Global/Unit").transform;
            // self.UI = GameObject.Find("/Global/UI").transform;
            
            #region FGUI
            
            self.GRoot = GRoot.inst;
            
            self.BottomGRoot = new GComponent();
            self.BottomGRoot.gameObjectName = "BottomGRoot";
            GRoot.inst.AddChild(self.BottomGRoot);

            self.NormalGRoot = new GComponent();
            self.NormalGRoot.gameObjectName = "NormalGRoot";
            GRoot.inst.AddChild(self.NormalGRoot);
            
            self.SecondGRoot = new GComponent();
            self.SecondGRoot.gameObjectName = "SecondGRoot";
            GRoot.inst.AddChild(self.SecondGRoot);

            self.PopUpGRoot = new GComponent();
            self.PopUpGRoot.gameObjectName = "PopUpGRoot";
            GRoot.inst.AddChild(self.PopUpGRoot);
            
            self.FixedGRoot = new GComponent();
            self.FixedGRoot.gameObjectName = "FixedGRoot";
            GRoot.inst.AddChild(self.FixedGRoot);
            
            self.OtherGRoot = new GComponent();
            self.OtherGRoot.gameObjectName = "OtherGRoot";
            GRoot.inst.AddChild(self.OtherGRoot);
            
            #endregion
        }
    }
}