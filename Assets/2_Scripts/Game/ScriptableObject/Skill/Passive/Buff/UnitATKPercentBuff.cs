using _2_Scripts.Game.Unit.Data;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff
{
    [CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/Buff/ATKRate")]
    public class UnitATKPercentBuff : UnitPassiveBuff
    {
        [Title("������ ���� �ۼ�Ʈ")]
        [SerializeField]
        private float mPercent;

        public override void AddPassive(BuffData data)
        {
            data.ATKRate += mPercent;
        }

        public override void RemovePassive(BuffData data)
        {
            data.ATKRate -= mPercent;
        }
    }
}