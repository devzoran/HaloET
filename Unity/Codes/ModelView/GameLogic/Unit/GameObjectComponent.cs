using UnityEngine;

namespace ET
{
    [ComponentOf]
    public class GameObjectComponent: Entity, IAwake, IDestroy
    {
        public GameObject GameObject { get; set; }
    }
}