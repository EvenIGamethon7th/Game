using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    using Monster;
    public class SkillCollision : MonoBehaviour
    {
        private float mLifeTime;
        private float mDamage;
        private GameObject mHitEffect;
        private LayerMask mTargetLayer;
        private CompositeDisposable disposables = new CompositeDisposable();
        
        public void Init(float lifeTime, float damage,LayerMask targetLayer,GameObject hitEffect = null)
        {
            mLifeTime = lifeTime;
            mDamage = damage;
            mHitEffect = hitEffect;
            mTargetLayer = targetLayer;
            
            Observable.Timer(TimeSpan.FromSeconds(mLifeTime))
                .Subscribe(_ => gameObject.SetActive(false))
                .AddTo(disposables);
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
                var monster = other.transform.GetComponent<Monster>();
                if (mHitEffect != null)
                {
                    ObjectPoolManager.Instance.CreatePoolingObject(mHitEffect.name, other.transform.position);
                }
                monster.TakeDamage(mDamage);
        }
        
        private void OnDisable()
        {
            disposables.Clear();
        }
    }
}