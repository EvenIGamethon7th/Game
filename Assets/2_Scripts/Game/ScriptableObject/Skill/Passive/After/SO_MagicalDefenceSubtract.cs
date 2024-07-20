using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive
{
    [CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/MagicDefenceSubtract")]
    public class SO_MagicalDefenceSubtract : AfterPassive
    {
        [Title("차감할 마법 방어력 퍼센트")] [SerializeField] private float mPercent; 
        public override void AfterDamage(Monster.Monster monsters)
        {
            if(monsters.DefenceFlag)
                return;
            monsters.GetMonsterData.AddMagicDefenceStat(-monsters.GetMonsterData.mdef * (mPercent * 0.01f));
            monsters.DefenceFlag = true;
        }
    }
}