﻿using _2_Scripts.Game.Unit;
using UnityEngine;

namespace _2_Scripts.Game.Map.Tile
{
    public class TileSlot : MonoBehaviour
    {
        public UnitGroup OccupantUnit { get; private set; }
        public EUnitClass CurrentUnitClass { get; private set; } = EUnitClass.None;
        public EUnitRank CurrentUnitRank { get; private set; } = EUnitRank.None;

        public void Init(UnitGroup unitGroup, CUnit unit)
        {
            OccupantUnit = unitGroup;
            OccupantUnit.AddUnit(unit);
            CurrentUnitClass = unit.CurrentUnitClass;
            CurrentUnitRank = unit.CurrentUnitRank;
        }

        public void SetOccupantUnit(UnitGroup unitGroup)
        {
            OccupantUnit = unitGroup;
            if (OccupantUnit != null)
            {
                OccupantUnit.MoveGroup(this);
            }

            else
            {
                CurrentUnitClass = EUnitClass.None;
                CurrentUnitRank = EUnitRank.None;
            }
        }

        public bool CanAddUnit()
        {
            if (OccupantUnit == null)
                return true;
            return OccupantUnit.CanAddUnit();
        } 
    }
}