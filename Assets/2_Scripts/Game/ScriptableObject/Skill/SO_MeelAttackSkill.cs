
using System.Collections.Generic;
using _2_Scripts.Game.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    using _2_Scripts.Game.Monster;
    using System;
    using System.Linq;

    /// <summary>
    /// 기본 공격 또는 투사체가 없는 공격 스킬
    /// </summary>

    [CreateAssetMenu(menuName = "ScriptableObject/Skill/Melee",fileName = "Melee_")]
    public class SO_MeelAttackSkill : Skill
    {
        
        [Title("데미지 증감 퍼센트")]
        [SerializeField]
        public float PercentDamage { get; private set; }
        [Title("기본 사거리를 따른다")]
        [SerializeField]
        public bool FollowDefaultRange { get; private set; }

        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster[]> beforeDamage = null, Action<Monster> passive = null)
        {
            float range = FollowDefaultRange ? ownerData.range : this.Range;
            // 방어력 부분은 몬스터별로 공격력 감소 수식이 있을 수 있으므로 여기선 owner 데미지만 계산.
            float totalDamage = ownerData.GetTotalDamageToType(AttackType) * (PercentDamage * 0.01f);
            var detectingTargets = Physics2D.OverlapCircleAll(ownerTransform.position, range, TargetLayer);
            if (detectingTargets.Length == 0)
            {
                return false;
            }
            var targetCount = MaxHitUnit == 0 ? detectingTargets.Length : MaxHitUnit;

            var monsterArray = detectingTargets
                .Select(collider => collider.GetComponent<Monster>())
                .Where(monster => monster != null)
                .ToArray();

            beforeDamage?.Invoke(monsterArray);
            CastEffectPlay(ownerTransform.position);
            var attacker = ownerTransform.GetComponent<CUnit>();
            attacker.SetFlipUnit(detectingTargets[0].transform);
            for (int i = 0; i < targetCount; i++)
            { 
                var TargetMonster = monsterArray[i];

                if (TargetMonster.TakeDamage(totalDamage, AttackType))
                {
                    var statusEffectHandler = TargetMonster.gameObject.GetComponent<StatusEffectHandler>();
                    StatueEffects?.ForEach(effect =>statusEffectHandler.AddStatusEffect(effect,attacker));
                    HitEffectPlay(TargetMonster.transform.position);
                    passive?.Invoke(TargetMonster);
                }

                else
                {
                    targetCount = targetCount < monsterArray.Length ? targetCount + 1 : targetCount;
                }
            }

            return true;
        }
    }
}