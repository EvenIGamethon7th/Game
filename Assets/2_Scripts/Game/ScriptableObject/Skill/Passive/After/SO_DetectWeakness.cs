using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive
{
    [CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/HealthRateDamage")]
    public class SO_DetectWeakness : AfterPassive
    {
        [Title("체력 비율")]
        [SerializeField]
        private float mPercent;

        public override void AfterDamage(Monster.Monster monsters)
        {
            monsters.TakeDamage(monsters.GetMonsterData.MaxHp * mPercent * 0.01f, Utils.Define.EAttackType.TrueDamage);
        }
    }
}