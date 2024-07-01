using System;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.Unit;
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

        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
            mMatController = GetComponent<MatController>();
        }

        public void SpawnMonster(string key,WayPoint waypoint)
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
            NextWayPoint();
        }
        
        public void TakeDamage(float damage)
        {
            mMonsterData.hp -= damage;
            if (mMonsterData.hp <= 0)
            {
                //TODO Monster Die
                gameObject.SetActive(false);
            }
        }

        private void NextWayPoint()
        {
            if(++mWayPointIndex == mWayPoint.GetWayPointCount())
            {
                //TODO Monster Reach End Point Player Damage!
                
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