using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive
{
    public abstract class PassiveSkill : SerializedScriptableObject
    {
        public enum EPassiveType
        {
            BeforeDefaultAttack,
            AfterDefaultAttack,
            Buff,
        }

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
        [Title("패시브 타입")]
        public EPassiveType PassiveType { get; private set; }

        [Title("시전 효과 파티클")]
        [SerializeField]
        public GameObject CastEffect { get; private set; }
        [Title("적중 효과 파티클")]
        [SerializeField]
        public GameObject HitEffect { get; private set; }

        [Title("타겟 레이어")]
        [SerializeField]
        public LayerMask TargetLayer { get; private set; }
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
    
    }
}