using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive
{
    public abstract class PassiveSkill : SerializedScriptableObject
    {
        public enum EPassiveType
        {
            BeforeDefaultAttack,
            AfterDefaultAttack,
            Buff,
        }

        [Title("��ų �̸� Ű")]
        [SerializeField]
        public LocalizeKey NameKey { get; private set; }

        [Title("��ų ���� Ű")]
        [SerializeField]
        public LocalizeKey DescriptionKey { get; private set; }

        [Title("��ų ������")]
        [SerializeField]
        public Sprite Icon { get; private set; }

        [Title("��ų ��Ÿ�")]
        [SerializeField]
        public float Range { get; private set; }
        
        [SerializeField]
        [Title("�нú� Ÿ��")]
        public EPassiveType PassiveType { get; private set; }

        [Title("���� ȿ�� ��ƼŬ")]
        [SerializeField]
        public GameObject CastEffect { get; private set; }
        [Title("���� ȿ�� ��ƼŬ")]
        [SerializeField]
        public GameObject HitEffect { get; private set; }

        [Title("Ÿ�� ���̾�")]
        [SerializeField]
        public LayerMask TargetLayer { get; private set; }
    }
}