
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

        private MonsterData mMonsterData;
        
        public override void OnApply(Action removeCallback)
        {
            //TODO 체력바 밑에 아이콘 표시, 파티클 효과 표시

            this.mRemoveCallback = removeCallback;
            ExecuteAfterDuration().Forget();
        }
        
        
        protected override void OnRemove()
        {
            mMonsterData.speed -= mMonsterData.speed * (mPercentSpeed * 0.01f);
            Debug.Log(mMonsterData.speed);
        }

        public void AdjustStat(MonsterData monsterData)
        {
            mMonsterData = monsterData;
            monsterData.speed += monsterData.speed * (mPercentSpeed * 0.01f);
        }
    }
}