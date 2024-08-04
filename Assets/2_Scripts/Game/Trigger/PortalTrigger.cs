using _2_Scripts.Game.Monster;
using Cysharp.Threading.Tasks;
using Rito.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace _2_Scripts.Trigger
{
    public class PortalTrigger : MonoBehaviour
    {
        [GetComponent]
        private BoxCollider2D mCollider;
        private CancellationTokenSource mCts = new ();
        private Action<Collider2D> mSkill;

        private void Awake()
        {
            mCollider = GetComponent<BoxCollider2D>();
        }

        public void Init(float range, float duration, Action<Collider2D> action)
        {
            mSkill += action;
            gameObject.SetActive(true);
            mCollider.size = Vector2.one * range * 2;
            ActiveFalseAsync(duration, action).Forget();
            mCollider.enabled = true;
        }

        private async UniTask ActiveFalseAsync(float time, Action<Collider2D> action)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: mCts.Token);
            mSkill -= action;
            if (gameObject == null) return;
            gameObject.SetActive(false);
            mCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            mSkill?.Invoke(collision);
        }
    }
}