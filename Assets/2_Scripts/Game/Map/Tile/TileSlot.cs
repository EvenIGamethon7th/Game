using _2_Scripts.Game.Unit;
using UnityEngine;

namespace _2_Scripts.Game.Map.Tile
{
    public class TileSlot : MonoBehaviour
    {
        public UnitGroup OccupantUnit { get; private set; }
        public CharacterData CurrentUnitData { get; private set; }
        public float TileZ { get; set; }

        public bool IsNormalUnit { get; private set; } = true;

        public void Init(UnitGroup unitGroup, CUnit unit, bool isNormalUnit = true)
        {
            OccupantUnit = unitGroup;
            OccupantUnit.AddUnit(unit);
            OccupantUnit.transform.position = new Vector3(OccupantUnit.transform.position.x, OccupantUnit.transform.position.y, TileZ);
            CurrentUnitData = unit.CharacterDatas;
            IsNormalUnit = isNormalUnit;
        }

        public void SetOccupantUnit(UnitGroup unitGroup)
        {
            OccupantUnit = unitGroup;
            CurrentUnitData = OccupantUnit?.GetCharacterData();
            if (OccupantUnit != null)
            {
                OccupantUnit.transform.position = new Vector3(OccupantUnit.transform.position.x, OccupantUnit.transform.position.y, TileZ);
                OccupantUnit.MoveGroup(this);
            }
        }

        public bool CanAddUnit()
        {
            if (OccupantUnit == null)
                return true;
            return OccupantUnit.CanAddUnit();
        }

        public void Clear(bool isDestroy = false)
        {
            OccupantUnit?.Clear(isDestroy);
            CurrentUnitData = null;
            OccupantUnit = null;
            //gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Clear(true);
        }
    }
}