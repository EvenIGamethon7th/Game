using System;
using UnityEngine;

namespace _2_Scripts.Game.Map
{
    public class WayPoint : MonoBehaviour
    {
        [SerializeField] private Vector3[] mPoints;

        private Vector3 mCurrenPoistion;
        
        public int GetWayPointCount()
        {
            return mPoints.Length;
        }
        
        public Vector3 GetWayPointPosition(int index)
        {
            return mPoints[index] + mCurrenPoistion;
        }

        private void Start()
        {
            mCurrenPoistion = transform.position;
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < mPoints.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(mPoints[i] + mCurrenPoistion,0.5f);
                if (i < mPoints.Length - 1)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(mPoints[i] + mCurrenPoistion, mPoints[i + 1] + mCurrenPoistion);
                }
            }
        }
    }
}