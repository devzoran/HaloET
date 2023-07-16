using System;
using FairyGUI;
using UnityEngine;

namespace ET
{
    public class FGUIGLoader: GLoader
    {
        //FGUIGLoader 内存缓存的图片， 超过100张图就会清理, 因此该数值必须大于游戏中同时显示的界面上的gloader图片数
        const int maxCacheCount = 100;
        
        private static readonly QueueDictionary<string, Texture2D> cache = new QueueDictionary<string, Texture2D>();

        private async ETTask GetTexture(string imgUrl)
        {
            Texture2D tex = null;
            try
            {
                if (cache.TryGetValue(imgUrl, out tex))
                {
                    //刷新缓存排序
                    cache.ForceEnqueue(imgUrl, tex);
                }
                else
                {
                    if (this.url.StartsWith("http"))
                    {
                        tex = await BestHttpComponent.Instance.LoadTexture2D(imgUrl);
                    }
                    else
                    {
                        //TODO：
                        /*tex = ResComponent.Instance.LoadAsset<Texture2D>(this.url);*/
                    }

                    if (tex != null)
                    {
                        if (cache.Count > maxCacheCount)
                        {
                            string tarUrl = cache.FirstKey;
                            cache.Dequeue();
                            //TODO：
                            /*ResComponent.Instance.UnloadAsset(tarUrl); //卸载旧的缓存图片*/
                        }

                        cache.Enqueue(imgUrl, tex);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                if (this.url == imgUrl && !this.isDisposed)
                {
                    if (tex != null)
                        onExternalLoadSuccess(new NTexture(tex));
                    else
                        onExternalLoadFailed();
                }
            }
        }

        override protected void LoadExternal()
        {
            /*
            开始外部载入，地址在url属性
            载入完成后调用OnExternalLoadSuccess
            载入失败调用OnExternalLoadFailed
  
            注意：如果是外部载入，在载入结束后，调用OnExternalLoadSuccess或OnExternalLoadFailed前，
            比较严谨的做法是先检查url属性是否已经和这个载入的内容不相符。
            如果不相符，表示loader已经被修改了。
            这种情况下应该放弃调用OnExternalLoadSuccess或OnExternalLoadFailed。
            */

            GetTexture(this.url).Coroutine();
        }

        override protected void FreeExternal(NTexture texture)
        {
            if (!this.url.StartsWith("http"))
            {
                //TODO：
                /*if (ResComponent.Instance.GetAssetRef(this.url).UseCount <= 1)*/
                {
                    //移除本地缓存
                    cache.Remove(this.url);
                }

                //释放外部载入的资源
                //TODO：
                /*ResComponent.Instance.UnloadAsset(this.url);*/
            }
            else
            {
                // http载入的资源， 在缓存中移除后，会被GC回收， 这里就不主动释放了
            }
        }
    }
}