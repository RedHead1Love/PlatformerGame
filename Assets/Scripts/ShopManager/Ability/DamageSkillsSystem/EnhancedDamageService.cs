using Player.Abilities;
using UnityEngine;

namespace Shared.Damage
{
    public sealed class EnhancedDamageService
    {
        private readonly DamageBonusService _bonusService;

        public EnhancedDamageService(DamageBonusService bonusService)
        {
            _bonusService = bonusService;
        }

        public int ProcessDamage(int baseDamage, GameObject target)
        {
            if (_bonusService != null)
            {
                return _bonusService.CalculateDamageWithBonuses(baseDamage, target);
            }

            return baseDamage;
        }
    }
}
