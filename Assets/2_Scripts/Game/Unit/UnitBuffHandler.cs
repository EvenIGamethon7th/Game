using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    using _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff;
    using _2_Scripts.Game.Unit.Data;

    public class UnitBuffHandler : MonoBehaviour
    {
        [field: SerializeField]
        public BuffData BuffData { get; private set; }

        private Dictionary<UnitPassiveBuff.EBuffType, int> mBuffCount = new ();

        private void OnEnable()
        {
            mBuffCount.Clear();

            if (BuffData != null)
            {
                BuffData.Clear();
            }

            BuffData = MemoryPoolManager<BuffData>.CreatePoolingObject();
        }

        public void AddCount(UnitPassiveBuff.EBuffType buffType)
        {
            if (!mBuffCount.ContainsKey(buffType))
            {
                mBuffCount.Add(buffType, 1);
            }

            else
            {
                mBuffCount[buffType] += 1;
            }
        }

        public void RemoveCount(UnitPassiveBuff.EBuffType buffType)
        {
            if (!mBuffCount.ContainsKey(buffType) || mBuffCount[buffType] <= 0) return;

            else
            {
                mBuffCount[buffType] -= 1;
            }
        }

        public int GetCount(UnitPassiveBuff.EBuffType buffType) { return mBuffCount[buffType]; }
    }
}