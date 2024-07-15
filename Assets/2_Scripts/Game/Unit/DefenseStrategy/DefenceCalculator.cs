using System.Collections.Generic;
using _2_Scripts.Utils;

namespace _2_Scripts.Game.Unit
{
    public static class DefenceCalculator
    {
        private static Dictionary<Define.EAttackType,IDefenseStrategy> mDefenseStrategies = new Dictionary<Define.EAttackType, IDefenseStrategy>
        {
            {Define.EAttackType.Physical, new PhysicalDefenseStrategy()},
            {Define.EAttackType.Magical, new MagicalDefenseStrategy()},
            {Define.EAttackType.TrueDamage, new TrueDamageDefenseStrategy()}
        };
        
        public static float CalculateDamage(float damage, MonsterData monsterData, Define.EAttackType attackType)
        {
            return mDefenseStrategies[attackType].CalculateDamage(damage, monsterData);
        }
    }
}