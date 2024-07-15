using _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff;
using _2_Scripts.Game.Unit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _2_Scripts.Buff
{
    public class BuffTrigger : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider2D mCollider;

        private PassiveBuff mPassiveBuff;

        public void Init(PassiveBuff passiveBuff)
        {
            mPassiveBuff = passiveBuff;
            mCollider.size = Vector2.one * passiveBuff.Range * 2;
            mCollider.enabled = true;
        }

        public void Clear()
        {
            gameObject.SetActive(false);
            mCollider.enabled = false;
            mPassiveBuff = null;
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