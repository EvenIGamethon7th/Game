using _2_Scripts.Buff;
using _2_Scripts.Game.ScriptableObject.Skill.Passive;
using _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    [CreateAssetMenu(menuName = "ScriptableObject/MainCharacterSkill/TreeGuard")]
    public class TreeGuard : Skill
    {
        [SerializeField]
        private UnitPassiveBuff mPassiveSkill;

        [SerializeField]
        private float mDuration;

        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster.Monster[]> beforeDamage = null, Action<Monster.Monster> afterDamage = null)
        {
            CastEffectPlay(ownerTransform.position);
            ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_BuffTrigger, ownerTransform.position).GetComponent<BuffTrigger>().Init(mPassiveSkill, mDuration);

            return true;
        }
    }
}