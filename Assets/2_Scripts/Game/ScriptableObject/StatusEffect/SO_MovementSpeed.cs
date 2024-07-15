
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.StatusEffect
{
    using _2_Scripts.Game.Monster;
    [CreateAssetMenu(menuName = "ScriptableObject/StatueEffect/Movement",fileName = "Movement_")]
    public class SO_MovementSpeed : StatusEffectSO
    {
        [Title("이동속도 증감")]
        [SerializeField]
        public float mPercentSpeed;

        public override bool CanApply(MonsterData monsterData)
        {
            return true;
        }

        public override void OnApply(MonsterData monsterData,Monster monster)
        {
            //TODO 체력바 밑에 아이콘 표시, 파티클 효과 표시
            monsterData.SetSpeed(mPercentSpeed);
        }
        
        
        public override void OnRemove(MonsterData monsterData, Action endCallback = null)
        {
            monsterData.SetSpeed(mPercentSpeed, true);
            endCallback?.Invoke();
        }
    }
}