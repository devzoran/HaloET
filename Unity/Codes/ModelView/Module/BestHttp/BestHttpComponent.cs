namespace ET
{
    [EnableMethod]
    [ComponentOf(typeof(Scene))]
    public class BestHttpComponent : Entity, IAwake, IDestroy
    {
        public static BestHttpComponent Instance => Game.Scene.AddComponent<BestHttpComponent>();
    }
}