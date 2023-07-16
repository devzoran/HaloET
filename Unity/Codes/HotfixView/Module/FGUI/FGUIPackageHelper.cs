using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(FGUIComponent))]
    public static class UIPackageHelper
    {
        public static async ETTask<GComponent> CreateObjectAsync(string packageName, string componentName)
        {
            ETTask<GComponent> task = ETTask<GComponent>.Create(true);
            UIPackage.CreateObjectAsync(packageName, componentName, result =>
            {
                task.SetResult(result.asCom);
            });
            return await task;
        }

        /// <summary>
        /// 增加 FariyGUI 的 Package
        /// </summary>
        /// <param name="self"></param>
        /// <param name="packageName"></param>
        public static async ETTask AddPackageAsync(this FGUIComponent self, string packageName)
        {
            // Temp
            string abName = packageName.ToLower().Replace("pkg_", "fgui") + ".unity3d";
            (AssetBundle assetsBundle, Dictionary<string, UnityEngine.Object> dictionary) = AssetsBundleHelper.LoadBundle(abName);
            string dataName = packageName + "_fui";
            byte[] descData = ((TextAsset)dictionary[dataName]).bytes;
            // TODO:
            /*string dataName = $"{packageName}_fui";
            byte[] descData = await ResComponent.Instance.LoadRawFileDataAsync(dataName);*/
            UIPackage.AddPackage(descData, packageName, (name, extension, type, item) =>
            {
                self.InnerLoader(name, extension, type, item).Coroutine();
            });
            self.UIPackageLocations.Add(packageName, dataName);
        }

        private static async ETTask InnerLoader(this FGUIComponent self, string location, string extension, Type type, PackageItem item)
        {
            //TODO：
            /*UnityEngine.Object res = await ResComponent.Instance.LoadAssetAsync("{0}".Fmt(location), type);
            item.owner.SetItemAsset(item, res, DestroyMethod.None);

            string packageName = item.owner.name;
            self.UIPackageLocations.Add(packageName, location);*/
        }

        /// <summary>
        /// 移除 FariyGUI 的 Package
        /// </summary>
        /// <param name="self"></param>
        /// <param name="packageName"></param>
        public static void RemovePackage(this FGUIComponent self, string packageName)
        {
            //TODO：
            /*UIPackage.RemovePackage(packageName);

            List<string> list = self.UIPackageLocations[packageName];
            foreach (string location in list)
            {
                ResComponent.Instance.UnloadAsset(location);
            }

            self.UIPackageLocations.Remove(packageName);*/
        }
    }
}