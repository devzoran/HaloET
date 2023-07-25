--FYI: https://github.com/Tencent/xLua/blob/master/Assets/XLua/Doc/XLua_Tutorial_EN.md

fprint('Active Panel Type Inspector Plugin.')

-- function onDestroy()
-- -------do cleanup here-------
-- end

local inspector = {};
function inspector.create()
    fprint('Panel Type Inspector create')
    inspector.panel = CS.FairyGUI.UIPackage.CreateObject("PanelTypeInspector", "Inspector")
    inspector.combo = inspector.panel:GetChild("comboBox")
    inspector.combo.onChanged:Add(function()
        local obj = App.activeDoc.inspectingTarget
        --use obj.docElement:SetProperty('xxx',..) instead of obj.xxx = ... to enable undo/redo mechanism
        obj.docElement:SetProperty("remark", inspector.combo.value)
    end)
    return inspector.panel
end

function inspector.updateUI()
    fprint('Panel Type Inspector create')
    local sels = App.activeDoc.inspectingTargets
    local obj = sels[0]

    inspector.combo.value = obj.remark

    --return true if everything is ok, return false to hide the inspector
    return true
end

--Register a inspector
App.inspectorView:AddInspector(inspector, "PanelTypeInspector", "PanelTypeInspector");
--Condition to show it
App.docFactory:ConnectInspector("PanelTypeInspector", "mixed", true, false);

App.pluginManager:LoadUIPackage(PluginPath..'/PanelTypeInspector')