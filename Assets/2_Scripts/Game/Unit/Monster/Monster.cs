using System;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using _2_Scripts.Utils;
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
        private bool mbBoss;

        private UI_MonsterCanvas mHpCanvas;
        private Collider2D mTrigger;
        private SpriteRenderer mSpriteRenderer;


        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
            mMatController = GetComponent<MatController>();
            mHpCanvas = GetComponentInChildren<UI_MonsterCanvas>();
            mTrigger = GetComponent<Collider2D>();
            mSpriteRenderer = GetComponent<SpriteRenderer>();
            Enabled(false);
        }

        public void SpawnMonster(string key,WayPoint waypoint,bool isBoss)
        {
            var originData = DataBase_Manager.Instance.GetMonster.GetData_Func(key);
            mMonsterData = global::Utils.DeepCopy(originData);
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
                mbBoss = isBoss;
                mDamageMessage = new GameMessage<float>(PLAYER_DAMAGE, mMonsterData.atk);
                Enabled(true);
            });
        }

        public bool TakeDamage(float damage, Define.EAttackType attackType)
        {
            if (mMonsterData.hp <= 0) return false;
            ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_DamageCanvas, transform.position + Vector3.up).GetComponent<UI_DamageCanvas>().SetDamage(damage);
            mMonsterData.hp -= DefenceCalculator.CalculateDamage(damage, mMonsterData, attackType);
            mHpCanvas.SetHpSlider(mMonsterData.hp);
            if (mMonsterData.hp <= 0)
            {
                //TODO Monster Die
                mMatController.RunDissolve(false, () => gameObject.SetActive(false));
                if (mbBoss)
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
            mSpriteRenderer.flipX = direction.x > 0;
        }

        private void Enabled(bool bEnable)
        {
            mTrigger.enabled = bEnable;
            mHpCanvas.gameObject.SetActive(bEnable);
            enabled = bEnable;
        }
    }
}