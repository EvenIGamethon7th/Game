﻿using System;
using System.Collections.Generic;
using _2_Scripts.Game.Unit;
using Sirenix.Utilities;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    using Monster;
    using StatusEffect;
    public class SkillCollision : MonoBehaviour
    {
        private float mLifeTime;
        private CompositeDisposable disposables = new CompositeDisposable();
        private List<StatusEffectSO> mStatusEffects = new ();
        public void Init(float lifeTime,List<StatusEffectSO> statusEffects)
        {
            mLifeTime = lifeTime;
            mStatusEffects = statusEffects;
            Observable.Timer(TimeSpan.FromSeconds(mLifeTime))
                .Subscribe(_ => gameObject.SetActive(false))
                .AddTo(disposables);
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (mStatusEffects.IsNullOrEmpty())
                return;
            var statusEffectHandler = other.transform.GetComponent<StatusEffectHandler>();
            foreach (var effect in mStatusEffects)
            {
                statusEffectHandler.AddStatusEffect(effect);
            }
        }
        
        private void OnDisable()
        {
            disposables.Clear();
        }
    }
}