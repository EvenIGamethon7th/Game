using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.StatusEffect
{
    public abstract class StatusEffectSO : SerializedScriptableObject
    {
        public enum EDebuffTypes
        {
            Slow,
            Def,
            MDef
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


        public abstract bool CanApply();
        public abstract void OnApply();
        public abstract void OnRemove();

        protected virtual async UniTaskVoid ExecuteAfterDuration(Action endCallback = null)
        {
            await UniTask.WaitForSeconds(Duration);
            OnRemove();
            endCallback?.Invoke();
        }
    }
}