using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Map.Tile;
using _2_Scripts.Game.Unit;
using _2_Scripts.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapManager : Singleton<MapManager>
{
    [SerializeField] private Tilemap mMap;

    private List<TileSlot> mTileDatas = new();
    private const string TILE_SLOT_NAME = AddressableTable.Default_TileSlot;
    private void Start()
    {
        MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.DefaultResourceLoad).Subscribe(
        _ =>
        {
            CreateInitialTileSlots();
            CreateUnitPool();
        });
    }

    public bool CreateUnit(EUnitClass unitClass, EUnitRank unitRank)
    {
        Debug.Log($"{unitClass} 클래스 {unitRank} 등급 유닛 생성");
        //먼저 같은 유닛 그룹과 그 그룹에 공간이 있는지 확인
        var tileSlot = mTileDatas.Where(x => x.CurrentUnitClass == unitClass && x.CurrentUnitRank == unitRank && x.CanAddUnit()).FirstOrDefault();

        //없다면 빈 타일 슬롯 확인
        if (tileSlot == null)
        {
            tileSlot = mTileDatas.Where(x => x.OccupantUnit == null).FirstOrDefault();
            //그것도 없으면 쩔수없지...
            if (tileSlot == null)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("모든 타일에 유닛이 배치되어 있습니다!");
                return false;
            }

            else
            {
                Debug.Log("그룹 생성 후 유닛 그룹에 유닛 추가");
                var unitGroup = ObjectPoolManager.Instance
                .CreatePoolingObject(AddressableTable.Default_UnitGroup, tileSlot.transform.position)
                .GetComponent<UnitGroup>();
                var unit = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Unit, tileSlot.transform.position).GetComponent<CUnit>();
                unit.Init(unitClass, unitRank);
                tileSlot.Init(unitGroup, unit);
            }
        }

        //해당 유닛 그룹이 있다면 유닛 생성해서 넣기
        else
        {
            var unit = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Unit, tileSlot.transform.position).GetComponent<CUnit>();
            unit.Init(unitClass, unitRank);
            tileSlot.OccupantUnit.AddUnit(unit);

            Debug.Log("그룹이 존재하여 유닛 추가");
        }

        return true;
    }

    public TileSlot GetClickTileSlotDetailOrNull()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero); 
        TileSlot tileSlot = hit.collider != null ? hit.transform.GetComponent<TileSlot>() : null;
        return tileSlot;
    }

    private void CreateUnitPool()
    {
        ObjectPoolManager.Instance.RegisterPoolingObject(AddressableTable.Default_Unit, 100);
    }

    private void CreateInitialTileSlots()
    {
        BoundsInt bounds = mMap.cellBounds;
        Vector3 tileSize = mMap.cellSize;
        Vector3 slotOffset = new Vector3(tileSize.x, tileSize.y, 0);
        for (int x = bounds.xMin; x < bounds.xMax; x += 2)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y += 2)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = mMap.CellToWorld(cellPosition) + slotOffset;
                // 타일 배치
                var tile = ObjectPoolManager.Instance.CreatePoolingObject(TILE_SLOT_NAME, worldPosition).GetComponent<TileSlot>();
                mTileDatas.Add(tile);
            }
        }
    }
}
