using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.MainCharacter
{
    using DirectionSkill;

    public class BeyondWall : DirectionSkill
    {
        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster.Monster[]> beforeDamage = null, Action<Monster.Monster> passive = null)
        {
            return true;
        }
    }
}