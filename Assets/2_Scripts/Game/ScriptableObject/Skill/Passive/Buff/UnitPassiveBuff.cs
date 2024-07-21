using _2_Scripts.Game.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff
{
    using _2_Scripts.Game.Unit.Data;
    using Sirenix.OdinInspector;

    public abstract class UnitPassiveBuff : PassiveBuff
    {
        public enum EBuffType
        {
            Amber,
            Angel,
            Flower,
            Bird
        }

        [Title("버프 타입")]
        [SerializeField]
        protected EBuffType mBuffType;

        public override sealed void AddPassive(Collider2D collision)
        {
            if (!collision.TryGetComponent<UnitBuffHandler>(out var handler)) return;
            
            AddPassive(handler);
        }

        public override sealed void RemovePassive(Collider2D collision)
        {
            if (!collision.TryGetComponent<UnitBuffHandler>(out var handler)) return;
            
            RemovePassive(handler);
        }

        public abstract void AddPassive(UnitBuffHandler handler);

        public abstract void RemovePassive(UnitBuffHandler handler);
    }
}