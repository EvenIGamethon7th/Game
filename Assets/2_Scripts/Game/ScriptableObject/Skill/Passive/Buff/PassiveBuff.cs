using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff
{
    public abstract class PassiveBuff : PassiveSkill
    {
        public abstract void AddPassive(Collider2D collision);
        public abstract void RemovePassive(Collider2D collision);
    }
}