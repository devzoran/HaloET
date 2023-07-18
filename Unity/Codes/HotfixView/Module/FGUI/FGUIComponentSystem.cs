using System;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class FGUIComponentAwakeSystem: AwakeSystem<FGUIComponent>
    {
        public override void Awake(FGUIComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class FGUIComponentDestroySystem: DestroySystem<FGUIComponent>
    {
        public override void Destroy(FGUIComponent self)
        {
            self.Destroy();
        }
    }
    
    [FriendClass(typeof (FGUIEventComponent))]
    [FriendClass(typeof (FGUIEntity))]
    [FriendClass(typeof (FGUIComponent))]
    public static class FGUIComponentSystem
    {
        public static void Awake(this FGUIComponent self)
        {
            FGUIComponent.Instance = self;
            // self.AddComponent<CDComponent>();

            //初始化fui设置
            GRoot.inst.SetContentScaleFactor(1920, 1080, UIContentScaler.ScreenMatchMode.MatchHeight);

            self.AllPanelsDic?.Clear();
            self.VisiblePanelsDic?.Clear();
            self.VisiblePanelsQueue?.Clear();
            self.HidePanelsStack?.Clear();
            AddExtensions();
        }

        public static void Destroy(this FGUIComponent self)
        {
            self.ForceCloseAllPanel();
            self.UIPackageLocations.Clear();
        }

        /// <summary>
        /// 添加拓展类 处理
        /// </summary>
        private static void AddExtensions()
        {
            UIObjectFactory.SetLoaderExtension(typeof(FGUIGLoader));
        }

        #region ==================== Panel 功能  通用方法 ====================

        /// <summary>
        /// 窗口是否是正在显示的 
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <returns></returns>
        public static bool IsPanelVisible(this FGUIComponent self, PanelId id)
        {
            return self.VisiblePanelsDic.ContainsKey((int)id);
        }

        /// <summary>
        /// 关闭指定类型的所有界面
        /// </summary>
        public static int GetVisibalPanelCount(this FGUIComponent self)
        {
            return self.VisiblePanelsDic.Count;
        }

        #endregion

        #region ==================== Panel 功能  ShowPanel ====================

        public static void ShowPanel<T>(this FGUIComponent self, UIParam showData = null, PanelId prePanelId = PanelId.Invalid) where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            if (showData != null)
            {
                var paneldata = ObjectPool.Instance.Fetch<ShowPanelData>();
                paneldata.ContextData = showData;

                self.ShowPanelAsync(panelId, paneldata, prePanelId).Coroutine();
            }
            else
            {
                self.ShowPanelAsync(panelId, null, prePanelId).Coroutine();
            }
            // self.ShowPanel(panelId, showData, prePanelId);
        }

        /// <summary>
        /// 显示Id指定的UI窗口
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <OtherParam name="showData"></OtherParam>
        public static void ShowPanel(this FGUIComponent self, PanelId id, ShowPanelData showData = null, PanelId prePanelId = PanelId.Invalid)
        {
            self.ShowPanelAsync(id, showData, prePanelId).Coroutine();
            // FGUIEntity fuiEntity = self.ReadyToShowfuiEntity(id);
            // if (fuiEntity != null)
            // {
            //     if (showData != null)
            //     {
            //         fuiEntity.AddChild(showData);
            //     }
            //     self.RealShowPanel(fuiEntity, id, showData, prePanelId);
            // }
        }

        public static async ETTask ShowPanelAsync(this FGUIComponent self, PanelId id, ShowPanelData showData = null,
        PanelId prePanelId = PanelId.Invalid)
        {
            try
            {
                if (self.IsPanelVisible(id)) return;

                string panelName = id.ToString();
                string pkgName = $"Pkg_{panelName.Remove(panelName.Length - 5)}";

                await self.LoadPkg(pkgName);
                FGUIEntity fuiEntity = await self.ShowFUIEntityAsync(id);

                if (fuiEntity != null)
                {
                    if (showData != null)
                    {
                        fuiEntity.AddComponent(showData);
                    }

                    if (prePanelId == PanelId.Invalid && fuiEntity.panelType == UIPanelType.SecondPanel)
                    {
                        prePanelId = (PanelId)self.VisiblePanelsDic.Keys[self.VisiblePanelsDic.Count - 1];
                    }

                    self.RealShowPanel(fuiEntity, id, showData, prePanelId);
                }

                //推送界面打开的等待消息
                self.AddOrGetComponent<MessageWait>().Notify(panelName, 0);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                showData?.Dispose();
            }
        }

        public static async ETTask ShowPanelAsync<T>(this FGUIComponent self, ShowPanelData showData = null, PanelId prePanelId = PanelId.Invalid)
                where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            if (self.IsPanelVisible(panelId)) return;

            await self.ShowPanelAsync(panelId, showData, prePanelId);
        }

        private static FGUIEntity ReadyToShowfuiEntity(this FGUIComponent self, PanelId id)
        {
            FGUIEntity fuiEntity = self.GetFUIEntity(id);
            // 如果UI不存在开始实例化新的窗口
            if (null == fuiEntity)
            {
                fuiEntity = self.AddChild<FGUIEntity>(true);
                fuiEntity.PanelId = id;
                self.LoadFUIEntity(fuiEntity);
            }

            if (!fuiEntity.IsPreLoad)
            {
                self.LoadFUIEntity(fuiEntity);
            }

            return fuiEntity;
        }

        private static async ETTask<FGUIEntity> ShowFUIEntityAsync(this FGUIComponent self, PanelId id)
        {
            CoroutineLock coroutineLock = null;
            try
            {
                coroutineLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoadingPanels, id.GetHashCode());

                FGUIEntity fuiEntity = self.GetFUIEntity(id);
                // 如果UI不存在开始实例化新的窗口
                if (null == fuiEntity)
                {
                    fuiEntity = self.AddChild<FGUIEntity>(true);
                    fuiEntity.PanelId = id;
                    await self.LoadFUIEntitysAsync(fuiEntity);
                }

                if (!fuiEntity.IsPreLoad)
                {
                    await self.LoadFUIEntitysAsync(fuiEntity);
                }

                return fuiEntity;
            }
            catch (Exception e)
            {
                Log.Error($"打开界面{id}时出错! 请检查界面是否以打开. " + e);
                return null;
            }
            finally
            {
                coroutineLock?.Dispose();
            }
        }

        public static FGUIEntity GetFUIEntity(this FGUIComponent self, PanelId id)
        {
            if (self.AllPanelsDic.ContainsKey((int)id))
            {
                return self.AllPanelsDic[(int)id];
            }

            return null;
        }

        public static FGUIEntity GetFUIEntity(this FGUIComponent self, string panelName)
        {
            PanelId panelId = EnumHelper.FromString<PanelId>(panelName);
            FGUIEntity fuiEntity = self.GetFUIEntity(panelId);
            if (null == fuiEntity)
            {
                Log.Warning($"{panelId} is not created!");
                return null;
            }

            return fuiEntity;
        }

        public static T GetPanel<T>(this FGUIComponent self, bool isNeedShowState = false) where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FGUIEntity fuiEntity = self.GetFUIEntity(panelId);
            if (null == fuiEntity)
            {
                Log.Warning($"{panelId} is not created!");
                return null;
            }

            if (!fuiEntity.IsPreLoad)
            {
                Log.Warning($"{panelId} is not loaded!");
                return null;
            }

            if (isNeedShowState)
            {
                if (!self.IsPanelVisible(panelId))
                {
                    Log.Warning($"{panelId} is need show state!");
                    return null;
                }
            }

            return fuiEntity.GetComponent<T>();
        }

        /// <summary>
        /// 彻底关闭最新出现的弹窗
        /// </summary>
        public static PanelId GetLastPanel(this FGUIComponent self)
        {
            if (self.VisiblePanelsDic.Count <= 0)
            {
                return PanelId.Invalid;
            }

            return self.VisiblePanelsDic.LastValue.PanelId;
        }

        public static IFGUIEventHandler GetFUIEventHandler(this FGUIComponent self, string panelName)
        {
            PanelId panelId = EnumHelper.FromString<PanelId>(panelName);
            return FGUIEventComponent.Instance.GetUIEventHandler(panelId);
        }

        public static PanelId GetPanelIdByGeneric<T>(this FGUIComponent self) where T : Entity
        {
            if (self.IsDisposed) return PanelId.Invalid;
            if (FGUIEventComponent.Instance.PanelTypeInfoDict.TryGetValue(typeof (T).Name, out PanelInfo panelInfo))
            {
                return panelInfo.PanelId;
            }

            Log.Error($"{typeof (T).FullName} is not have any PanelId!");
            return PanelId.Invalid;
        }

        private static void RealShowPanel(this FGUIComponent self, FGUIEntity fuiEntity, PanelId id, ShowPanelData showData = null,
        PanelId prePanelId = PanelId.Invalid)
        {
            if (fuiEntity.panelType == UIPanelType.PopUp || fuiEntity.panelType == UIPanelType.SecondPanel)
            {
                self.VisiblePanelsQueue.Add(id);
            }

            fuiEntity.GComponent.visible = true;

            FGUIEventComponent.Instance.GetUIEventHandler(id).OnShow(fuiEntity, showData?.ContextData);

            self.VisiblePanelsDic.ForceEnqueue((int)id, fuiEntity);
            if (prePanelId != PanelId.Invalid)
            {
                self.HidePanel(prePanelId);
            }

            Log.Info("<color=#289D3A>### current Navigation panel </color>{0}".Fmt(fuiEntity.PanelId));
        }

        private static bool CheckDirectlyHide(this FGUIComponent self, PanelId id)
        {
            if (!self.VisiblePanelsDic.ContainsKey((int)id))
            {
                return false;
            }

            FGUIEntity fuiEntity = self.VisiblePanelsDic[(int)id];
            if (fuiEntity != null && !fuiEntity.IsDisposed)
            {
                var panelType = fuiEntity.panelType;
                if (panelType == UIPanelType.Bottom || panelType == UIPanelType.Fixed || panelType == UIPanelType.Other)
                {
                    return false;
                }

                fuiEntity.GComponent.visible = false;
                FGUIEventComponent.Instance.GetUIEventHandler(id).OnHide(fuiEntity);
            }

            // self.VisiblePanelsDic.Remove((int)id);
            // self.VisiblePanelsQueue.Remove(id);
            return true;
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        private static void LoadFUIEntity(this FGUIComponent self, FGUIEntity fuiEntity)
        {
            if (!FGUIEventComponent.Instance.PanelIdInfoDict.TryGetValue(fuiEntity.PanelId, out PanelInfo panelInfo))
            {
                Log.Error($"{fuiEntity.PanelId} panelInfo is not Exist!");
                return;
            }

            fuiEntity.GComponent = UIPackage.CreateObject(panelInfo.PackageName, panelInfo.ComponentName).asCom;

            FGUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnInitPanelCoreData(fuiEntity);

            fuiEntity.SetRoot(FGUIRootHelper.GetTargetRoot(fuiEntity.panelType));

            FGUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnInitComponent(fuiEntity);
            FGUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnRegisterUIEvent(fuiEntity);

            self.AllPanelsDic[(int)fuiEntity.PanelId] = fuiEntity;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        private static async ETTask LoadFUIEntitysAsync(this FGUIComponent self, FGUIEntity fuiEntity)
        {
            try
            {
                if (!FGUIEventComponent.Instance.PanelIdInfoDict.TryGetValue(fuiEntity.PanelId, out PanelInfo panelInfo))
                {
                    Log.Error($"{fuiEntity.PanelId} panelInfo is not Exist!");
                    return;
                }

                fuiEntity.GComponent = await UIPackageHelper.CreateObjectAsync(panelInfo.PackageName, panelInfo.ComponentName);
                fuiEntity.GComponent.MakeFullScreen();
            }
            catch (Exception ex)
            {
                Log.Error($"FUI 初始化错误:{ex}");
            }

            try
            {
                var handler = FGUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId);
                handler.OnInitPanelCoreData(fuiEntity);

                fuiEntity.SetRoot(FGUIRootHelper.GetTargetRoot(fuiEntity.panelType));

                handler.OnInitComponent(fuiEntity);
                handler.OnRegisterUIEvent(fuiEntity);
                self.AllPanelsDic[(int)fuiEntity.PanelId] = fuiEntity;
            }
            catch (Exception ex)
            {
                Log.Error($"FUI: {fuiEntity.PanelId} 初始化错误, 请检查UI事件逻辑! {ex}");
            }
        }

        /// <summary>
        /// 加载FUI的package包
        /// </summary>
        /// <param name="fUIComponent"></param>
        /// <param name="pkgName"></param>
        /// <returns></returns>
        public static async ETTask LoadPkg(this FGUIComponent fUIComponent, string pkgName)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            try
            {
                //已加载,则无需再加载
                if (!fUIComponent.UIPackageLocations.ContainsKey(pkgName))
                {
                    await fUIComponent.AddPackageAsync(pkgName);
                }

                //命名空间+类名  注意：类不可以是抽象类，否则无法创建
                string strClass = $"ET.{pkgName}.{pkgName}Binder";

                //通strClass获得type
                Type type = EventSystem.Instance.GetType(strClass);
                if (type == null)
                {
                    Log.Error("FGUI 反射获取class失败! Class:" + strClass);
                    return;
                }

                //创建type类的实例 "objs"
                object objs = System.Activator.CreateInstance(type);
                if (objs == null)
                {
                    Log.Error("FGUI 创建实例失败! pkgName:" + pkgName);
                    return;
                }

                //加载需要访问的方法，如果有参数的可以设置传参Type[]中是参数的个数和类型，可根据实际调用的方法定义,无参方法GetMethod中只填写类名变量即可
                type.GetMethod("BindAll").Invoke(objs, null);
            }
            catch (Exception ex)
            {
                Log.Error($"LoadPkg: {pkgName} 失败,请检查是否未生成FUI代码: {ex}");
            }
        }

        #endregion

        #region ==================== Panel 功能  ClosePanel ====================

        /// <summary>
        /// 关闭并卸载UI界面
        /// </summary>
        /// <param name="self"></param>
        /// <param name="panelId"></param>
        public static void ClosePanel(this FGUIComponent self, PanelId panelId)
        {
            if (!self.VisiblePanelsDic.ContainsKey((int)panelId))
            {
                return;
            }

            //推送关闭界面的消息
            self.AddOrGetComponent<MessageWait>().Remove(panelId.ToString());

            self.HidePanel(panelId);
            self.UnLoadPanel(panelId);
            Log.Info($"<color=#289D3A>## close panel without Pop ##</color>  {panelId}");
        }

        /// <summary>
        /// 关闭并卸载UI界面
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        public static void ClosePanel<T>(this FGUIComponent self) where T : Entity
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(hidePanelId);
        }

        /// <summary>
        /// 根据系统ID, 关闭 对应的界面
        /// </summary>
        /// <param name="self"></param>
        /// <param name="systemId"></param>
        public static void ClosePanelBySystemId(this FGUIComponent self, int systemId)
        {
            //TODO:
            /*var conf = category_SystemUnlock.Instance.Get(systemId);
            PanelId panelId = EnumHelper.FromString<PanelId>(conf.PanelName);
            self.ClosePanel(panelId);*/
        }

        /// <summary>
        /// 彻底关闭最新出现的弹窗
        /// </summary>
        public static void CloseLastPopPanel(this FGUIComponent self)
        {
            if (self.VisiblePanelsQueue.Count <= 0)
            {
                return;
            }

            PanelId panelId = self.VisiblePanelsQueue[self.VisiblePanelsQueue.Count - 1];
            if (!self.IsPanelVisible(panelId))
            {
                return;
            }

            self.ClosePanel(panelId);
        }

        /// <summary>
        /// 关闭指定类型的所有界面
        /// </summary>
        public static void ClosePanelByType(this FGUIComponent self, UIPanelType targetPanelType = UIPanelType.PopUp,
        PanelId ignorePanelId = PanelId.Invalid)
        {
            if (self.AllPanelsDic == null)
            {
                return;
            }

            int ignoreId = (int)ignorePanelId;
            foreach (var dic in self.AllPanelsDic.ToArray())
            {
                if (dic.Key != ignoreId &&
                    targetPanelType == dic.Value.panelType) //targetPanelType.HasFlag(dic.Value.PanelCoreData.panelType)
                {
                    self.ClosePanel((PanelId)dic.Key);
                }
            }
        }

        public static void CloseAllPanel(this FGUIComponent self, UIPanelType ignore = UIPanelType.Fixed)
        {
            if (self.AllPanelsDic == null)
            {
                return;
            }

            foreach (var dic in self.AllPanelsDic.ToArray())
            {
                if (!ignore.HasFlag(dic.Value.panelType))
                {
                    self.ClosePanel((PanelId)dic.Key);
                }
            }
        }

        public static void ForceCloseAllPanel(this FGUIComponent self)
        {
            if (self.AllPanelsDic == null)
            {
                return;
            }

            foreach (KeyValuePair<int, FGUIEntity> panel in self.AllPanelsDic)
            {
                FGUIEntity fuiEntity = panel.Value;
                if (fuiEntity == null || fuiEntity.IsDisposed)
                {
                    continue;
                }

                self.HidePanel(fuiEntity.PanelId);
                self.UnLoadPanel(fuiEntity.PanelId, false);
                fuiEntity?.Dispose();
            }

            self.VisiblePanelsDic.Clear();
            self.AllPanelsDic.Clear();
            self.FGUIEntitylistCached.Clear();
            self.VisiblePanelsQueue.Clear();
            self.HidePanelsStack.Clear();
        }

        /// <summary>
        /// 隐藏ID指定的UI窗口
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <OtherParam name="onComplete"></OtherParam>
        private static void HidePanel(this FGUIComponent self, PanelId id)
        {
            if (!self.CheckDirectlyHide(id))
            {
                Log.Warning($"检测关闭 panelId: {id} 失败！");
                return;
            }
        }

        private static void HidePanel<T>(this FGUIComponent self) where T : Entity
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T>();
            self.HidePanel(hidePanelId);
        }

        /// <summary>
        /// 隐藏最新出现的窗口
        /// </summary>
        public static void HideLastPanel(this FGUIComponent self)
        {
            if (self.VisiblePanelsQueue.Count <= 0)
            {
                return;
            }

            PanelId panelId = self.VisiblePanelsQueue[self.VisiblePanelsQueue.Count - 1];
            if (!self.IsPanelVisible(panelId))
            {
                return;
            }

            self.HidePanel(panelId);
        }

        /// <summary>
        /// 卸载指定的UI窗口实例
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        private static void UnLoadPanel(this FGUIComponent self, PanelId id, bool isDispose = true)
        {
            FGUIEntity fuiEntity = self.GetFUIEntity(id);
            if (null == fuiEntity)
            {
                Log.Error($"FGUIEntity PanelId {id} is null!!!");
                return;
            }

            FGUIEventComponent.Instance.GetUIEventHandler(id).BeforeUnload(fuiEntity);
            if (fuiEntity.IsPreLoad)
            {
                fuiEntity.GComponent.Dispose();
                fuiEntity.GComponent = null;
            }

            if (isDispose)
            {
                self.AllPanelsDic.Remove((int)id);
                self.VisiblePanelsDic.Remove((int)id);
                self.VisiblePanelsQueue.Remove(id);
                fuiEntity?.Dispose();

                string panelName = id.ToString();
                string pkgName = $"pkg_{panelName.Remove(panelName.Length - 5)}";
                self.RemovePackage(pkgName);
            }
        }

        private static void UnLoadPanel<T>(this FGUIComponent self) where T : Entity
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T>();
            self.UnLoadPanel(hidePanelId);
        }

        #endregion
    }
}