using System;
using _2_Scripts.Game.Unit;
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
        
        [Title("레벨 제한이 들어가는지?")] 
        [SerializeField]
        private int mLevel = 0;
        
        
        public override void OnApply(MonsterData monsterData, Monster monster,CUnit unit)
        {
            if(unit.CharacterDatas.rank < mLevel)
                return;
            
            BleedingDuration(monsterData, monster);
        }

        public override void OnApply(MonsterData monsterData, Monster monster)
        {
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
                            HitEffectPlay(monster.transform.position);
                            
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