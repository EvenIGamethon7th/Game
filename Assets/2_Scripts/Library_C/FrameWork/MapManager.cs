using System;
using System.Collections.Generic;
using _2_Scripts.Game.Map.Tile;
using _2_Scripts.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private Tilemap mMap;
    private Dictionary<Vector3Int, TileSlot> mTileDatas = new();
    private const string TILE_SLOT_NAME = AddressableTable.Default_TileSlot;
    private void Start()
    {
        MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.ResourceLoad).Subscribe(
        _ =>
        {
            CreateInitialTileSlots();
        });
    }

    private void CreateInitialTileSlots()
    {
        BoundsInt bounds = mMap.cellBounds;
        Vector3 tileSize = mMap.cellSize;
        Vector3 indicatorOffset = new Vector3(tileSize.x, tileSize.y, 0); // 타일 크기의 반만큼 이동하여 중앙에 배치

        for (int x = bounds.xMin; x < bounds.xMax; x += 2)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y += 2)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = mMap.CellToWorld(cellPosition) + indicatorOffset;
                // 타일 배치
                var tile = ObjectPoolManager.Instance.CreatePoolingObject(TILE_SLOT_NAME, worldPosition).GetComponent<TileSlot>();
                mTileDatas.Add(cellPosition,tile);
            }
        }
    }
}
