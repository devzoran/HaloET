using System;
using System.Collections.Generic;
using ET;
using FairyGUI.Utils;
using UnityEngine;

namespace FUIEditor
{
    public partial class ComponentInfo
    {
        public string NameSpace { get; private set; } = "";

        public string PackageId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
        
        public PanelType PanelType { get; set; }
        
        public string ComponentTypeName { get; private set; }

        public string NameWithoutExtension { get; set; }
        
        public ComponentType ComponentType { get; set; }

        public string ComponentClassName { get; private set; }

        public string Url { get; set; }
        
        // 编辑器里设置为导出
        public bool Exported { get; set; }

        public XMLList ControllerList  { get; } = new();
        
        public XMLList TransitionList  { get; } = new();

        public XMLList DisplayList { get; set; }

        // 最终是否需要导出类
        public bool NeedExportClass { get; private set; }
        
        public List<VariableInfo> VariableInfos { get; } = new();
        
        private bool HasCustomVariableName;

        public void CheckCanExport(HashSet<string> ExtralExportURLs, bool IgnoreDefaultVariableName)
        {
            // package.xml 中未设置导出，且不在额外导出列表中，那么不需要导出类
            bool needExportClass = !(!this.Exported && !ExtralExportURLs.Contains(this.Url));

            this.CollectVariables();

            if (this.PanelType != PanelType.None)
            {
                needExportClass = true;
            }
            else
            {
                // 如果没有变量，那么不需要导出类
                if (VariableInfos.Count == 0)
                {
                    needExportClass = false;
                }

                // 如果没有控制器，且忽略默认变量名，且没有自定义变量名，那么不需要导出类
                if (ControllerList.Count == 0 && IgnoreDefaultVariableName && !HasCustomVariableName)
                {
                    needExportClass = false;
                }
            }

            NeedExportClass = needExportClass;

            ComponentClassName = ComponentTypeToClassType[ComponentType];

            if (needExportClass)
            {
                NameSpace = "{0}.{1}".Fmt(FUICodeSpawner.NameSpace, FUICodeSpawner.PackageInfos[PackageId].Name);
                ComponentTypeName = "{0}{1}".Fmt(FUICodeSpawner.ClassNamePrefix, NameWithoutExtension);
            }
            else
            {
                ComponentTypeName = ComponentClassName;
            }
        }

        public void SetVariableInfoTypeName()
        {
            for (int index = 0; index < this.VariableInfos.Count; index++)
            {
                VariableInfo variableInfo = this.VariableInfos[index];
                variableInfo.TypeName = GetTypeNameByDisplayXML(this.PackageId, variableInfo.displayXML);

                string packageId = variableInfo.displayXML.GetAttribute("pkg");
                if (string.IsNullOrEmpty(packageId))
                {
                    packageId = this.PackageId;
                }
            
                string key = "{0}/{1}".Fmt(packageId, variableInfo.displayXML.GetAttribute("src"));

                if (!FUICodeSpawner.ComponentInfos.TryGetValue(key, out ComponentInfo componentInfo))
                {
                    continue;
                }

                if (componentInfo.PanelType == PanelType.Component)
                {
                    variableInfo.ComponentInfo = componentInfo;
                }
            }
        }
        
        private void CollectVariables()
        {
            if (DisplayList == null)
            {
                return;
            }
            
            foreach (XML displayXML in DisplayList)
            {
                string variableName = displayXML.GetAttribute("name");

                bool isAppointName = IsAppointName(variableName, ComponentType);

                bool isDefaultName = displayXML.GetAttribute("id").StartsWith(variableName);

                string packageId = displayXML.GetAttribute("pkg");
                // 如果没有设置包id，那么就是当前包
                if (string.IsNullOrEmpty(packageId))
                {
                    packageId = PackageId;
                }
                
                VariableInfos.Add(new VariableInfo()
                {
                    PackageId = packageId,
                    VariableName = variableName,
                    IsDefaultName = isDefaultName,
                    IsAppointName = isAppointName,
                    displayXML = displayXML,
                });

                if (!isDefaultName && !isAppointName)
                {
                    HasCustomVariableName = true;
                }
            }
        }
        
        /// <summary>
        /// 是否是 FairyGUI Editor 默认的节点名称
        /// </summary>
        private static bool IsAppointName(string variableName, ComponentType componentType)
        {
            if (variableName is "icon" or "text")
            {
                return true;
            }

            switch (componentType)
            {
                case ComponentType.Component:
                    return false;
                case ComponentType.Button: 
                case ComponentType.ComboBox:
                case ComponentType.Label:
                    if (variableName is "title" or "dragArea" or "closeButton")
                    {
                        return true;
                    }
                    break;
                case ComponentType.ProgressBar:
                    if (variableName is "bar" or "bar_v" or "ani")
                    {
                        return true;
                    }
                    break;
                case ComponentType.ScrollBar:
                    if (variableName is "arrow1" or "arrow2" or "grip" or "bar")
                    {
                        return true;
                    }
                    break;
                case ComponentType.Slider:
                    if (variableName is "bar" or "bar_v" or "grip" or "ani")
                    {
                        return true;
                    }
                    break;
                case ComponentType.None:
                    break;
                case ComponentType.Tree:
                    break;
                default:
                    throw new Exception("没有处理这种类型: {0}".Fmt(componentType));
            }

            return false;
        }
        
        private static string GetTypeNameByDisplayXML(string parentPackageId, XML displayXML)
        {
            string typeName = string.Empty;

            switch (displayXML.name)
            {
                case "component":
                {
                    string packageId = displayXML.GetAttribute("pkg");
                    if (string.IsNullOrEmpty(packageId))
                    {
                        packageId = parentPackageId;
                    }
                
                    string key = "{0}/{1}".Fmt(packageId, displayXML.GetAttribute("src"));
                    ComponentInfo displayComponentInfo = FUICodeSpawner.ComponentInfos[key];
                    if (displayComponentInfo == null)
                    {
                        throw new Exception("没找到对应类型：{0}".Fmt(displayXML.GetAttribute("src")));
                    }

                    typeName = string.IsNullOrEmpty(displayComponentInfo.NameSpace) ? 
                            displayComponentInfo.ComponentTypeName : 
                            "{0}.{1}".Fmt(displayComponentInfo.NameSpace, displayComponentInfo.ComponentTypeName);

                    break;
                }
                case "text":
                {
                    ObjectType objectType = displayXML.GetAttribute("input") == "true" ? 
                            ObjectType.textinput : 
                            ObjectType.textfield;
                    typeName = ObjectTypeToClassType[objectType];
                    break;
                }
                case "group" when displayXML.GetAttribute("advanced") != "true":
                    return typeName;
                case "group":
                {
                    ObjectType objectType = EnumHelper.FromString<ObjectType>(displayXML.name);
                    typeName = ObjectTypeToClassType[objectType];
                    break;
                }
                default:
                {
                    ObjectType objectType = EnumHelper.FromString<ObjectType>(displayXML.name);

                    try
                    {
                        typeName = ObjectTypeToClassType[objectType];
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"{objectType}没找到！");
                        Debug.LogError(e);
                    }

                    break;
                }
            }

            return typeName;
        }
    }
}