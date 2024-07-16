using _2_Scripts.Game.Unit.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff
{
    [CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/Buff/MagicalAtkAddRate")]
    public class UnitMagicalAtkAddRate : UnitPassiveBuff
    {
        
        [Title("마법 공격력 증가 퍼센트")]
        [SerializeField]
        private float mPercent;
        public override void AddPassive(BuffData data)
        {
            data.MATKRate += mPercent;
        }

        public override void RemovePassive(BuffData data)
        {
            data.MATKRate -= mPercent;
        }
    }
}