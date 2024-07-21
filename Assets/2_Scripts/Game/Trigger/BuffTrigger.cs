using System;
using System.Threading;
using _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _2_Scripts.Buff
{
    public class BuffTrigger : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider2D mCollider;

        private PassiveBuff mPassiveBuff;

        private CancellationTokenSource mCts = new CancellationTokenSource();

        public void Init(PassiveBuff passiveBuff, float duration = 0)
        {
            mPassiveBuff = passiveBuff;
            mCollider.size = Vector2.one * passiveBuff.Range * 2;
            mCollider.enabled = true;
            if (duration != 0)
                ActiveFalseAsync(duration).Forget();
        }

        public void Clear()
        {
            gameObject.SetActive(false);
            mCollider.enabled = false;
            mPassiveBuff = null;
        }

        private async UniTask ActiveFalseAsync(float duration)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: mCts.Token);
            Clear();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var layer = 1 << collision.gameObject.layer;
            if (collision.gameObject.layer != mPassiveBuff.TargetLayer &&
                layer != mPassiveBuff.TargetLayer) return;
            mPassiveBuff.AddPassive(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var layer = 1 << collision.gameObject.layer;
            if (collision.gameObject.layer != mPassiveBuff.TargetLayer &&
                layer != mPassiveBuff.TargetLayer) return;
            mPassiveBuff.RemovePassive(collision);
        }
    }
}