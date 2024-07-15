using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    using _2_Scripts.Game.Unit.Data;

    public class UnitBuffHandler : MonoBehaviour
    {
        public BuffData BuffData { get; private set; }

        private void OnEnable()
        {
            if (BuffData != null)
            {
                BuffData.Clear();
            }
            BuffData = MemoryPoolManager<BuffData>.CreatePoolingObject();
        }
    }
}