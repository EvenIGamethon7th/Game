using _2_Scripts.Game.Monster;
using _2_Scripts.Game.StatusEffect;
using _2_Scripts.Game.Unit;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static _2_Scripts.Utils.Define;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    public class TestMainCharacterSkill : MonoBehaviour
    {
        List<Monster.Monster> monsters = new List<Monster.Monster>();

        [SerializeField]
        private int mTargetLayer;

        [Title("시전 효과 파티클")]
        [field: SerializeField]
        public GameObject CastEffect { get; private set; }

        [field: SerializeField]
        public StatusEffectSO StatueEffect { get; private set; }

        [SerializeField]
        private float mDamage;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer != mTargetLayer) return;

            if (collision.gameObject.TryGetComponent<Monster.Monster>(out var monster))
            {
                monsters.Add(monster);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer != mTargetLayer) return;

            if (collision.gameObject.TryGetComponent<Monster.Monster>(out var monster))
            {
                monsters.Remove(monster);
            }
        }

        public void UseSkill()
        {
            ObjectPoolManager.Instance.CreatePoolingObject(CastEffect, transform.position);
            var arr = monsters.ToArray();
            for (int i = 0; i < arr.Length; ++i)
            {
                var statusEffectHandler = arr[i].gameObject.GetComponent<StatusEffectHandler>();
                statusEffectHandler.AddStatusEffect(StatueEffect);
                arr[i].TakeDamage(mDamage, EAttackType.Physical);
            }
        }
    }
}