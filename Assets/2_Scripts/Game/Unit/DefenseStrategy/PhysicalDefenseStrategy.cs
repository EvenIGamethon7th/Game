namespace _2_Scripts.Game.Unit
{
    public class PhysicalDefenseStrategy : IDefenseStrategy
    {
        public float CalculateDamage(float damage, MonsterData monsterData)
        {
            float totalDamage = damage * (100 / (100 + monsterData.def));
            return totalDamage > 0 ? totalDamage : 0;
        }
    }
}