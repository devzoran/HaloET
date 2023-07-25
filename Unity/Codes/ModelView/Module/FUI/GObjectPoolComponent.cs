using FairyGUI;
using UnityEngine;

namespace ET
{
    [ComponentOf]
    [EnableMethod]
    public class GObjectPoolComponent: Entity, IAwake<Transform>, IDestroy
    {
        [ObjectSystem]
        public class AwakeSystem: AwakeSystem<GObjectPoolComponent, Transform>
        {
            public override void Awake(GObjectPoolComponent self, Transform poolParent)
            {
                self.Awake(poolParent);
            }
        }

        [ObjectSystem]
        public class DestroySystem: DestroySystem<GObjectPoolComponent>
        {
            public override void Destroy(GObjectPoolComponent self)
            {
                self.Destroy();
            }
        }

        private GObjectPool _gObjectPool;

        public void Awake(Transform poolParent)
        {
            _gObjectPool = new GObjectPool(poolParent);
        }

        public void Destroy()
        {
            this._gObjectPool?.Clear();
            this._gObjectPool = null;
        }
        
        public GObject GetObject(string url)
        {
            return _gObjectPool.GetObject(url);
        }
        
        public GObject GetObject(string url, GComponent gParent)
        {
            GObject obj = _gObjectPool.GetObject(url);
            gParent.AddChild(obj);
            return obj;
        }
        
        public void ReturnObject(GObject obj)
        {
            _gObjectPool.ReturnObject(obj);
        }
    }
}