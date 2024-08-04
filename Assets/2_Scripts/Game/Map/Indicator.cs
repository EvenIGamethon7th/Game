using _2_Scripts.Game.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _2_Scripts.Game.Map
{
    public class Indicator : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer mLine;
        [SerializeField]
        private GameObject mUnitMark;
        [SerializeField] 
        private GameObject mDstMark;

        public void SetUnitMarkPos(Vector3 pos)
        {
            mUnitMark.SetActive(true);
            mUnitMark.transform.position = pos;
        }

        public void SetIndicator(Vector3 src, Vector3 dst)
        {
            SetActive();
            mUnitMark.transform.position = src;
            mDstMark.transform.position = dst;
            mLine.SetPosition(0, src);
            mLine.SetPosition(1, dst);
        }

        public void SetActive(bool bActive = true)
        {
            mUnitMark.SetActive(bActive);
            mDstMark.SetActive(bActive);
            mLine.gameObject.SetActive(bActive);
        }

        public Vector3 GetDestinationPosition => mLine.GetPosition(1);
    }
}