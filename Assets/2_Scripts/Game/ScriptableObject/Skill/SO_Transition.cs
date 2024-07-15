using _2_Scripts.Game.Unit;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    [CreateAssetMenu(menuName = "ScriptableObject/Skill/Transition")]
    public class SO_Transition : Skill
    {
        [Title("�⺻ ��Ÿ��� ������")]
        [SerializeField]
        public bool FollowDefaultRange { get; private set; }

        [SerializeField]
        [Title("������ ���� �ۼ�Ʈ")]
        private float mPercentDamage;

        [SerializeField]
        [Title("��ų ���� ( 1�ʸ� 1)")]
        private float mLifeTime;

        [SerializeField]
        [Title("���� ����Ʈ")]
        private GameObject mTransition;

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

            TransitionAsync(monsterArray, ownerData, afterDamage).Forget();

            return true;
        }

        private async UniTask TransitionAsync(Monster.Monster[] monsterArray, CharacterData ownerData, Action<Monster.Monster> afterDamage = null)
        {
            float totalDamage = ownerData.GetTotalDamageToType(AttackType) * mPercentDamage * 0.01f;

            var monster = monsterArray.Where(monster => !monster.IsDead).FirstOrDefault();
            if (monster == null) return;

            float time = HitEffectPlayAndGetLength(monster.transform.position);

            await UniTask.Delay(TimeSpan.FromSeconds(time));

            if (!monster.IsDead)
                monster.TakeDamage(totalDamage, AttackType);
            
            var detectingTargets = Physics2D.OverlapCircleAll(monster.transform.position, Range, TargetLayer);

            if (detectingTargets.Length == 0) return;

            var transitionMonsterArray = detectingTargets
                .Select(collider => collider.GetComponent<Monster.Monster>())
                .Where(monster => monster != null)
                .ToArray();

            int count = transitionMonsterArray.Length < MaxHitUnit ? transitionMonsterArray.Length : MaxHitUnit;

            if (count == 0) return;

            for (int i = 0; i < count; ++i)
            {
                var line = ObjectPoolManager.Instance.CreatePoolingObject(mTransition, Vector2.zero).GetComponent<LightLine>();
                line.LightingTransition(monster.transform.position, transitionMonsterArray[i].transform.position, mLifeTime).Forget();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(mLifeTime));

            for (int i = 0; i < count; ++i)
            {
                if (!transitionMonsterArray[i].IsDead)
                {
                    transitionMonsterArray[i].TakeDamage(totalDamage, AttackType);
                }
            }
        }
    }
}