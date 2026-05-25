namespace Shared.Damage
{
    public static class DamageCalculator
    {
        private const int MinimumNetDamage = 0;

        public static int CalculateNetDamage(int incomingDamage, int defense)
        {
            int netDamage = incomingDamage - defense;

            return netDamage > MinimumNetDamage ? netDamage : MinimumNetDamage;
        }
    }
}
