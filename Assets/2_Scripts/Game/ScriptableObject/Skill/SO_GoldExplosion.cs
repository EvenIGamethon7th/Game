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
        [Title("기준 스테이지")]
        [SerializeField]
        private int mEnhanceWave;

        [Title("n스테이지 전에 획득하는 골드량")]
        [SerializeField]
        private int mGoldBefore11Wave;

        [Title("n스테이지 후에 획득하는 골드량")]
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
                    GameManager.Instance.UpdateMoney(EMoneyType.Gold,mGoldBefore11Wave);
                }
                else
                {
                    GameManager.Instance.UpdateMoney(EMoneyType.Gold,mGoldAfter10Wave);
                }
            }

            return isSuccess;
        }
    }
}