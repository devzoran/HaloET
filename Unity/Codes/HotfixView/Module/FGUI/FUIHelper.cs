using System;
using System.Collections.Generic;
using ET.EventType;
using FairyGUI;

namespace ET
{
    [FriendClass(typeof(FUIComponent))]
    public static class FUIHelper
    {
        #region ==================== 循环列表List ====================

        /// <summary>
        /// GList的初始化
        /// </summary>
        public static void Init(this GList list, Action<int, GObject> onRefreshItem, Action<int, GObject> onClickItem = null, int itemNums = -1,
        int selectIndex = -1)
        {
            list.itemRenderer = (index, ojb) => { onRefreshItem.Invoke(index, ojb); };

            if (onClickItem != null)
            {
                list.onClickItem.Set((context) =>
                {
                    GObject item = (GObject)context.data;
                    int childIndex = list.GetChildIndex(item);
                    onClickItem.Invoke(list.ChildIndexToItemIndex(childIndex), item);
                });

                if (itemNums > -1)
                {
                    list.numItems = itemNums;
                }

                if (list.numItems != 0 && selectIndex > -1)
                {
                    int childIndex = list.ItemIndexToChildIndex(selectIndex);
                    GObject item = list.GetChildAt(childIndex);
                    list.selectedIndex = selectIndex;
                    onClickItem.Invoke(selectIndex, item);
                }
            }
        }

        /// <summary>
        /// GList的初始化， 需要传入DataList
        /// </summary>
        public static void Init<T>(this GList list, List<T> dataList, Action<int, T, GObject> onRefreshItem,
        Action<int, T, GObject> onClickItem = null,
        int selectIndex = -1)
        {
            if (dataList == null || dataList.Count == 0)
            {
                list.numItems = 0;
                return;
            }

            list.itemRenderer = (index, ojb) => { onRefreshItem.Invoke(index, dataList[index], ojb); };

            if (onClickItem != null)
            {
                list.onClickItem.Set((context) =>
                {
                    GObject item = (GObject)context.data;
                    int childIndex = list.GetChildIndex(item);
                    onClickItem.Invoke(list.ChildIndexToItemIndex(childIndex), dataList[list.ChildIndexToItemIndex(childIndex)], item);
                });

                list.numItems = dataList.Count;

                if (list.numItems != 0 && selectIndex > -1)
                {
                    int childIndex = list.ItemIndexToChildIndex(selectIndex);
                    GObject item = list.GetChildAt(childIndex);
                    list.selectedIndex = selectIndex;
                    onClickItem.Invoke(selectIndex, dataList[selectIndex], item);
                }
            }
        }

        #endregion

        #region ==================== 树形页签List ====================

        /// <summary>
        /// 初始化 TreeList (仅限2层结构)
        /// onRefreshItem 刷新文件夹和页签回调
        /// onClickItem 点击页签回调
        /// mulSelect : true 文件夹可以多选,  false 文件夹单选
        /// </summary>
        public static void InitTree(this GTree list, Action<int, GTreeNode> onRefreshItem, Action<int, GObject> onClickItem, int itemNums = -1,
        int selectIndex = -1, bool mulSelect = false)
        {
            list.treeNodeRender = (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                onRefreshItem.Invoke(index, treeNode);
            };

            list.treeNodeWillExpand = (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                onRefreshItem.Invoke(index, treeNode);

                int startIndex = index + 1;
                if (index > 0)
                {
                    for (int i = 0; i < index; i++)
                    {
                        startIndex += list.rootNode.GetChildAt(i).numChildren;
                    }
                }

                for (int i = 0; i < treeNode.numChildren; i++)
                {
                    GTreeNode item = treeNode.GetChildAt(i);
                    onRefreshItem.Invoke(i + startIndex, item);

                    if (i == 0 && treeNode.expanded)
                    {
                        //点击文件夹下第一个页签
                        item.cell.asButton.selected = true;
                        onClickItem.Invoke(i + startIndex, item.cell);
                    }
                }
            };

            EventCallback1 listener = (context) =>
            {
                GObject item = (GObject)context.data;

                int childIndex = list.GetChildIndex(item);
                int index = list.ChildIndexToItemIndex(childIndex);

                GTreeNode node = item.treeNode;
                if (node.isFolder)
                {
                    //单选时, 点击已经展开的文件夹,不会收起它
                    if (!mulSelect)
                    {
                        for (int i = list.rootNode.numChildren - 1; i >= 0; i--)
                        {
                            GTreeNode tarNode = list.rootNode.GetChildAt(i);
                            tarNode.expanded = tarNode == node;
                        }
                    }

                    //选中文件夹下第一个页签按钮
                    node.GetChildAt(0).cell.asButton.selected = true;
                }
                else
                {
                    int pIndex = list.rootNode.GetChildIndex(node.parent);

                    for (int i = 0; i < pIndex; i++)
                    {
                        index += list.rootNode.GetChildAt(i).numChildren;
                    }

                    //选中文件夹下第一个页签
                    onClickItem.Invoke(index, item);
                }
            };

            list.onClickItem.Set(listener);

            if (itemNums > -1)
            {
                list.numItems = itemNums;
            }
            else
            {
                itemNums = list.numItems;
            }

            if (itemNums != 0 && selectIndex > -1)
            {
                GTreeNode treeNode = list.rootNode.GetChildAt(selectIndex);

                //展开文件夹
                list.treeNodeWillExpand(treeNode, true);
                //触发点击文件夹事件
                listener.Invoke(new EventContext() { data = treeNode.cell });
            }

            onRefreshItem.Invoke(0, list.rootNode.GetChildAt(0));
        }

        #endregion

        #region ==================== 树形页签List ====================

        /// <summary>
        /// 初始化 TreeList (仅限2层结构)
        /// onRefreshItem 刷新文件夹和页签回调
        /// onClickItem 点击页签回调
        /// mulSelect : true 文件夹可以多选,  false 文件夹单选
        /// </summary>
        public static void InitTree<T>(this GTree list, List<T> data, Action<T, GTreeNode> onRefreshItem, Action<T, GTreeNode> onClickItem,
        int selectIndex = -1, bool mulSelect = false)
        {
            list.treeNodeRender = (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                onRefreshItem.Invoke(data[index], treeNode);
            };

            list.treeNodeWillExpand = (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                onRefreshItem.Invoke(data[index], treeNode);

                int startIndex = index + 1;
                if (index > 0)
                {
                    for (int i = 0; i < index; i++)
                    {
                        startIndex += list.rootNode.GetChildAt(i).numChildren;
                    }
                }

                for (int i = 0; i < treeNode.numChildren; i++)
                {
                    GTreeNode item = treeNode.GetChildAt(i);
                    onRefreshItem.Invoke(data[i + startIndex], item);

                    if (i == 0 && treeNode.expanded)
                    {
                        //点击文件夹下第一个页签
                        item.cell.asButton.selected = true;
                        onClickItem.Invoke(data[i + startIndex], item);
                    }
                }
            };

            EventCallback1 listener = (context) =>
            {
                GObject item = (GObject)context.data;

                int childIndex = list.GetChildIndex(item);
                int index = list.ChildIndexToItemIndex(childIndex);

                GTreeNode node = item.treeNode;
                if (node.isFolder)
                {
                    //单选时, 点击已经展开的文件夹,不会收起它
                    if (!mulSelect)
                    {
                        for (int i = list.rootNode.numChildren - 1; i >= 0; i--)
                        {
                            GTreeNode tarNode = list.rootNode.GetChildAt(i);
                            tarNode.expanded = tarNode == node;
                        }
                    }

                    //选中文件夹下第一个页签按钮
                    node.GetChildAt(0).cell.asButton.selected = true;
                }
                else
                {
                    int pIndex = list.rootNode.GetChildIndex(node.parent);

                    for (int i = 0; i < pIndex; i++)
                    {
                        index += list.rootNode.GetChildAt(i).numChildren;
                    }

                    //选中文件夹下第一个页签
                    onClickItem.Invoke(data[index], node);
                }
            };

            list.onClickItem.Set(listener);

            if (data.Count > 0 && selectIndex > -1)
            {
                GTreeNode treeNode = list.rootNode.GetChildAt(selectIndex);

                //展开文件夹
                list.treeNodeWillExpand(treeNode, true);
                //触发点击文件夹事件
                listener.Invoke(new EventContext() { data = treeNode.cell });
            }

            onRefreshItem.Invoke(data[0], list.rootNode.GetChildAt(0));
        }

        #endregion

        #region ==================== Tips ====================

        //TODO:
        /*public static void ShowTips(int textId)
        {
            EventSystem.Instance.Publish(ClientSceneManagerComponent.Instance.ClientScene, new UI_ShowTips(){ Tip = textId.GetLanguageText()});
        }

        public static void ShowTips(string tip)
        {
            EventSystem.Instance.Publish(ClientSceneManagerComponent.Instance.ClientScene, new UI_ShowTips(){ Tip = tip});
        }
        
        public static void ShowItemTip(Msg_Item msgItem, bool haveCount = true)
        {
            Item item = ClientSceneManagerComponent.Instance.ClientScene.MyPlayer().GetItem(msgItem.Id);
            EventSystem.Instance.Publish(ClientSceneManagerComponent.Instance.ClientScene, new UI_ShowItemTip { Item = item, HaveCount = haveCount});
        }
        public static void ShowItemTip(Item item, bool haveCount = true)
        {
            EventSystem.Instance.Publish(ClientSceneManagerComponent.Instance.ClientScene, new UI_ShowItemTip { Item = item, HaveCount = haveCount});
        }
        public static void ShowItemTip(int itemConfigId, bool haveCount = false)
        {
            Item item = Root.Instance.Scene.MyPlayer().GetItem(itemConfigId);
            EventSystem.Instance.Publish(ClientSceneManagerComponent.Instance.ClientScene, new UI_ShowItemTip { Item = item, HaveCount = haveCount});
        }*/

        #endregion

        #region ==================== 确认弹窗 功能 ====================
        
        //TODO:
        /*/// <summary>
        /// 确认 / 取消 弹窗
        /// </summary>
        public static async ETTask<bool> ShowMessageBox1(string content, int title = 300, int yes = 303, int no = 304)
        {
            if (IsPanelVisible(PanelId.MessageBoxPanel))
            {
                return false;
            }

            await ShowPanelAsync<MessageBoxPanel>();
            var panel = GetPanel<MessageBoxPanel>();
            return await panel.ShowMessageBox(content, title, yes, no);
        }

        /// <summary>
        /// 是 / 否 弹窗
        /// </summary>
        public static async ETTask<bool> ShowMessageBox2(string content, int title = 300)
        {
            if (IsPanelVisible(PanelId.MessageBoxPanel))
            {
                return false;
            }

            await ShowPanelAsync<MessageBoxPanel>();
            var panel = GetPanel<MessageBoxPanel>();
            return await panel.ShowMessageBox(content, title, 301, 302);
        }

        /// <summary>
        /// 确认 (关闭) 弹窗
        /// </summary>
        public static async ETTask<bool> ShowMessageBox3(string content, int title = 300)
        {
            if (IsPanelVisible(PanelId.MessageBoxPanel))
            {
                return false;
            }

            await ShowPanelAsync<MessageBoxPanel>();
            var panel = GetPanel<MessageBoxPanel>();
            return await panel.ShowMessageBox(content, title, 303, 0);
        }*/

        #endregion

        #region ==================== 打开 / 关闭界面 及相关方法 ====================

        public static async ETTask WaitShowen(PanelId id)
        {
            await FUIComponent.Instance.GetComponent<MessageWait>().Wait(id.ToString());
        }
        public static async ETTask WaitShowen(string PanelId)
        {
            await FUIComponent.Instance.GetComponent<MessageWait>().Wait(PanelId);
        }
        
        
        /// <summary>
        /// 显示Id指定的UI窗口
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <OtherParam name="showData"></OtherParam>
        public static void ShowPanel(PanelId id, ShowPanelData showData = null, PanelId prePanelId = PanelId.Invalid)
        {
            FUIComponent.Instance.ShowPanelAsync(id, showData, prePanelId).Coroutine();
        }

        public static void ShowPanel<T>(Entity showData = null, PanelId prePanelId = PanelId.Invalid) where T : Entity
        {
            FUIComponent fuiComponent = FUIComponent.Instance;

            PanelId panelId = fuiComponent.GetPanelIdByGeneric<T>();
            if (IsPanelVisible(panelId))
            {
                ClosePanel<T>();
            }
            if (showData != null)
            {
                ShowPanelData paneldata = ObjectPool.Instance.Fetch<ShowPanelData>();
                paneldata.ContextData = showData;
                fuiComponent.ShowPanelAsync(panelId, paneldata, prePanelId).Coroutine();
            }
            else
            {
                fuiComponent.ShowPanelAsync(panelId, null, prePanelId).Coroutine();
            }
        }

        public static async ETTask ShowPanelAsync<T>(Entity showData = null, PanelId prePanelId = PanelId.Invalid) where T : Entity
        {
            FUIComponent fuiComponent = FUIComponent.Instance;

            PanelId panelId = fuiComponent.GetPanelIdByGeneric<T>();
            if (IsPanelVisible(panelId))
            {
                ClosePanel<T>();
            }
            if (showData != null)
            {
                ShowPanelData paneldata = ObjectPool.Instance.Fetch<ShowPanelData>();
                paneldata.ContextData = showData;
                await fuiComponent.ShowPanelAsync(panelId, paneldata, prePanelId);
            }
            else
            {
                await fuiComponent.ShowPanelAsync(panelId, null, prePanelId);
            }
        }

        public static void ClosePanel(PanelId id)
        {
            FUIComponent.Instance.ClosePanel(id);
        }

        public static void ClosePanel<T>() where T : Entity
        {
            FUIComponent fuiComponent = FUIComponent.Instance;
            PanelId panelId = fuiComponent.GetPanelIdByGeneric<T>();
            fuiComponent.ClosePanel(panelId);
        }

        /// <summary>
        /// 关闭指定类型的界面
        /// </summary>
        /// <param name="targetPanelType"></param>
        /// <param name="ignorePanelId"></param>
        public static void ClosePanelByType(UIPanelType targetPanelType = UIPanelType.PopUp,
                PanelId ignorePanelId = PanelId.Invalid)
        {
            FUIComponent.Instance.ClosePanelByType(targetPanelType, ignorePanelId);
        }
        
        public static void CloseAllPanel()
        {
            FUIComponent.Instance.CloseAllPanel();
        }

        /// <summary>
        /// 根据界面名, 弹出/跳转 对应的界面
        /// </summary>
        public static void ShowPanelByName(string panelName, ShowPanelData data = null)
        {
            PanelId panelId = EnumHelper.FromString<PanelId>(panelName);
            FUIComponent.Instance.ShowPanel(panelId, data);
        }

        /// <summary>
        /// 根据系统ID, 弹出/跳转 对应的界面
        /// </summary>
        public static void ShowPanelBySystemId(int systemId, ShowPanelData data = null)
        {
            //TODO:
            /*var conf = category_SystemUnlock.Instance.Get(systemId);
            PanelId panelId = EnumHelper.FromString<PanelId>(conf.PanelName);
            FUIComponent.Instance.ShowPanel(panelId, data);*/
        }

        /// <summary>
        /// 跳转到指定系统的UI界面
        /// </summary>
        public static void Turn2PanelBySystemId(int systemId, Entity showData = null, PanelId prePanelId = PanelId.Invalid)
        {
            //TODO:
            /*var conf = category_SystemUnlock.Instance.Get(systemId);
            PanelId panelId = EnumHelper.FromString<PanelId>(conf.PanelName);
            FUIComponent fuiComponent = FUIComponent.Instance;

            fuiComponent.ClosePanelByType();
            fuiComponent.ClosePanelByType(UIPanelType.SecondPanel, panelId);

            ShowPanelData paneldata = ObjectPool.Instance.Fetch<ShowPanelData>();
            paneldata.ContextData = showData;

            fuiComponent.ShowPanelAsync(panelId, paneldata, prePanelId).Coroutine();*/
        }

        /// <summary>
        /// 返回上一级主界面 (关闭所有弹出窗口)
        /// </summary>
        public static void ClosePopUpPanels()
        {
            FUIComponent fuiComponent = FUIComponent.Instance;
            fuiComponent.ClosePanelByType(); //关闭所有弹窗
            fuiComponent.ClosePanelByType(UIPanelType.SecondPanel); //关闭所有2级界面
            
            //刷新界面
            PanelId panelId = fuiComponent.GetLastPanel();
            if (panelId == PanelId.Invalid)
            {
                return;
            }

            FUIEntity fuiEntity = fuiComponent.GetFUIEntity(panelId);
            fuiEntity.GComponent.visible = true;
            FUIEventComponent.Instance.GetUIEventHandler(panelId).OnShow(fuiEntity);
        }

        /// <summary>
        /// 返回上一级主界面 (关闭所有弹出窗口)
        /// </summary>
        public static void Return()
        {
            FUIComponent fuiComponent = FUIComponent.Instance;
            fuiComponent.CloseLastPopPanel();
            
            //刷新界面
            PanelId panelId = fuiComponent.GetLastPanel();
            if (panelId == PanelId.Invalid)
            {
                return;
            }

            FUIEntity fuiEntity = fuiComponent.GetFUIEntity(panelId);
            fuiEntity.GComponent.visible = true;
            FUIEventComponent.Instance.GetUIEventHandler(panelId).OnShow(fuiEntity);
        }

        /// <summary>
        /// 判断界面是否显示
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public static bool IsPanelVisible(PanelId panelId)
        {
            return FUIComponent.Instance.IsPanelVisible(panelId);
        }

        /// <summary>
        /// 获取FUI界面脚本对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPanel<T>() where T : Entity
        {
            return FUIComponent.Instance.GetPanel<T>();
        }

        public static FUIEntity GetFUIEntity(string panelId)
        {
            return FUIComponent.Instance.GetFUIEntity(panelId);
        }
        
        #endregion
    }
}