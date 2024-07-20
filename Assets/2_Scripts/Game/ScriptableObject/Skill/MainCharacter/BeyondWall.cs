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
        [Title("���� �ð� ( 1�ʸ� 1)")]
        private float mLifeTime;

        [SerializeField]
        [Title("Ʈ���� ������Ʈ")]
        private GameObject mPortal;

        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster.Monster[]> beforeDamage = null, Action<Monster.Monster> passive = null)
        {
            var trigger = ObjectPoolManager.Instance.CreatePoolingObject(mPortal, ownerTransform.position).GetComponent<PortalTrigger>();
            trigger.Init(Range, TargetLayer, mLifeTime);

            return true;
        }
    }
}