using System;

namespace _2_Scripts.Game.StatusEffect
{
    public interface IUnitStatsModifier
    {
        public void AdjustStat(MonsterData monsterData,Action endCallback);
    }
}