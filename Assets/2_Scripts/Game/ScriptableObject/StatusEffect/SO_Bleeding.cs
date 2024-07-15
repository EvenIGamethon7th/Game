using System;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
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
            BleedingDuration(monsterData, monster);
        }

        private void BleedingDuration(MonsterData monsterData, Monster monster)
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(Duration)))
                .TakeUntil(monster.OnDisableAsObservable())
                .Subscribe(
                    _ =>
                    {
                        if (monster.isActiveAndEnabled)
                        {
                            float damage = monsterData.MaxHp * (mPercentDamage * 0.01f);
                            monster.TakeDamage(damage, Define.EAttackType.TrueDamage);
                        }
                    }
                )
                .AddTo(monster);
        }
        
        public override void OnRemove(MonsterData monsterData, Action endCallback = null)
        {
            endCallback?.Invoke();
        }
    }
}