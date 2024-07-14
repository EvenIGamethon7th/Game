using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive
{
    using _2_Scripts.Game.Monster;

    public abstract class AfterPassive : PassiveSkill
    {
        public abstract void AfterDamage(Monster monsters);
    }
}