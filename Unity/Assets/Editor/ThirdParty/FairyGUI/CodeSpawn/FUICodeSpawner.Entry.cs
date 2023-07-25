using System;
using System.Collections.Generic;
using System.IO;
using ET;
using FairyGUI.Utils;
using UnityEditor;
using UnityEngine;

namespace FUIEditor
{
    public static partial class FUICodeSpawner
    {
        public static readonly Dictionary<string, PackageInfo> PackageInfos = new();

        public static readonly Dictionary<string, ComponentInfo> ComponentInfos = new();
        
        public static readonly List<ComponentInfo> MainPanelComponentInfos = new();
        
        public static readonly MultiDictionary<string, string, ComponentInfo> ExportedComponentInfos = new();

        private static readonly HashSet<string> ExtralExportURLs = new();

        public static void FUICodeParseAndSpawn()
        {
            ParseAllPackages();
            AfterParseAllPackages();
            SpawnCode();
            AssetDatabase.Refresh();
        }
        
        public static bool Localize(string xmlPath)
        {
            if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
            {
                Log.Error("没有提供语言文件！可查看此文档来生成：https://www.fairygui.com/docs/editor/i18n");
                return false;
            }
            
            FUILocalizeHandler.Localize(xmlPath);
            return true;
        }
        
    }
}











