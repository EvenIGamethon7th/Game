using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.StatusEffect
{[CreateAssetMenu(menuName = "ScriptableObject/StatueEffect/StatusModifier",fileName = "Status_")]
    public class SO_StatusModifier : StatusEffectSO
    {
        [Title("방어력 증감")]
        [SerializeField]
        private float mPercentDef;
        public override void OnApply(MonsterData monsterData, Monster.Monster monster)
        {
            monsterData.AddDefenceStat((monsterData.def * (mPercentDef * 0.01f)));
        }

        public override void OnRemove(MonsterData monsterData, Action endCallback = null)
        {
            monsterData.AddDefenceStat(-(monsterData.def * (mPercentDef * 0.01f)));
            endCallback?.Invoke();
        }
    }
}