using GeneralLogicEnemies;

namespace Player.Abilities
{
    public interface IVampireHealthSystem
    {
        void OnEnemyKilled(Entity enemy);
    }
}
