using System;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    using Monster;
    using StatusEffect;
    public class StatusEffectHandler : MonoBehaviour
    {
        private HashSet<StatusEffect> mStatusEffects = new ();
        // 차후 유저 유닛도 해당 Handler를 사용할 가능성이 높기에 확장성 고려해서 다시 짜야할 듯 
        private Monster mMonster;

        public void Start()
        {
            mMonster = GetComponent<Monster>();
        }

        public void AddStatusEffect(StatusEffect statusEffect)
        {
            if(!mStatusEffects.Add(statusEffect))
            {
                return;
            }

            if (statusEffect is IUnitStatsModifier adjuster)
            {
                adjuster.AdjustStat(mMonster.GetMonsterData);
            }
            statusEffect.OnApply(()=>
            {
                Debug.Log("끝남");
                mStatusEffects.Remove(statusEffect);
            });
        }
    }
}