using System;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.Unit;
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

        private WayPoint mWayPoint;
        private int mWayPointIndex = 0;
        private Vector3 mNextWayPoint;

        private const EGameMessage PLAYER_DAMAGE = EGameMessage.PlayerDamage;
        private const EGameMessage BOSS_DEATH = EGameMessage.BossDeath;
        private GameMessage<float> mDamageMessage;
        private bool mbBoss;
        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
            mMatController = GetComponent<MatController>();
        }

        public void SpawnMonster(string key,WayPoint waypoint,bool isBoss)
        {
            var originData = DataBase_Manager.Instance.GetMonster.GetData_Func(key);
            mMonsterData = global::Utils.DeepCopy(originData);
            //TODO Sprite Change And Animation
            ResourceManager.Instance.Load<RuntimeAnimatorController>(originData.nameKey,
            (controller) =>
            {
                mAnimator.runtimeAnimatorController = controller;
            });
            mWayPoint = waypoint;
            mWayPointIndex = 0;
            mMatController.RunDissolve();
            mbBoss = isBoss;
            mDamageMessage = new GameMessage<float>(PLAYER_DAMAGE, mMonsterData.atk);
            NextWayPoint();
        }

        
        public void TakeDamage(float damage)
        {
            Debug.Log($"데미지 받음{damage}");
            mMonsterData.hp -= damage;
            if (mMonsterData.hp <= 0)
            {
                //TODO Monster Die
                if (mbBoss)
                {
                    MessageBroker.Default.Publish(BOSS_DEATH);
                }
                gameObject.SetActive(false);
            }
        }

        private void NextWayPoint()
        {
            if(++mWayPointIndex == mWayPoint.GetWayPointCount())
            {
                MessageBroker.Default.Publish(mDamageMessage);
                gameObject.SetActive(false);
                return;
            }
            mNextWayPoint = mWayPoint.GetWayPointPosition(mWayPointIndex);
        }

        /// <summary>
        /// https://rito15.github.io/posts/unity-update-vs-coroutine-every-frame/ 참고
        /// </summary>
        private void Update()
        {
            if (Vector3.Distance(transform.position, mNextWayPoint) < 0.1f)
            {
                NextWayPoint();
            }

            var position = transform.position;
            Vector3 direction = mNextWayPoint - position;
            FlipSprite(direction);
            // 위치 이동
            position = Vector3.MoveTowards(position, mNextWayPoint, mMonsterData.speed * Time.deltaTime);
            transform.position = position;
        }
        
        private void FlipSprite(Vector3 direction)
        {
            float scaleX = direction.x < 0 ? 1 : -1;
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }
    }
}