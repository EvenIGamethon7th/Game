
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    using _2_Scripts.Game.Monster;
    /// <summary>
    /// 기본 공격 또는 투사체가 없는 공격 스킬
    /// </summary>
    
    [CreateAssetMenu(menuName = "ScriptableObject/Skill/Melee",fileName = "Melee_")]
    public class SO_MeelAttackSkill : Skill
    {

        private CharacterData mOwnerData;
        private Monster mTargetMonster;
        [Title("데미지 증감 퍼센트")]
        [SerializeField]
        public float PercentDamage { get; private set; }
        [Title("기본 사거리를 따른다")]
        [SerializeField]
        public bool FollowDefaultRange { get; private set; }
        
        
        public override void CastAttack(Transform ownerTransform, CharacterData ownerData)
        {
            //mOwnerData = ownerTransform.GetComponent<CharacterData>(); 매번 GetComponent 비용 발생하므로 CastAttack에 참조.
            mOwnerData = ownerData;
            float range = FollowDefaultRange ? ownerData.range : this.Range;
            // 방어력 부분은 몬스터별로 공격력 감소 수식이 있을 수 있으므로 여기선 owner 데미지만 계산.
            float totalDamage = ownerData.atk * (PercentDamage * 0.01f);
            var detectingTargets = Physics2D.OverlapCircleAll(ownerTransform.position, range, TargetLayer);
            if (detectingTargets.Length == 0)
            {
                return;
            }
            var targetCount = MaxHitUnit == 0 ? detectingTargets.Length : MaxHitUnit;
            CastEffectPlay(ownerTransform.position);
            for (int i = 0; i < targetCount; i++)
            {
                mTargetMonster = detectingTargets[i].GetComponent<Monster>();
                if (mTargetMonster == null)
                {
                    continue;
                }
                mTargetMonster.TakeDamage(totalDamage);
                HitEffectPlay(mTargetMonster.transform.position);
            }

        }
    }
}