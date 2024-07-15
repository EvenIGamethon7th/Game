using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.Unit.Data
{
    [Serializable]
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
            ATK = 0;
            ATKSpeed = 0;
            ATKRate = 100;
            MATK = 0;
            MATKRate = 100;
            ATKSpeedRate = 100;
        }

        public void Clear()
        {
            mIsActive = false;
            ATK = 0;
            ATKSpeed = 0;
            ATKRate = 100;
            MATK = 0;
            MATKRate = 100;
            ATKSpeedRate = 100;
        }
    }
}