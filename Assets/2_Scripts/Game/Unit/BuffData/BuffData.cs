using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.Unit.Data
{
    public class BuffData : IPoolable
    {
        public bool IsActive { get => mIsActive; set => mIsActive = value; }
        private bool mIsActive;

        public float ATK;
        public float ATKRate;
        public float MATK;
        public float MATKRate;
        public float ATKSpeed;
        public float ATKSpeedRate;

        public BuffData()
        {

        }

        public void Clear()
        {
            mIsActive = false;
            ATK = 0;
            ATKSpeed = 0;
            ATKRate = 0;
            MATK = 0;
            MATKRate = 0;
            ATKSpeedRate = 0;
        }
    }
}