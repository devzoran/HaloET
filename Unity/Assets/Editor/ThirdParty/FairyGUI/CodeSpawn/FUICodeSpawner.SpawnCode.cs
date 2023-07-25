using System.Collections.Generic;
using System.IO;

namespace FUIEditor
{
    public static partial class FUICodeSpawner
    {
        private static void SpawnCode()
        {
            if (Directory.Exists(FUIAutoGenDir)) 
            {
                Directory.Delete(FUIAutoGenDir, true);
            }
            
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                FUIComponentSpawner.SpawnComponent(componentInfo);
            }
            
            List<PackageInfo> ExportedPackageInfos = new List<PackageInfo>();
            foreach (var kv in ExportedComponentInfos)
            {
                FUIBinderSpawner.SpawnCodeForPanelBinder(PackageInfos[kv.Key], kv.Value);
                
                ExportedPackageInfos.Add(PackageInfos[kv.Key]);
            }

            FUIPanelIdSpawner.SpawnPanelId();
            //TODO: 暂时未用到的方式不作导出生成
            // FUIPackageHelperSpawner.GenerateMappingFile();
            // FUIBinderSpawner.SpawnFUIBinder(ExportedPackageInfos);

            foreach ((string _, ComponentInfo componentInfo) in ComponentInfos)
            {
                if (componentInfo.PanelType != PanelType.Panel)
                {
                    continue;
                }

                PackageInfo packageInfo = PackageInfos[componentInfo.PackageId];
                    
                SpawnSubPanelCode(componentInfo);

                FUIPanelSpawner.SpawnPanel(packageInfo.Name, componentInfo);
                        
                FUIPanelSystemSpawner.SpawnPanelSystem(packageInfo.Name, componentInfo);
                        
                FUIEventHandlerSpawner.SpawnEventHandler(packageInfo.Name, componentInfo);
            }
        }
        
        private static void SpawnSubPanelCode(ComponentInfo componentInfo)
        {
            componentInfo.VariableInfos.ForEach(variableInfo =>
            {
                if (!variableInfo.IsExported || variableInfo.ComponentInfo == null)
                {
                    return;
                }
                
                string subPackageName = PackageInfos[variableInfo.PackageId].Name;

                FUIPanelSpawner.SpawnSubPanel(subPackageName, variableInfo.ComponentInfo);
                FUIPanelSystemSpawner.SpawnPanelSystem(subPackageName, variableInfo.ComponentInfo, variableInfo);
                
                SpawnSubPanelCode(variableInfo.ComponentInfo);
            });
        }
    }
}