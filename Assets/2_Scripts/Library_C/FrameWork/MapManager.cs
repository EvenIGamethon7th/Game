using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Map.Tile;
using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using _2_Scripts.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class MapManager : Singleton<MapManager>
{
    [SerializeField] private Tilemap mMap;
    [SerializeField] private UI_AcademyPannel mAcademy;

    private List<TileSlot> mTileDatas = new();
    private const string TILE_SLOT_NAME = AddressableTable.Default_TileSlot;
    private void Start()
    {
        //추후 수정해야함
        MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.DefaultResourceLoad).Subscribe(
        _ =>
        {
            CreateInitialTileSlots();
            CreatePool();
            mAcademy.Init();
        });
    }

    public UnitGroup CheckOccupantSameUnit(UnitGroup unitGroup)
    {
        var tileSlot = mTileDatas
            .Where(x => x.OccupantUnit != null)
            .Where(x => x.CurrentUnitData.nameKey == unitGroup.GetCharacterData().nameKey && unitGroup != x.OccupantUnit && x.OccupantUnit.CanAddUnit()).FirstOrDefault();
        return tileSlot?.OccupantUnit;
    }

    public bool GoAcademy(UnitGroup group, CUnit unit)
    {
        bool canEnterAcademy = mAcademy.CanLesson(unit);
        if (canEnterAcademy)
            mAcademy.AcademyLesson(unit);

        return canEnterAcademy;
    }

    public void ClearTile(UnitGroup group)
    {
        var tileSlot = mTileDatas.Where(x => x.OccupantUnit == group).FirstOrDefault();

        if (tileSlot != null)
        {
            tileSlot.Clear();
        }
    }

    public bool CreateUnit(CharacterData characterData, bool isAlumni = false)
    {
        //먼저 같은 유닛 그룹과 그 그룹에 공간이 있는지 확인
        var tileSlot = mTileDatas.Where(x => x.CurrentUnitData?.nameKey == characterData.nameKey && x.CanAddUnit()).FirstOrDefault();

        //없다면 빈 타일 슬롯 확인
        if (tileSlot == null)
        {
            tileSlot = mTileDatas.Where(x => x.OccupantUnit == null).FirstOrDefault();
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

        return true;
    }

    public TileSlot GetClickTileSlotDetailOrNull()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero); 
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
        ObjectPoolManager.Instance.RegisterPoolingObject(AddressableTable.Default_Unit, 100);
        ObjectPoolManager.Instance.RegisterPoolingObject(AddressableTable.Default_DamageCanvas, 100);
    }

    private void CreateInitialTileSlots()
    {
        BoundsInt bounds = mMap.cellBounds;
        Vector3 tileSize = mMap.cellSize;
        Vector3 slotOffset = new Vector3(tileSize.x, tileSize.y, 0);
        bool isMainCharacterInit = false;
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
                mTileDatas.Add(tile);
            }
        }
    }
}
