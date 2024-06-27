using _2_Scripts.Game.Unit;
using UnityEngine;

namespace _2_Scripts.Game.Map.Tile
{
    public class TileSlot : MonoBehaviour
    {
        public UnitGroup OccupantUnit { get; private set; }
        public void SetOccupantUnit(UnitGroup unitGroup)
        {
            OccupantUnit = unitGroup;
            OccupantUnit.MoveGroup(this);
        }
    }
}