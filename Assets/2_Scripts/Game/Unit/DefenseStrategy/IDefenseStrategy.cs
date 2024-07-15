namespace _2_Scripts.Game.Unit
{
    public interface IDefenseStrategy
    {
        public float CalculateDamage(float damage,MonsterData monsterData);
    }
}