using _2_Scripts.Game.Unit;
using _2_Scripts.Game.Unit.Data;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff
{
    [CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/Buff/MnATKRate")]
    public class UnitMnATKPrecentBuff : UnitPassiveBuff
    {
        [Title("데미지 증가 퍼센트")]
        [SerializeField]
        private float mPercent;

        public override void AddPassive(UnitBuffHandler handler)
        {
            handler.AddCount(mBuffType);

            if (handler.GetCount(mBuffType) == 1)
            {
                handler.BuffData.ATKRate += mPercent;
                handler.BuffData.MATKRate += mPercent;
            }
        }

        public override void RemovePassive(UnitBuffHandler handler)
        {
            handler.RemoveCount(mBuffType);

            if (handler.GetCount(mBuffType) == 0)
            {
                handler.BuffData.ATKRate -= mPercent;
                handler.BuffData.MATKRate -= mPercent;
            }
        }
    }
}