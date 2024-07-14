using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Character
{
    using _2_Scripts.Game.ScriptableObject.Skill.Passive;
    using Skill;
    [CreateAssetMenu(menuName = "ScriptableObject/Character",fileName = "Character_")]
    public class CharacterInfo : SerializedScriptableObject
    {
        [Title("캐릭터 진화 정보")]
        [SerializeField]
        public Dictionary<int,CharacterKey> CharacterEvolutions { get; private set; }
        
        [Title("기본 공격")]
        [SerializeField]
        public Skill DefaultAttack { get; private set; }
        
        [Title("액티브 스킬 리스트")]
        [SerializeField]
        public List<SkillInfo> ActiveSkillList { get; private set; }

        [Title("패시브 스킬 리스트")]
        [SerializeField]
        public List<PassiveSkillInfo> PassiveSkillList { get; private set; }
    }

    [Serializable]
    public class SkillInfo
    {
        [SerializeField]
        public int Level { get; private set; }
        [SerializeField]    
        public float CoolTime { get; private set; }
        [SerializeField]
        public Skill Skill { get; private set; }
    }

    [Serializable]
    public class PassiveSkillInfo
    {
        [SerializeField]
        public int Level { get; private set; }

        [SerializeField]
        public PassiveSkill Skill { get; private set; }
    }
}