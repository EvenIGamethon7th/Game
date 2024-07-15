using System;
using _2_Scripts.Game.Unit;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.StatusEffect
{
    using _2_Scripts.Game.Monster;
    public abstract class StatusEffectSO : SerializedScriptableObject
    {

        public enum EDebuffTypes
        {
            Slow,
            Def,
            MDef,
            Bleeding,
            TargetLuckyKey
        }

        [Title("상태이상 이름 키")]
        [SerializeField]
        public LocalizeKey NameKey { get; private set; }
        [Title("상태이상 설명 키")]
        [SerializeField]
        public LocalizeKey DescriptionKey { get; private set; }

        [Title("버프/디버프 종류")]
        [SerializeField]
        public EDebuffTypes DeBuffType { get; private set; }

        [Title("상태이상 지속시간 ")]
        [SerializeField]
        public float Duration { get; private set; }
        [Title("상태이상 아이콘")]
        [SerializeField]
        public Sprite Icon { get; private set; }

        [Title("상태이상 이펙트 효과")] 
        [SerializeField]
        public GameObject HitEffect { get; private set; }
        
        public virtual bool CanApply(MonsterData monsterData) => true;

        public virtual void OnApply(MonsterData monsterData, Monster monster, CUnit attacker) =>
            OnApply(monsterData, monster);
        public abstract void OnApply(MonsterData monsterData, Monster monster);
        public abstract void OnRemove(MonsterData monsterData, Action endCallback = null);
        
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