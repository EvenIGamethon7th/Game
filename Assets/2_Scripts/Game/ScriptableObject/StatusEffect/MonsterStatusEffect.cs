using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace _2_Scripts.Game.StatusEffect
{
    public class MonsterStatusEffect : IPoolable
    {
        public StatusEffectSO.EDebuffTypes DeBuffType { get; private set; }
        public float Duration { get; set; }
        public bool IsActive { get => mIsActive;  set => mIsActive = value; }
        private bool mIsActive;

        protected CancellationTokenSource mCts;
        private Action mRemoveCallback;

        public MonsterStatusEffect()
        {
            mCts = new CancellationTokenSource();
        }

        public MonsterStatusEffect(StatusEffectSO.EDebuffTypes deBuffType, float duration, Action removeCallback = null)
        {
            Init(deBuffType, duration, removeCallback);
        }

        public void Init(StatusEffectSO.EDebuffTypes deBuffType, float duration, Action removeCallback)
        {
            DeBuffType = deBuffType;
            Duration = duration;
            mCts = new CancellationTokenSource();
            mRemoveCallback = removeCallback;
            mIsActive = true;
        }

        public void Clear()
        {
            CancelAndDisposeToken();
            mRemoveCallback?.Invoke();
            mRemoveCallback = null;
            mIsActive = false;
        }
        
        private void CancelAndDisposeToken()
        {
            if (mCts != null)
            {
                if (!mCts.IsCancellationRequested)
                {
                    mCts.Cancel();
                }
                mCts.Dispose();
                mCts = null;
            }
            else
            {
                mCts = new CancellationTokenSource();
            }
        }
    }
    

}