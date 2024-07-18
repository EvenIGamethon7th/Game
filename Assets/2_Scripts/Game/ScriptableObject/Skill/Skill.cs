using System.Collections.Generic;
using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    using _2_Scripts.Game.Monster;
    using _2_Scripts.Utils.Components;
    using StatusEffect;
    using System;

    public abstract class Skill : SerializedScriptableObject
    {
        [Title("스킬 이름 키")]
        [SerializeField]
        public LocalizeKey NameKey { get; private set; }
        [Title("스킬 설명 키")]
        [SerializeField]
        public LocalizeKey DescriptionKey { get; private set; }
        [Title("스킬 아이콘")]
        [SerializeField]
        public Sprite Icon { get; private set; }
        [Title("스킬 사거리")]
        [SerializeField]
        public float Range { get; private set; }
        [SerializeField] 
        [Title("공격 타입")]
        public Define.EAttackType AttackType { get; private set; }
        [Title("최대 적중 유닛 개수 0이면, 범위내 전부")]
        [SerializeField]
        public int MaxHitUnit { get; private set; }
        [Title("시전 효과 파티클")]
        [SerializeField]
        public GameObject CastEffect { get; private set; }
        [Title("적중 효과 파티클")]
        [SerializeField]
        public GameObject HitEffect { get; private set; }

        // 아군 버프와 같은 효과가 있을 수 있기에 LayerMask를 통해 타겟 구분
        [Title("타겟 레이어")] 
        [SerializeField] public LayerMask TargetLayer { get; private set; } 
        
        [Title("버프 또는 디버프 효과")]
        [SerializeField]
        public List<StatusEffectSO> StatueEffects { get; private set; }
        
        public abstract bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster[]> beforeDamage = null, Action <Monster> afterDamage = null);

        protected virtual void CastEffectPlay(Vector2 position)
        {
            if (CastEffect == null)
            {
                return;
            }
            ObjectPoolManager.Instance.CreatePoolingObject(CastEffect, position);
        }
        
        protected virtual void HitEffectPlay(Vector2 position)
        {
            if (HitEffect == null)
            {
                return;
            }
            ObjectPoolManager.Instance.CreatePoolingObject(HitEffect, position);
        }

        protected virtual float HitEffectPlayAndGetLength(Vector2 position)
        {
            if (HitEffect == null)
            {
                return 0;
            }
            return ObjectPoolManager.Instance.CreatePoolingObject(HitEffect, position).GetComponentInChildren<AnimationEndToDisable>().GetClipLength();
        }
    }
}