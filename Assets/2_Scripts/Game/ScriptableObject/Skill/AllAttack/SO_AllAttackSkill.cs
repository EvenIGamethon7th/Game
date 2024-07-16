using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Unit;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.AllAttack
{
    [CreateAssetMenu(menuName = "ScriptableObject/Skill/AllAttack", fileName = "AllAttack_")]
    public class SO_AllAttackSkill : Skill
    {

        [SerializeField] [Title("데미지 증감 퍼센트")] private float mPercentDamage;

        // 수정 하기
        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData,
            Action<Monster.Monster[]> beforeDamage = null,
            Action<Monster.Monster> afterDamage = null)
        {

            float totalDamage = ownerData.GetTotalDamageToType(AttackType) * (mPercentDamage * 0.01f);
            CastEffectPlay(ownerTransform.position);
            AttackAsync(totalDamage, AttackType, afterDamage).Forget();

            return true;
        }

        private async UniTaskVoid AttackAsync(float totalDamage, Define.EAttackType attackType,
            Action<Monster.Monster> afterDamage = null)
        {
            float waitAnim = 0;
            var monsterList = StageManager.Instance.MonsterList;
            monsterList.Where(m => m.gameObject.activeSelf).ToList().ForEach(go =>
            {
                waitAnim = HitEffectPlayAndGetLength(go.transform.position);
            });
            
            await UniTask.Delay(TimeSpan.FromSeconds(waitAnim));
            
            monsterList.Where(m => m.gameObject.activeSelf).ToList().ForEach(monster =>
            {
                if (monster.TakeDamage(totalDamage, attackType))
                {     
                    var statusEffectHandler = monster.gameObject.GetComponent<StatusEffectHandler>();
                    StatueEffects?.ForEach(effect => statusEffectHandler.AddStatusEffect(effect, null));
                    afterDamage?.Invoke(monster);
                }
            });
        }
    }
}