using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.StatusEffect;
using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using _2_Scripts.Utils;
using JetBrains.Annotations;
using Rito.Attributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

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

        private const EGameMessage PLAYER_DAMAGE = EGameMessage.PlayerDamage;
        private const EGameMessage BOSS_DEATH = EGameMessage.BossDeath;
        private GameMessage<float> mDamageMessage;
        public bool IsBoss { get; private set; }

        private UI_MonsterCanvas mHpCanvas;
        private Collider2D mTrigger;


        private Action<Monster> DamageActionCallback;
        private List<StatusEffectSO> mTargetStatusEffectList = new ();
        public SpriteRenderer Renderer { get; private set; }

        // Monster 방깍 한 번만 받기 위한 플래그
        public  bool DefenceFlag = false;
        public bool IsDead => mMonsterData.hp <= 0;
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
            mHpCanvas = GetComponentInChildren<UI_MonsterCanvas>();
            mTrigger = GetComponent<Collider2D>();
            Renderer = GetComponent<SpriteRenderer>();
            Enabled(false);
        }

        public void SpawnMonster(string key,WayPoint waypoint,bool isBoss)
        {
            var originData = DataBase_Manager.Instance.GetMonster.GetData_Func(key);
            mMonsterData = global::Utils.DeepCopy(originData);
            mMonsterData.MaxHp = mMonsterData.hp;
            mHpCanvas.InitHpSlider(mMonsterData.hp, isBoss);
            //TODO Sprite Change And Animation
            ResourceManager.Instance.Load<RuntimeAnimatorController>(originData.nameKey,
            (controller) =>
            {
                mAnimator.runtimeAnimatorController = controller;
            });
            mWayPoint = waypoint;
            mWayPointIndex = 0;
            NextWayPoint();
            mMatController.RunDissolve(true, () => {
                IsBoss = isBoss;
                mDamageMessage = new GameMessage<float>(PLAYER_DAMAGE, mMonsterData.atk);
                Enabled(true);
            });
        }

        public bool TakeDamage(float damage, Define.EAttackType attackType, bool isExile = false)
        {
            if (mMonsterData.hp <= 0) return false;
            ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_DamageCanvas, transform.position + Vector3.up).GetComponent<UI_DamageCanvas>().SetDamage(damage);
            mMonsterData.hp -= DefenceCalculator.CalculateDamage(damage, mMonsterData, attackType);
            DamageActionCallback?.Invoke(this);
            mHpCanvas.SetHpSlider(mMonsterData.hp);
            if (mMonsterData.hp <= 0)
            {
                //TODO Monster Die
                if (!isExile) GameManager.Instance.UpdateGold(mMonsterData.reward);

                mMatController.RunDissolve(false, () => gameObject.SetActive(false));
                if (IsBoss)
                {
                    MessageBroker.Default.Publish(BOSS_DEATH);
                }
                Enabled(false);
            }

            return true;
        }

        private void NextWayPoint()
        {
            if(++mWayPointIndex == mWayPoint.GetWayPointCount())
            {
                MessageBroker.Default.Publish(mDamageMessage);
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
        }
        
        private void FlipSprite(Vector3 direction)
        {
            Renderer.flipX = direction.x > 0;
        }

        private void Enabled(bool bEnable)
        {
            mTrigger.enabled = bEnable;
            mHpCanvas.gameObject.SetActive(bEnable);
            enabled = bEnable;
            DefenceFlag = false;
        }
    }
}