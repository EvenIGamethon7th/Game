using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.StatusEffect
{
    public abstract class StatusEffect : SerializedScriptableObject
    {
        [Title("상태이상 이름 키")]
        [SerializeField]
        public LocalizeKey NameKey { get; private set; }
        [Title("상태이상 설명 키")]
        [SerializeField]
        public LocalizeKey DescriptionKey { get; private set; }
        [Title("상태이상 지속시간 ")]
        [SerializeField]
        public float Duration { get; private set; }
        [Title("상태이상 아이콘")]
        [SerializeField]
        public Sprite Icon { get; private set; }

        [Title("상태이상 이펙트 효과")] 
        [SerializeField]
        public GameObject HitEffect { get; private set; }


        protected Action mRemoveCallback;
        public abstract void OnApply(Action removeCallback);
        protected abstract void OnRemove();

        protected virtual async UniTaskVoid ExecuteAfterDuration()
        {
            await UniTask.WaitForSeconds(Duration);
            OnRemove();
            mRemoveCallback.Invoke();
        }
    }
}