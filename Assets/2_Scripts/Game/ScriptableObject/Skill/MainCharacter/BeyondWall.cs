using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.MainCharacter
{
    using _2_Scripts.Game.Unit;
    using _2_Scripts.Trigger;
    using Sirenix.OdinInspector;
    [CreateAssetMenu(menuName = "ScriptableObject/MainCharacterSkill/BeyondWall")]
    public class BeyondWall : Skill
    {
        [SerializeField]
        [Title("지속 시간 ( 1초당 1)")]
        private float mLifeTime;

        [SerializeField]
        [Title("스킬 트리거")]
        private GameObject mPortal;

        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster.Monster[]> beforeDamage = null, Action<Monster.Monster> passive = null)
        {
            var trigger = ObjectPoolManager.Instance.CreatePoolingObject(mPortal, ownerTransform.position).GetComponent<PortalTrigger>();
            trigger.Init(Range, mLifeTime, SummonWall);

            return true;
        }

        private void SummonWall(Collider2D collision)
        {
            int layer = 1 << collision.gameObject.layer;

            if (layer == TargetLayer)
            {
                if (!collision.TryGetComponent<Monster.Monster>(out var monster)) return;
                if (!monster.IsBoss)
                    monster.TakeDamage(monster.GetMonsterData.MaxHp, Utils.Define.EAttackType.TrueDamage, Utils.Define.EInstantKillType.Transition);
            }
        }
    }
}