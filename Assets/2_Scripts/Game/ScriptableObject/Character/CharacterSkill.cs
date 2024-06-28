using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Character
{
    using Skill;
    [CreateAssetMenu(menuName = "ScriptableObject/Character",fileName = "Character_")]
    public class CharacterSkill : SerializedScriptableObject
    {
        [Title("기본 공격")]
        public Skill DefaultAttack { get; private set; }
        
        [Title("모든 스킬 리스트")]
        public List<SkillInfo> SkillList { get; private set; }
        
    }

    [Serializable]
    public class SkillInfo
    {
        public float CoolTime { get; private set; }
        public Skill Skill { get; private set; }
        
    }
}