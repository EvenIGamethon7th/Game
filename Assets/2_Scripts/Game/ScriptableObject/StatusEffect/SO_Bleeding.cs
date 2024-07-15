using System;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.StatusEffect
{
    using _2_Scripts.Game.Monster;
    [CreateAssetMenu(menuName = "ScriptableObject/StatueEffect/Bleeding",fileName = "Bleeding_")]
    public class SO_Bleeding : StatusEffectSO
    {
        [Title("출혈 데미지 퍼센트")]
        [SerializeField]
        private float mPercentDamage;
        public override void OnApply(MonsterData monsterData,Monster monster)
        {
            BleedingDuration(monsterData,monster).Forget();
        }

        private async UniTaskVoid BleedingDuration(MonsterData monsterData,Monster monster)
        {
            // monster max hp percent 2% damage
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            if (!monster.isActiveAndEnabled)
                return;
            monster.TakeDamage((monsterData.GetMonsterMaxHP * (mPercentDamage*0.01f)),Define.EAttackType.TrueDamage);
        }
        
        public override void OnRemove(MonsterData monsterData, Action endCallback = null)
        {
            endCallback?.Invoke();
        }
    }
}