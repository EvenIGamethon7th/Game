using _2_Scripts.Game.Unit;
using _2_Scripts.Game.Unit.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff
{
    [CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/Buff/ATKSpeedBuff")]
    public class UnitATKSpeedBuff : UnitPassiveBuff
    {
        [Title("공속 증가 퍼센트")]
        [SerializeField]
        private float mPercent;
        public override void AddPassive(UnitBuffHandler handler)
        {
            handler.AddCount(mBuffType);

            if (handler.GetCount(mBuffType) == 1)
            {
                handler.BuffData.ATKSpeedRate += mPercent;
            }
        }

        public override void RemovePassive(UnitBuffHandler handler)
        {
            handler.RemoveCount(mBuffType);

            if (handler.GetCount(mBuffType) == 0)
            {
                handler.BuffData.ATKSpeedRate -= mPercent;
            }
        }
    }
}