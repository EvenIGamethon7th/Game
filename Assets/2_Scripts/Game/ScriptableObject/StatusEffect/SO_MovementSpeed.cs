
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.StatusEffect
{
    [CreateAssetMenu(menuName = "ScriptableObject/StatueEffect/Movement",fileName = "Movement_")]
    public class SO_MovementSpeed : StatusEffect,IUnitStatsModifier
    {
        [Title("이동속도 증감")]
        [SerializeField]
        public float mPercentSpeed;

        
        public override void OnApply()
        {
            //TODO 체력바 밑에 아이콘 표시, 파티클 효과 표시
        }
        
        
        protected override void OnRemove()
        {
            
        }

        public void AdjustStat(MonsterData monsterData, Action endCallback)
        {
            monsterData.speed  *= (mPercentSpeed * 0.01f);
            ExecuteAfterDuration(()=>
            {
                monsterData.speed /= (mPercentSpeed * 0.01f);
                endCallback.Invoke();
            }).Forget();
        }
    }
}