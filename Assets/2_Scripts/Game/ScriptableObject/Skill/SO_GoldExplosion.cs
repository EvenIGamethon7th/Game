using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    [CreateAssetMenu(menuName = "ScriptableObject/Skill/GoldExplosion")]
    public class SO_GoldExplosion : SO_MeelAttackSkill
    {
        [Title("���� ��������")]
        [SerializeField]
        private int mEnhanceWave;

        [Title("n�������� ���� ȹ���ϴ� ��差")]
        [SerializeField]
        private int mGoldBefore11Wave;

        [Title("n�������� �Ŀ� ȹ���ϴ� ��差")]
        [SerializeField]
        private int mGoldAfter10Wave;

        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster.Monster[]> beforeDamage = null, Action<Monster.Monster> afterDamage = null)
        {
            bool isSuccess = base.CastAttack(ownerTransform, ownerData, beforeDamage, afterDamage);
            if (isSuccess)
            {
                Debug.Log("GetGold Unique");
                if (StageManager.Instance.WaveCount < mEnhanceWave)
                {
                    GameManager.Instance.UpdateGold(mGoldBefore11Wave);
                }
                else
                {
                    GameManager.Instance.UpdateGold(mGoldAfter10Wave);
                }
            }

            return isSuccess;
        }
    }
}