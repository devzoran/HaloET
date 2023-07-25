using System;
using System.IO;
using ET;
using FairyGUI.Utils;
using UnityEngine;

namespace FUIEditor
{
    public static partial class FUICodeSpawner
    {
        private static void ParseAllPackages()
        {
            PackageInfos.Clear();
            ComponentInfos.Clear();
            MainPanelComponentInfos.Clear();
            ExportedComponentInfos.Clear();
            ExtralExportURLs.Clear();

            string fuiAssetsDir = Application.dataPath + "/../../FGUIProject/assets";
            string[] packageDirs = Directory.GetDirectories(fuiAssetsDir);
            foreach (string packageDir in packageDirs)
            {
                PackageInfo packageInfo = ParsePackage(packageDir);
                if (packageInfo == null)
                {
                    continue;
                }
                
                PackageInfos.Add(packageInfo.Id, packageInfo);
            }
        }
        
        private static PackageInfo ParsePackage(string packageDir)
        {
            string packageXmlPath = $"{packageDir}/package.xml";
            if (!File.Exists(packageXmlPath))
            {
                Log.Warning($"{packageXmlPath} 不存在！");
                return null;
            }
            
            XML xml = new(File.ReadAllText(packageDir + "/package.xml"));
            PackageInfo packageInfo = new()
            {
                Id = xml.GetAttribute("id"),
                Name = Path.GetFileName(packageDir),
                Path = packageDir,
            };

            if (xml.elements[0].name != "resources")
            {
                throw new Exception("package.xml 格式异常！");
            }
            
            foreach (XML element in xml.elements[0].elements)
            {
                if (element.name != "component")
                {
                    continue;
                }
                
                string componentId = element.GetAttribute("id");
                string componentName = element.GetAttribute("name");
                string componentPath = element.GetAttribute("path");
                string componentExported = element.GetAttribute("exported");
                PackageComponentInfo packageComponentInfo = new()
                {
                    Id = componentId,
                    Name = componentName,
                    Path = "{0}{1}{2}".Fmt(packageDir, componentPath, componentName),
                    Exported = componentExported == "true",
                };
                
                packageInfo.PackageComponentInfos.Add(packageComponentInfo.Name, packageComponentInfo);

                ComponentInfo componentInfo = ParseComponent(packageInfo, packageComponentInfo);
                string key = "{0}/{1}".Fmt(componentInfo.PackageId, componentInfo.Id);
                ComponentInfos.Add(key, componentInfo);

                if (componentInfo.PanelType == PanelType.Panel)
                {
                    MainPanelComponentInfos.Add(componentInfo);    
                }
            }

            return packageInfo;
        }
        
        private static ComponentInfo ParseComponent(PackageInfo packageInfo, PackageComponentInfo packageComponentInfo)
        {
            ComponentInfo componentInfo = new()
            {
                PackageId = packageInfo.Id,
                Id = packageComponentInfo.Id,
                Name = packageComponentInfo.Name,
                NameWithoutExtension = Path.GetFileNameWithoutExtension(packageComponentInfo.Name),
                ComponentType = ComponentType.Component,
                Url = "ui://{0}{1}".Fmt(packageInfo.Id, packageComponentInfo.Id),
                Exported = packageComponentInfo.Exported,
            };

            XML xml = new(File.ReadAllText(packageComponentInfo.Path));

            if (xml.attributes.TryGetValue("extention", out string typeName))
            {
                ComponentType type = EnumHelper.FromString<ComponentType>(typeName);
                if (type == ComponentType.None)
                {
                    Debug.LogError("{0}类型没有处理！".Fmt(typeName));
                }
                else
                {
                    componentInfo.ComponentType = type;
                }
            }
            else if (xml.attributes.TryGetValue("remark", out string remark))
            {
                if (Enum.TryParse(remark, out PanelType panelType))
                {
                    componentInfo.PanelType = panelType;
                }
            }

            foreach (XML element in xml.elements)
            {
                switch (element.name)
                {
                    case "displayList":
                        componentInfo.DisplayList = element.elements;
                        break;
                    case "controller":
                        componentInfo.ControllerList.Add(element);
                        break;
                    case "transition":
                        componentInfo.TransitionList.Add(element);
                        break;
                    case "relation":
                        break;
                    case "customProperty":
                        break;
                    case "ComboBox" when componentInfo.ComponentType == ComponentType.ComboBox:
                        ExtralExportURLs.Add(element.GetAttribute("dropdown"));
                        break;
                }
            }

            return componentInfo;
        }
        
    }   
}