﻿using System.Collections.Generic;

namespace ET
{
    public struct PanelInfo
    {
        public PanelId PanelId;
    
        public string PackageName;
    
        public string ComponentName;
    }
    
    [ComponentOf(typeof(Scene))]
    public class FUIEventComponent : Entity, IAwake, IDestroy
    {
        public static FUIEventComponent Instance { get; set; }
        public CDComponent FGUICD => this.AddOrGetComponent<CDComponent>();
        
        public readonly Dictionary<PanelId, IFUIEventHandler> UIEventHandlers = new();
        public readonly Dictionary<PanelId, PanelInfo> PanelIdInfoDict = new();
        public readonly Dictionary<string, PanelInfo> PanelTypeInfoDict = new();

        public bool isClicked { get; set; }
    }
}