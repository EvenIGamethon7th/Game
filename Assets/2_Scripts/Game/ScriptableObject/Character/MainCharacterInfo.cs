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
        [Title("ĳ���� ��ȭ ����")]
        [SerializeField]
        public Dictionary<int, CharacterKey> CharacterEvolutions { get; private set; }

        [Title("��ų ����Ʈ")]
        [SerializeField]
        public List<SkillInfo> SkillList { get; private set; }
    }
}