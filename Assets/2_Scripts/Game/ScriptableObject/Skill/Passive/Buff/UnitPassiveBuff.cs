using _2_Scripts.Game.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff
{
    using _2_Scripts.Game.Unit.Data;

    public abstract class UnitPassiveBuff : PassiveBuff
    {
        public override sealed void AddPassive(Collider2D collision)
        {
            if (!collision.TryGetComponent<UnitBuffHandler>(out var handler)) return;
            
            AddPassive(handler.BuffData);
        }

        public override sealed void RemovePassive(Collider2D collision)
        {
            if (!collision.TryGetComponent<UnitBuffHandler>(out var handler)) return;
            
            RemovePassive(handler.BuffData);
        }

        public abstract void AddPassive(BuffData data);

        public abstract void RemovePassive(BuffData data);
    }
}