using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.StatusEffect;
using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using _2_Scripts.UI.Ingame;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Rito.Attributes;
using Sirenix.Utilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static _2_Scripts.Utils.Define;

namespace _2_Scripts.Game.Monster
{
    public class Monster : MonoBehaviour
    {
        private Animator mAnimator;
        private MatController mMatController;
        private MonsterData mMonsterData;
        public MonsterData GetMonsterData  => mMonsterData;

        private WayPoint mWayPoint;
        private int mWayPointIndex = 0;
        [field: SerializeField]
        public Vector3 NextWayPointVector { get; private set; }

        private const EGameMessage BOSS_DEATH = EGameMessage.BossDeath;
        public bool IsBoss { get; private set; }

        public IMonsterHpUI CurrentHpCanvas { get; set; }

        private Collider2D mTrigger;


        private Action<Monster> DamageActionCallback;
        private List<StatusEffectSO> mTargetStatusEffectList = new ();
        public SpriteRenderer Renderer { get; private set; }

        private List<IDamagebleAction> mDamagebleActions;
        private Action damagebleActions;

        public bool IsLastBoss = false;
        // Monster 방깍 한 번만 받기 위한 플래그
        public  bool DefenceFlag = false;
        public bool IsDead => mMonsterData.hp <= 0;

        private GameMessage<Monster> mMonsterMessage;

        public void DamageActionAdd(Action<Monster> action,StatusEffectSO so)
        {
            if (mTargetStatusEffectList.Contains(so))
            {
                return;
            }
            DamageActionCallback += action;
            mTargetStatusEffectList.Add(so);
        }
        public void DamageActionRemove(Action<Monster> action,StatusEffectSO so)
        {
            if (!mTargetStatusEffectList.Contains(so))
            {
                return;
            }
            DamageActionCallback -= action;
            mTargetStatusEffectList.Remove(so);
        }

        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
            mMatController = GetComponent<MatController>();
            mTrigger = GetComponent<Collider2D>();
            Renderer = GetComponent<SpriteRenderer>();
            Enabled(false);
            mDamagebleActions = new List<IDamagebleAction>(GetComponents<IDamagebleAction>());
            mDamagebleActions.ForEach(action => damagebleActions += action.DamageAction());
            mMonsterMessage = new GameMessage<Monster>(EGameMessage.MonsterHp, this);
        }
        
        public void SpawnMonster(string key,WayPoint waypoint,bool isBoss,WaveStatData waveStatData,float statWeight,bool isLastBoss)
        {
            var monsterData = DataBase_Manager.Instance.GetMonster.GetData_Func(key);
            mMonsterData = MemoryPoolManager<MonsterData>.CreatePoolingObject();
            mMonsterData.Init(waveStatData,statWeight);
            this.IsLastBoss = isLastBoss;

            mAnimator.speed = 0;
            //TODO Sprite Change And Animation
            ResourceManager.Instance.Load<RuntimeAnimatorController>(monsterData.image,
            (controller) =>
            {
                mAnimator.runtimeAnimatorController = controller;
            });
            mWayPoint = waypoint;
            mWayPointIndex = 0;
            NextWayPoint();
            WaitLoadSprite(() =>
            {
                mMatController.RunDissolve(true, () => {
                    IsBoss = isBoss;
                    MessageBroker.Default.Publish(mMonsterMessage);
                    CurrentHpCanvas.InitHpUI(mMonsterData.MaxHp);
                    Enabled(true);
                    mAnimator.speed = 1;
                });
            }).Forget();
        }
        
        private async UniTaskVoid WaitLoadSprite(Action callbackAction)
        {
            await UniTask.WaitUntil(() => Renderer.sprite != null);
            callbackAction.Invoke();
        }

        public bool TakeDamage(float damage, Define.EAttackType attackType, EInstantKillType instant = EInstantKillType.None)
        {
            if (mMonsterData.hp <= 0) 
                return false;
            
            if (instant == EInstantKillType.None)
                ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_DamageCanvas, transform.position + Vector3.up).GetComponent<UI_DamageCanvas>().SetDamage(damage);
            mMonsterData.DamageHp(DefenceCalculator.CalculateDamage(damage, mMonsterData, attackType));
            DamageActionCallback?.Invoke(this);
            damagebleActions?.Invoke();
            CurrentHpCanvas.SetHpUI(mMonsterData.hp);

            if (mMonsterData.hp <= 0)
            {
                CurrentHpCanvas.Active = false;
                if (IsLastBoss)
                {
                    MessageBroker.Default.Publish(new TaskMessage(ETaskList.GameOver));
                }
                
                if (instant != EInstantKillType.Exile)
                {
                    mMonsterData.rewardList
                        .Where(reward=>reward.Value > 0)
                        .ForEach(reward =>
                        {
                            var lootingItem =
                                ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_LootingItem,
                                    transform.position).GetComponent<LootingItem>();
                            lootingItem.CreateItem(reward.Key, reward.Value);
                            IngameDataManager.Instance.UpdateMoney(reward.Key, reward.Value);
                        });
                }

                mMatController.RunDissolve(false, () => gameObject.SetActive(false));
                if (IsBoss)
                {
                    MessageBroker.Default.Publish(BOSS_DEATH);
                }
                StageManager.Instance.RemoveMonster(this);
                Enabled(false);
                ClearData();
            }

            return true;
        }

        private void NextWayPoint()
        {
            if(++mWayPointIndex == mWayPoint.GetWayPointCount())
            {
                if (IsLastBoss)
                {
                    IngameDataManager.Instance.UpdateUserHp(IngameDataManager.Instance.MaxHp);
                }

                else
                {
                    IngameDataManager.Instance.UpdateUserHp((int)mMonsterData.atk);
                }

                Enabled(false);
                gameObject.SetActive(false);
                return;
            }
            NextWayPointVector = mWayPoint.GetWayPointPosition(mWayPointIndex);
        }

        /// <summary>
        /// https://rito15.github.io/posts/unity-update-vs-coroutine-every-frame/ 참고
        /// </summary>
        private void Update()
        {
            if (Vector3.Distance(transform.position, NextWayPointVector) < 0.1f)
            {
                NextWayPoint();
            }

            var position = transform.position;
            Vector3 direction = NextWayPointVector - position;
            FlipSprite(direction);
            // 위치 이동
            position = Vector3.MoveTowards(position, NextWayPointVector, mMonsterData.speed * Time.deltaTime);
            transform.position = position;
            CurrentHpCanvas.UpdatePos(position);
        }
        
        private void FlipSprite(Vector3 direction)
        {
            Renderer.flipX = direction.x > 0;
        }

        private void ClearData()
        {
            mMonsterData.Clear();
        }

        private void Enabled(bool bEnable)
        {
            mTrigger.enabled = bEnable;
            if (!bEnable && CurrentHpCanvas != null)
                CurrentHpCanvas.Active = bEnable;
            enabled = bEnable;
            DefenceFlag = false;
        }
    }
}