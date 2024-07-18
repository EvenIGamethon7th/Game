using _2_Scripts.Game.ScriptableObject.Skill.Passive;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Character
{
    using Skill;
    [CreateAssetMenu(menuName = "ScriptableObject/MainCharacter")]
    public class MainCharacterInfo : SerializedScriptableObject
    {
        [Title("캐릭터 진화 정보")]
        [SerializeField]
        public Dictionary<int, CharacterKey> CharacterEvolutions { get; private set; }

        [Title("액티브 스킬 리스트")]
        [SerializeField]
        public List<SkillInfo> ActiveSkillList { get; private set; }

        [Title("패시브 스킬 리스트")]
        [SerializeField]
        public List<PassiveSkillInfo> PassiveSkillList { get; private set; }
    }
}