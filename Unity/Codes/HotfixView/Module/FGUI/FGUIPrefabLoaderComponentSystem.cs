using FairyGUI;
using UnityEngine;

namespace ET
{
	[FriendClass(typeof(FUIPrefabLoaderComponent))]
	public static class FGUIPrefabLoaderComponentSystem
	{
		[ObjectSystem]
		public class AwakeSystem1 : AwakeSystem<FUIPrefabLoaderComponent, GGraph, string>
		{
			public override void Awake(FUIPrefabLoaderComponent self, GGraph graph, string locationName)
			{
				self.Awake(graph, locationName).Coroutine();
			}
		}
		[ObjectSystem]
		public class DestroySystem : DestroySystem<FUIPrefabLoaderComponent>
		{
			public override void Destroy(FUIPrefabLoaderComponent self)
			{
				self.OnDestroy();
			}
		}

		private static async ETTask Awake(this FUIPrefabLoaderComponent self, GGraph graph, string locationName)
		{
			graph.visible = false;
			self.AssetLocationName = locationName;
		    GameObjectComponent objComponent = self.AddComponent<GameObjectComponent>();

		    //TODO:
		    /*await objComponent.LoadPrefabAsync(locationName);
		    self.ShowPrefab(graph, objComponent.GameObject);*/
		    
		    //设置默认大小
		    self.SetPosAndRot(new Vector3(200, 200, 1000), Quaternion.Euler(new Vector3(0f, 150f, 0f)), new Vector3(400, 400, 400));
		    graph.visible = true;
		}

		private static void ShowPrefab(this FUIPrefabLoaderComponent self, GGraph graph, GameObject prefab)
		{
			self.Graph = graph;
			
			//关于Wrapper的使用, 详见 https://fairygui.com/docs/unity/insert3d
			if (self.Wrapper == null)
			{
				self.Wrapper = new GoWrapper(prefab);
			}
			else
			{
				self.Wrapper.wrapTarget = prefab;
			}
			self.Graph.SetNativeObject(self.Wrapper);
		}

		private static void SetPosAndRot(this FUIPrefabLoaderComponent self, Vector3 pos, Quaternion rot , Vector3 scale)
		{
			Transform trans = self.GetComponent<GameObjectComponent>().GameObject.transform;

			trans.localPosition = pos;
			trans.localScale = scale;
			trans.localRotation = rot;
			
			self.Refresh();
		}
		
		public static void SetRot(this FUIPrefabLoaderComponent self, Quaternion rot)
		{
			Transform trans = self.GetComponent<GameObjectComponent>().GameObject.transform;

			trans.localRotation = rot;
			self.Refresh();
		}
		
		public static void AddRot(this FUIPrefabLoaderComponent self, Quaternion rot)
		{
			Transform trans = self.GetComponent<GameObjectComponent>().GameObject.transform;
			
			trans.localRotation = new Quaternion(trans.localRotation.x + rot.x, trans.localRotation.y + rot.y, trans.localRotation.w + rot.w, trans.localRotation.z + rot.z);
			self.Refresh();
		}

		/// <summary>
		/// 刷新显示的prefab
		/// </summary>
		/// <param name="self"></param>
		private static void Refresh(this FUIPrefabLoaderComponent self)
		{
			self.Wrapper.CacheRenderers();
		}

		private static void OnDestroy(this FUIPrefabLoaderComponent self)
		{
			self.Wrapper?.Dispose();
			self.Wrapper = null;
			self.AssetLocationName = string.Empty;
		}
		
	}
}
