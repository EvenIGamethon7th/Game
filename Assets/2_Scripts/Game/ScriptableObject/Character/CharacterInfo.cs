using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Character
{
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
        
        [Title("모든 스킬 리스트")]
        [SerializeField]
        public List<SkillInfo> SkillList { get; private set; }
        
    }

    [Serializable]
    public class SkillInfo
    {
        [SerializeField]    
        public float CoolTime { get; private set; }
        [SerializeField]
        public Skill Skill { get; private set; }
        
    }
}