using _2_Scripts.Game.Unit;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    [CreateAssetMenu(menuName = "ScriptableObject/Skill/HomeRun")]
    public class SO_HomeRun : Skill
    {
        [SerializeField]
        [Title("������ ���� �ۼ�Ʈ")]
        private float mPercentDamage;

        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster.Monster[]> beforeDamage = null, Action<Monster.Monster> afterDamage = null)
        {
            var detectingTargets = Physics2D.OverlapCircleAll(ownerTransform.position, ownerData.range, TargetLayer);
            if (detectingTargets.Length == 0)
            {
                return false;
            }

            var monsterArray = detectingTargets
                .Select(collider => collider.GetComponent<Monster.Monster>())
                .Where(monster => monster != null)
                .ToArray();

            beforeDamage?.Invoke(monsterArray);
            CastEffectPlay(ownerTransform.position);
            var attacker = ownerTransform.GetComponent<CUnit>();
            attacker.SetFlipUnit(detectingTargets[0].transform);

            var monster = monsterArray.Where(monster => !monster.IsDead).FirstOrDefault();
            if (monster == null) return true;
            return true;
        }
    }
}