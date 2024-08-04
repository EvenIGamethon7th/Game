using _2_Scripts.Game.Monster;
using _2_Scripts.Game.ScriptableObject.Skill;
using _2_Scripts.Game.Unit;
using _2_Scripts.Trigger;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    [CreateAssetMenu(menuName = "ScriptableObject/Skill/Ressurection")]
    public class Ressurection : Skill
    {
        [SerializeField]
        [Title("지속 시간 ( 1초당 1)")]
        private float mLifeTime;

        [SerializeField]
        [Title("힐량(퍼센트)")]
        private float mHealAmountRate;

        //[SerializeField]
        //[Title("스킬 트리거")]
        //private GameObject mPortal;

        public override bool CastAttack(Transform ownerTransform = null, CharacterData ownerData = null, Action<Monster.Monster[]> beforeDamage = null, Action<Monster.Monster> afterDamage = null)
        {
            int healAmount = Mathf.RoundToInt(-IngameDataManager.Instance.MaxHp * mHealAmountRate * 0.01f);
            IngameDataManager.Instance.UpdateUserHp(healAmount);
            var detectingTargets = Physics2D.OverlapCircleAll(Vector2.zero, Range, TargetLayer);
            if (detectingTargets.Length == 0)
            {
                return false;
            }
            Movement(detectingTargets);
            return true;
        }

        private void Movement(Collider2D[] collision)
        {
            for (int i = 0; i < collision.Length; i++)
            {
                if (!collision[i].TryGetComponent<StatusEffectHandler>(out var monster)) continue;
                StatueEffects?.ForEach(effect => monster.AddStatusEffect(effect));
            }
        }
    }
}