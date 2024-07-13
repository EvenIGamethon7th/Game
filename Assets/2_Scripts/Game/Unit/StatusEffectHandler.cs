using System;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    using Monster;
    using StatusEffect;
    using System.Diagnostics;

    public class StatusEffectHandler : MonoBehaviour
    {
        private Dictionary<StatusEffectSO.EDebuffTypes, MonsterStatusEffect> mStatusEffects = new ();
        // 차후 유저 유닛도 해당 Handler를 사용할 가능성이 높기에 확장성 고려해서 다시 짜야할 듯 
        private Monster mMonster;

        public void Start()
        {
            mMonster = GetComponent<Monster>();
        }

        public void Update()
        {
            UpdateStatusEffect();
        }

        public bool AddStatusEffect(StatusEffectSO statusEffect)
        {
            bool isSuccess = mStatusEffects.TryGetValue(statusEffect.DeBuffType, out MonsterStatusEffect remainDebuff);
            if (isSuccess)
            {
                isSuccess = statusEffect.Duration > remainDebuff.Duration;
                if (!isSuccess) return false;
                remainDebuff.Clear();
                mStatusEffects.Remove(statusEffect.DeBuffType);
            }

            isSuccess = statusEffect.CanApply();
            if (!isSuccess) return false;

            statusEffect.OnApply();
            var monsterEffect = MemoryPoolManager<MonsterStatusEffect>.CreatePoolingObject();
            mStatusEffects.Add(statusEffect.DeBuffType, monsterEffect);
            return true;
        }

        private void UpdateStatusEffect()
        {
            foreach (var statusEffect in mStatusEffects)
            {
                mStatusEffects[statusEffect.Key].Duration -= Time.deltaTime;
                if (mStatusEffects[statusEffect.Key].Duration < 0)
                {
                    mStatusEffects[statusEffect.Key].Clear();
                    mStatusEffects.Remove(statusEffect.Key);
                }
            }
        }

        private void OnDisable()
        {
            foreach (var statusEffect in mStatusEffects)
            {
                statusEffect.Value.Clear();
            }

            mStatusEffects.Clear();
        }
    }
}