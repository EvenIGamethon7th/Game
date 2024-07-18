using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    public class Bless : Skill
    {
        [SerializeField]
        private float mHealAmount;

        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster.Monster[]> beforeDamage = null, Action<Monster.Monster> afterDamage = null)
        {
            GameManager.Instance.UpdateUserHp(-mHealAmount);
            return true;
        }
    }
}