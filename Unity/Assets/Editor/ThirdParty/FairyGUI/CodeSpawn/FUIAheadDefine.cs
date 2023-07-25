/*
 * FUIAheadDefine.cs
 * 此文件添加需要动态维护的相关数据和类型定义
 */
using System.Collections.Generic;

namespace FUIEditor
{
    public static partial class FUICodeSpawner
    {
        // 名字空间
        public const string NameSpace = "ET";

        // 类名前缀
        public const string ClassNamePrefix = "FUI_";

        // 代码生成路径
        public const string FUIAutoGenDir = "../Unity/Codes/ModelView/GameLogic/FUIAutoGen";
        public const string ModelViewCodeDir = "../Unity/Codes/ModelView/GameLogic/FUI";
        public const string HotfixViewCodeDir = "../Unity/Codes/HotfixView/GameLogic/FUI";
        
        // 不生成使用默认名称的成员
        public const bool IgnoreDefaultVariableName = true;
    }
    
    public partial class ComponentInfo
    {
        private static readonly Dictionary<ComponentType, string> ComponentTypeToClassType = new()
        {
            {ComponentType.Component, "GComponent"},
            {ComponentType.Button, "GButton"},
            {ComponentType.ComboBox, "GComboBox"},
            {ComponentType.Label, "GLabel"},
            {ComponentType.ProgressBar, "GProgressBar"},
            {ComponentType.ScrollBar, "GScrollBar"},
            {ComponentType.Slider, "GSlider"},
            {ComponentType.Tree, "GTree"}
        };
        
        private static readonly Dictionary<ObjectType, string> ObjectTypeToClassType = new()
        {
            {ObjectType.graph, "GGraph"},
            {ObjectType.group, "GGroup"},
            {ObjectType.image, "GImage"},
            {ObjectType.loader, "GLoader"},
            {ObjectType.loader3D, "GLoader3D"},
            {ObjectType.movieclip, "GMovieClip"},
            {ObjectType.textfield, "GTextField"},
            {ObjectType.textinput, "GTextInput"},
            {ObjectType.richtext, "GRichTextField"},
            {ObjectType.list, "GList"}
        };
    }
    
    public enum PanelType
    {
        None,
        Panel,
        Component
    }
    
    public enum ComponentType
    {
        None,
        Component,
        Button,
        ComboBox, // 下拉框
        Label,
        ProgressBar,
        ScrollBar,
        Slider,
        Tree
    }
    
    public enum ObjectType
    {
        None,
        graph,
        group,
        image,
        loader,
        loader3D,
        movieclip,
        textfield,
        textinput,
        richtext,
        list
    }
}