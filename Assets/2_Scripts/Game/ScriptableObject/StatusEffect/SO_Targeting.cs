using System;
using _2_Scripts.Game.Unit;
using _2_Scripts.UI.Ingame;
using _2_Scripts.Utils;
using Cargold;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = System.Random;

namespace _2_Scripts.Game.StatusEffect
{
    [CreateAssetMenu(menuName = "ScriptableObject/StatueEffect/Targeting",fileName = "Targeting_")]
    public class SO_Targeting : StatusEffectSO
    {
        [Title("행운 열쇠 얻을 확률")]
        [SerializeField]
        private int mPercent;

        [Title("레벨 제한이 들어가는지?")] 
        [SerializeField]
        private int mLevel;
        public override void OnApply(MonsterData monsterData, Monster.Monster monster,CUnit unit)
        {
            if (unit.CharacterDatas.rank < mLevel)
                return;
            monster.DamageActionAdd(TargetAction,this);
            Observable
                .Timer(TimeSpan.FromSeconds(Duration))
                .TakeUntil(monster.OnDisableAsObservable())
                .TakeUntil(unit.OnDisableAsObservable())
                .Subscribe(
                    _ =>
                    {
                        if (monster.isActiveAndEnabled)
                        {
                            monster.DamageActionRemove(TargetAction,this);
                        }
                    }
                );
        }

        public override void OnApply(MonsterData monsterData, Monster.Monster monster)
        {
        }
        private void TargetAction(Monster.Monster monster)
        {
            if (Random_C.CheckPercent_Func(1000, mPercent))
            {
                var lootingItem = ObjectPoolManager.Instance
                    .CreatePoolingObject(AddressableTable.Default_LootingItem, monster.transform.position, true)
                    .GetComponent<LootingItem>();

                lootingItem.CreateItem(EMoneyType.GoldKey, 1);

                IngameDataManager.Instance.UpdateMoney(EMoneyType.GoldKey,1);
                monster.DamageActionRemove(TargetAction,this);
            }
        }

        public override void OnRemove(MonsterData monsterData, Action endCallback = null)
        {
            endCallback?.Invoke();
        }
    }
}