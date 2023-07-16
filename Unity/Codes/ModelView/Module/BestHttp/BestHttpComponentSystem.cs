using System;
using BestHTTP;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class BestHttpComponentAwakeSystem: AwakeSystem<BestHttpComponent>
    {
        public override void Awake(BestHttpComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class BestHttpComponentDestroySystem: DestroySystem<BestHttpComponent>
    {
        public override void Destroy(BestHttpComponent self)
        {
            self.Destroy();
        }
    }

    [FriendClass(typeof(BestHttpComponent))]
    public static class BestHttpComponentSystem
    {
        public static void Awake(this BestHttpComponent self)
        {
            int logLevel = PlayerPrefs.GetInt("BestHTTP.HTTPManager.Logger.Level", (int)HTTPManager.Logger.Level);
            HTTPManager.Logger.Level = (BestHTTP.Logger.Loglevels)logLevel;

            string proxyURL = PlayerPrefs.GetString("BestHTTP.HTTPManager.Proxy", null);
            if (!string.IsNullOrEmpty(proxyURL))
            {
                try
                {
                    HTTPManager.Proxy = new HTTPProxy(new Uri(proxyURL), null, true);
                }
                catch
                {
                }
            }
            else
                HTTPManager.Proxy = null;
    
#if !BESTHTTP_DISABLE_CACHING
            //开启缓存
            HTTPManager.IsCachingDisabled = false;
            
            //清理超过30天的缓存
            // Remove too old cache entries.
            BestHTTP.Caching.HTTPCacheService.BeginMaintainence(
                new BestHTTP.Caching.HTTPCacheMaintananceParams(TimeSpan.FromDays(30), ulong.MaxValue));
#endif

            // Set a well observable value
            // This is how many concurrent requests can be made to a server
            HTTPManager.MaxConnectionPerServer = 10;
        }

        public static void Destroy(this BestHttpComponent self)
        {
        }

        /// <summary>
        /// 从网络或者本地加载图片
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async ETTask<Texture2D> LoadTexture2D(this BestHttpComponent self, string url)
        {
            ETTask<Texture2D> tcs = ETTask<Texture2D>.Create();

            // Construct the request
            var request = new HTTPRequest(new Uri(url), HTTPMethods.Get, true ,(req, resp) =>
            {
                switch (req.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                        {
                            Texture2D texture2D = resp.DataAsTexture2D;
                            tcs.SetResult(texture2D);
                        }
                        else
                        {
                            string error = string.Format(
                                "Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode,
                                resp.Message, resp.DataAsText);
                            tcs.SetException(new Exception(error));
                        }

                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HTTPRequestStates.Error:
                        tcs.SetException(new Exception("Request Finished with Error! " +
                            (req.Exception != null? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception")));
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        tcs.SetException(new Exception("Request Aborted!"));
                        break;

                    // Connecting to the server is timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        tcs.SetException(new Exception("Connection Timed Out!"));
                        break;

                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        tcs.SetException(new Exception("Processing the request Timed Out!"));
                        break;
                }
            });
            // Send out the request
            request.Send();

            return await tcs;
        }
    }
}