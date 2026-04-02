namespace GeneralLogicEnemies
{
    public interface IEnemyDeathHandler
    {
        void DisableColliderOnDeath(bool disable);
        void DisablePhysicsOnDeath(bool disable);
        void DisableAIOnDeath(bool disable);
    }
}