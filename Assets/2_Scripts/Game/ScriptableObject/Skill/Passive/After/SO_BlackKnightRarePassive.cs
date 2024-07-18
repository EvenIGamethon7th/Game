using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive
{[CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/execution")]
    public class SO_BlackKnightRarePassive : AfterPassive
    {
        [Title("처형 체력 비율")] 
        [SerializeField] 
        private float mPercent;
        public override void AfterDamage(Monster.Monster monsters)
        {
            if (monsters.GetMonsterData.hp <= monsters.GetMonsterData.MaxHp * (mPercent* 0.01f))
            {
                // TODO 공격력 1 상승 어떻게?? PlayerData를 받는 작업이 필요할 것 같은데 문제는 
                // 그렇게 해서 받아도 진화, 아카데미 보낼 시 사라질 가능성이 매우 농후함
                monsters.TakeDamage(monsters.GetMonsterData.MaxHp * (mPercent * 0.01f), Define.EAttackType.TrueDamage, Define.EInstantKillType.Execution);
                HitEffectPlay(monsters.transform.position);
            }
        }
    }
}