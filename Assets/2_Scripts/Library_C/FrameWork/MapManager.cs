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
        MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.ResourceLoad).Subscribe(
        _ =>
        {
            CreateInitialTileSlots();
            TestUnitGroupCreate();
        });
    }

    private void TestUnitGroupCreate()
    {
        for (int i = 0; i < 2; i++)
        {
            var tileSlot = mTileDatas[i];
            var unitGroup = ObjectPoolManager.Instance
                .CreatePoolingObject(AddressableTable.Default_UnitGroup, tileSlot.transform.position)
                .GetComponent<UnitGroup>();
            tileSlot.SetOccupantUnit(unitGroup);
        }
    }

    public TileSlot GetClickTileSlotDetailOrNull()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero); 
        TileSlot tileSlot = hit.collider != null ? hit.transform.GetComponent<TileSlot>() : null;
        return tileSlot;
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
