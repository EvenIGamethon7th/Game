using _2_Scripts.Game.Monster;
using Cysharp.Threading.Tasks;
using Rito.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace _2_Scripts.Trigger
{
    public class PortalTrigger : MonoBehaviour
    {
        [GetComponent]
        private BoxCollider2D mCollider;
        private CancellationTokenSource mCts = new ();
        private LayerMask mMask;

        private void Awake()
        {
            mCollider = GetComponent<BoxCollider2D>();
        }

        public void Init(float range, LayerMask targetLayer, float duration)
        {
            gameObject.SetActive(true);
            mCollider.size = Vector2.one * range * 2;
            ActiveFalseAsync(duration).Forget();
            mMask = targetLayer;
            mCollider.enabled = true;
        }

        private async UniTask ActiveFalseAsync(float time)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: mCts.Token);
            gameObject.SetActive(false);
            mCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            int layer = 1 << collision.gameObject.layer;

            if (layer == mMask)
            {
                if (!collision.TryGetComponent<Monster>(out var monster)) return;
                if (!monster.IsBoss)
                    monster.TakeDamage(monster.GetMonsterData.MaxHp, Utils.Define.EAttackType.TrueDamage, Utils.Define.EInstantKillType.Transition);
            }
        }
    }
}