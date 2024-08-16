using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.Map.Tile;
using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class MapManager : Singleton<MapManager>
{
    [SerializeField] private Tilemap mMap;
    [SerializeField] private Tilemap mMonsterPathMap;
    [SerializeField] private MapTile mMapTile;
    [SerializeField] private GameObject mTestCharacter;

    private List<TileSlot> mTileDatas = new();
    private const string TILE_SLOT_NAME = AddressableTable.Default_TileSlot;

    private void Start()
    {
        SceneLoadManager.Instance.SceneClear += Clear;

        void Clear()
        {
            SceneLoadManager.Instance.SceneClear -= Clear;
            for (int i = 0; i < mTileDatas.Count; ++i)
            {
                mTileDatas[i].Clear();
            }
        }

        if (GameManager.Instance.IsTest)
        {
            MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.DefaultResourceLoad).Subscribe(
                _ =>
                {
                    mTestCharacter?.SetActive(true);
                    CreateInitialTileSlots();
                    CreatePool();
                }).AddTo(this);

            MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.MapDataResourceLoad).Subscribe(
                _ =>
                {
                    var go = Instantiate(ResourceManager.Instance.Load<GameObject>("1"));
                    mMapTile = go.GetComponent<MapTile>();
                    mMap = mMapTile.UnitTile;
                    mMonsterPathMap = mMapTile.MonsterTile;
                }).AddTo(this);
        }

        else if (!BackEndManager.Instance.IsUserTutorial)
        {
            mTestCharacter?.SetActive(true);
            CreateInitialTileSlots();
            CreatePool();
        }

        else
        {
            var go = Instantiate(ResourceManager.Instance.Load<GameObject>(GameManager.Instance.CurrentStageData.ChapterNumber.ToString()));
            mMapTile = go.GetComponent<MapTile>();
            mMap = mMapTile.UnitTile;
            mMonsterPathMap = mMapTile.MonsterTile;

            Instantiate(ResourceManager.Instance.Load<GameObject>($"{GameManager.Instance.CurrentMainCharacter.name}{BackEndManager.Instance.UserMainCharacterData[GameManager.Instance.CurrentMainCharacter.name].rank}"));
            CreateInitialTileSlots();
            CreatePool();
        }
    }

    public Vector2Int GetCellFromWorldPos(Vector2 pos)
    {
        int x = Mathf.FloorToInt(pos.x / mMonsterPathMap.cellSize.x);
        int y = Mathf.FloorToInt(pos.y / mMonsterPathMap.cellSize.y);
        return new Vector2Int(x, y);
    }
    
    public Vector3 GetWorldPosFromCell(Vector3Int pos)
    {
        Vector3 tileSize = mMonsterPathMap.cellSize;
        Vector3 slotOffset = new Vector3(tileSize.x, tileSize.y, 0);
        return mMonsterPathMap.CellToWorld(pos) + slotOffset;
    }

    public void CheckTileSlotOnUnit(Vector3Int pos,Action<Collider2D[]> action)
    {
        Vector3 cellWorldPosition = GetWorldPosFromCell(pos);
        var _cellSize = mMonsterPathMap.cellSize;
        Vector3 cellCenterWorldPosition = cellWorldPosition + _cellSize / 2;
        Vector2 cellSize = _cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(cellCenterWorldPosition, cellSize, 0f);
        action.Invoke(colliders);
    }

    public UnitGroup CheckOccupantSameUnit(UnitGroup unitGroup)
    {
        var tileSlot = mTileDatas
            .Where(x => x.OccupantUnit != null)
            .Where(x => x.OccupantUnit.GetCharacterData() != null && x.OccupantUnit.GetCharacterData().nameKey == unitGroup.GetCharacterData().nameKey && unitGroup != x.OccupantUnit && x.OccupantUnit.CanAddUnit()).FirstOrDefault();
        return tileSlot?.OccupantUnit;
    }

    public TileSlot GetTileSlotAboutGroup(UnitGroup group)
    {
        return mTileDatas.Where(x => x.OccupantUnit == group).FirstOrDefault();
    }

    public void ClearTile(TileSlot tileSlot)
    {
        MessageBroker.Default.Publish(new GameMessage<UnitGroup>(EGameMessage.SelectCharacter, null));
        if (tileSlot != null)
        {
            tileSlot.Clear();
        }
    }

    public bool CanCreateUnit()
    {
        var tile = mTileDatas.Where(x => x.OccupantUnit == null).FirstOrDefault();
        return tile == null ? false : true;
    }

    public bool CreateUnit(CharacterData characterData, bool isAlumni = false, Action<Vector3> spawnAction = null, TileSlot assignSlot = null)
    {
        //먼저 같은 유닛 그룹과 그 그룹에 공간이 있는지 확인
        var tileSlot = mTileDatas.Where(x => x.OccupantUnit != null && x.OccupantUnit.GetCharacterData().nameKey == characterData.nameKey && x.CanAddUnit()).FirstOrDefault();

        //없다면 빈 타일 슬롯 확인
        if (tileSlot == null)
        {
            if (assignSlot != null && assignSlot.OccupantUnit == null)
            {
                tileSlot = assignSlot;
            }

            else
            {
                tileSlot = mTileDatas.Where(x => x.OccupantUnit == null).FirstOrDefault();
            }

            //그것도 없으면 쩔수없지...
            if (tileSlot == null)
            {
                if (!isAlumni)
                    UI_Toast_Manager.Instance.Activate_WithContent_Func("모든 타일에 유닛이 배치되어 있습니다!");
                else
                    UI_Toast_Manager.Instance.Activate_WithContent_Func("아카데미 수업 종료\n필드를 비워주세요!");
                return false;
            }

            else
            {
                var unitGroup = ObjectPoolManager.Instance
                .CreatePoolingObject(AddressableTable.Default_UnitGroup, tileSlot.transform.position)
                .GetComponent<UnitGroup>();
                var unit = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Unit, tileSlot.transform.position).GetComponent<CUnit>();
                unit.Init(characterData);
                tileSlot.Init(unitGroup, unit);
            }
        }

        //해당 유닛 그룹이 있다면 유닛 생성해서 넣기
        else
        {
            var unit = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Unit, tileSlot.transform.position).GetComponent<CUnit>();
            unit.Init(characterData);
            tileSlot.OccupantUnit.AddUnit(unit);
        }
        spawnAction?.Invoke(tileSlot.transform.position);
        if (characterData.rank == 3)
        {
            BackEndManager.Instance.AddSpawnMission(characterData);
        }

        return true;
    }

    public TileSlot GetClickTileSlotDetailOrNull()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int outlineLayer = 1 << LayerMask.NameToLayer("Outline");
        int buffLayer = 1 << LayerMask.NameToLayer("Buff");
        int layerMask = ~(outlineLayer | buffLayer); 

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 100, layerMask, 0);
        TileSlot tileSlot = hit.collider != null ? hit.transform.GetComponent<TileSlot>() : null;
        return tileSlot;
    }

    public TileSlot GetNearClickTileSlotDetailOrNull()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mTileDatas.OrderBy(x => Vector3.Distance(x.transform.position, pos)).FirstOrDefault();
    }

    public TileSlot GetClickTileSlotDetailOrNull(Vector3 pos)
    {
        int outlineLayer = 1 << LayerMask.NameToLayer("Outline");
        int buffLayer = 1 << LayerMask.NameToLayer("Buff");
        int monsterTile = 1 << LayerMask.NameToLayer("MonsterTile");
        int layerMask = ~(outlineLayer | buffLayer | monsterTile);

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 100, layerMask, 0);
        TileSlot tileSlot = hit.collider != null ? hit.transform.GetComponent<TileSlot>() : null;
        return tileSlot;
    }

    protected override void ChangeSceneInit(Scene prev, Scene next)
    {
        for (int i = 0; i < mTileDatas.Count; ++i)
        {
            mTileDatas[i].Clear();
        }

        mTileDatas.Clear();

    }

    private void CreatePool()
    {
        //ObjectPoolManager.Instance.RegisterPoolingObject(AddressableTable.Default_Unit, 30);
        ObjectPoolManager.Instance.RegisterPoolingObject(AddressableTable.Default_DamageCanvas, 100);
    }

    private void ChangeTile(Tilemap map,Tile tile)
    {
        BoundsInt bounds = map.cellBounds;
        Vector3Int tilePosition = new Vector3Int(0, 0, 0);
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                tilePosition.x = x;
                tilePosition.y = y;
                if (map.HasTile(tilePosition))
                {
                    map.SetTile(tilePosition, tile);
                }
            }
        }
    }
    
    private void CreateInitialTileSlots()
    {
        BoundsInt bounds = mMap.cellBounds;
        Vector3 tileSize = mMap.cellSize;
        Vector3 slotOffset = new Vector3(tileSize.x, tileSize.y, 0);
        bool isMainCharacterInit = false;
        float tempZ = -0.04f;
        for (int x = bounds.xMin; x < bounds.xMax; x += 2)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y += 2)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (!mMap.HasTile(cellPosition)) 
                {
                    if (!isMainCharacterInit)
                    {
                        isMainCharacterInit = true;
                        Vector3 temp = mMap.CellToWorld(cellPosition) + slotOffset * 2;
                    }
                    continue;
                }
                
                Vector3 worldPosition = mMap.CellToWorld(cellPosition) + slotOffset;
                // 타일 배치
                var tile = ObjectPoolManager.Instance.CreatePoolingObject(TILE_SLOT_NAME, worldPosition).GetComponent<TileSlot>();
                tile.TileZ = tempZ;
                tempZ += 0.001f;
                mTileDatas.Add(tile);
            }
        }
    }
}
