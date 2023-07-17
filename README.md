# HaloET Project
***
* Unity 编辑器版本 2021.3.27f1
* FairyGUI

# NOTES
***

### 目前客户端共有5个程序集，其作用如下 

  - 1.`Model` 逻辑层数据结构定义 
  
  - 2.`Hotfix` 逻辑层的逻辑方法 
  
  - 3.`ModelView` 显示层的数据结构定义 
  
  - 4.`HotfixView` 显示层的逻辑方法 
  
  - 5.`ThirdParty` 第三方库 

  ```
  这四个程序集引用 Mono 工程，dll 引用 Unity/Temp/Bin/Debug下的dll；
  
  Unity.Mono 工程，用来放置 Unity 脚本代码，作为热更新的冷更层；
  
  Model ModelView Hotfix HotfixView 不允许放任何 MonoBehaviour 脚本。
  ```

<br/>

### 为什么要分出 `ModelView` 跟 `HotfixView` ？
&emsp;&emsp;主要原因是要分离显示层跟逻辑层，逻辑层的代码其实可以用来做压测机器人。
如果一开始就定好这样的结构，压测机器人完全可以利用客户端逻辑层的代码，节省大量时间

<br/>

***


### 贴个关于`EventSystem.PublishAsync` 方法的排查记录
```csharp {.line-numbers}
// 如果 AppStart_Init.cs 中异步方式抛事件
await Game.EventSystem.PublishAsync(new EventType.AppStartInitFinish() { ZoneScene = zoneScene });


// 那么事件处理类一定要继承 AEventAsync 而不是下面代码段的 AEvent
public class AppStartInitFinish_CreateLoginUI: AEvent<EventType.AppStartInitFinish>
{
    ...
}


// 附上 EventSystem.cs 下的代码段和注释，不过 ET7 里已经统一成了 AEvent，确实TM有点坑
public async ETTask PublishAsync<T>(T a) where T : struct
{
    ···
    using (ListComponent<ETTask> list = ListComponent<ETTask>.Create())
    {
        for (int i = 0; i < iEvents.Count; ++i)
        {
            object obj = iEvents[i];
            //看这里！ 它做了类型判断过滤！！！！！！！！！！！！！
            if (!(obj is AEventAsync<T> aEvent))
            {
                Log.Error($"event error: {obj.GetType().FullName}");
                continue;
            }

            list.Add(aEvent.Handle(a));
        }

        ···
    }
}
```