using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    public enum EUnitClass
    {
        None,
        Archer,
        Sorcerer,
        Knight
    }

    public enum EUnitRank
    {
        None,
        Normal,
        Rare,
        Unique
    }

    public class CUnit : MonoBehaviour
    {
        public EUnitClass CurrentUnitClass { get; private set; } = EUnitClass.None;
        public EUnitRank CurrentUnitRank { get; private set; } = EUnitRank.None;

        public void Init(EUnitClass unitClass, EUnitRank unitRank)
        {
            CurrentUnitClass = unitClass;
            CurrentUnitRank = unitRank;
        }
    }
}
