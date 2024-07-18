using _2_Scripts.Demo;
using _2_Scripts.Game.Unit;
using _2_Scripts.Utils;
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
        [Title("데미지 증감 퍼센트")]
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

            var monster = monsterArray.Where(monster => !monster.IsDead).ToArray();

            int count = monster.Length < MaxHitUnit ? monster.Length : MaxHitUnit;
            if (count == 0) return true;

            for (int i = 0; i < count; ++i)
            {
                if (!monster[i].IsBoss)
                {
                    monster[i].TakeDamage(monster[i].GetMonsterData.MaxHp, AttackType, Define.EInstantKillType.Exile);
                    ObjectPoolManager.Instance.CreatePoolingObject(HitEffect, monster[i].transform.position).GetComponent<HomeRunParticle>().SetTextureParticle(monster[i].Renderer.sprite);
                }

                else
                {
                    var statusEffectHandler = monster[i].GetComponent<StatusEffectHandler>();
                    StatueEffects?.ForEach(effect => statusEffectHandler.AddStatusEffect(effect, attacker));
                }
            }

            return true;
        }
    }
}