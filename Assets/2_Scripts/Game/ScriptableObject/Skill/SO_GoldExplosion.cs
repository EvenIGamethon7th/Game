using Sirenix.OdinInspector;
using System;
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
                if (StageManager.Instance.WaveCount < mEnhanceWave)
                {
                    IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold, mGoldBefore11Wave);
                }
                else
                {
                    IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold, mGoldAfter10Wave);
                }
            }

            return isSuccess;
        }

        protected override void HitEffectPlay(Vector2 pos)
        {
            var lootingItem = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_LootingItem,
                pos).GetComponent<LootingItem>();

            if (StageManager.Instance.WaveCount < mEnhanceWave)
            {
                lootingItem.CreateItem(EMoneyType.Gold, mGoldBefore11Wave);
            }

            else
            {
                lootingItem.CreateItem(EMoneyType.Gold, mGoldAfter10Wave);
            }
        }
    }
}