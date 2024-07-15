using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    using _2_Scripts.Game.Monster;
    using System;
    using System.Linq;
    using _2_Scripts.Game.Unit;
    [CreateAssetMenu(menuName = "ScriptableObject/Skill/MeleeCoolTimeSet",fileName = "MeleeC_")]
    public class SO_AttackCoolTimeSet : SO_MeelAttackSkill
    {
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
            
            CastEffectPlay(ownerTransform.position);
            var attackerUnit = ownerTransform.GetComponent<CUnit>();
            attackerUnit.SetFlipUnit(detectingTargets[0].transform);
            for (int i = 0; i < targetCount; i++)
            { 
                var TargetMonster = monsterArray[i];

                if (TargetMonster.TakeDamage(totalDamage, AttackType))
                {
                    var statusEffectHandler = TargetMonster.gameObject.GetComponent<StatusEffectHandler>();
                    if (TargetMonster.GetMonsterData.hp <= 0)
                    {
                            attackerUnit.IsNotCoolTimeSKill = true;
                    }

                    StatueEffects?.ForEach(effect =>statusEffectHandler.AddStatusEffect(effect,attackerUnit));
                    HitEffectPlay(TargetMonster.transform.position);
                }
            }

            return true;
        }
    }
}