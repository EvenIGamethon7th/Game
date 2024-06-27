using System;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.Map.Tile;
using _2_Scripts.Game.Unit;
using Cargold;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace _2_Scripts.Game.Controller
{
    public class UnitController : MonoBehaviour
    {
        private TileSlot mSelectTileSlot;
        private UnitGroup mSelectUnitGroup;
        private Indicator mIndicator;
        private GameObject mSelectCircle;

        private bool mHasLongTouch = false;

        private void Start()
        {

            var mouseDownStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0));
            var mouseUpStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonUp(0));

            //롱 터치 시 목적지 위치를 프레임마다 받아옴
            this.UpdateAsObservable()
                .Where(_ => mHasLongTouch)
                .Subscribe(_ =>
                {
                    var dstSlot = MapManager.Instance.GetClickTileSlotDetailOrNull();
                    if (dstSlot != null)
                    {
                        mIndicator.SetIndicator(mSelectTileSlot.transform.position, dstSlot.transform.position);
                    }
                    
                    Debug.Log($"Long touch : srcTile: {mSelectTileSlot?.transform.position}, dstTile: {MapManager.Instance.GetClickTileSlotDetailOrNull()?.transform.position}");
                });

            //유닛을 선택했을 때 같은 타일을 선택했는지
            mouseDownStream
                .Subscribe(_ =>
                {
                    if (mSelectUnitGroup != null && mSelectTileSlot == MapManager.Instance.GetClickTileSlotDetailOrNull())
                    {
                        Debug.Log("Click Same Unit and Tile");
                        //롱 터치 판별
                        mouseUpStream
                            .Buffer(TimeSpan.FromSeconds(1))
                            .Take(1)
                            .Where(x => x.Count == 0)
                            .Subscribe(_ =>
                            {
                                mHasLongTouch = true;
                            });
                    }

                    else
                    {
                        Debug.Log("Click Different Unit or Tile");
                    }
                    
                });

            mouseDownStream
                .SelectMany(_ => mouseUpStream.First())
                .Subscribe(_ =>
                {
                    if (mIndicator == null)
                    {
                        mIndicator = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Indicator, Vector2.zero).GetComponent<Indicator>();
                    }

                    if (mHasLongTouch)
                    {
                        //위치 이동 및 자리 변경
                        TileSlot dstSlot = MapManager.Instance.GetClickTileSlotDetailOrNull();
                        if (dstSlot != null)
                        {
                            UnitGroup dstUnit = dstSlot.OccupantUnit;
                            if (dstUnit != null)
                            {
                                mSelectTileSlot.SetOccupantUnit(dstUnit);
                            }
                            dstSlot.SetOccupantUnit(mSelectUnitGroup);
                        }
                        mHasLongTouch = false;
                        mIndicator?.SetActive(false);
                    }

                    else
                    {
                        mSelectTileSlot = MapManager.Instance.GetClickTileSlotDetailOrNull();
                        mSelectUnitGroup = mSelectTileSlot?.GetComponent<TileSlot>().OccupantUnit;
                        if (mSelectCircle == null)
                            mSelectCircle = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Select_Circle, Vector2.zero);
                        if (mSelectUnitGroup != null)
                        {
                            //TODO: UI에 정보 올리기
                            mSelectCircle.transform.parent = mSelectUnitGroup.transform;
                            mSelectCircle.transform.position = mSelectUnitGroup.transform.position;
                            mSelectCircle.SetActive(true);
                        }

                        else
                        {
                            mSelectCircle.transform.parent = null;
                            mSelectCircle.SetActive(false);
                        }
                    }
                    Debug.Log($"select Unit : {mSelectUnitGroup?.name}");
                });
            
        }
    }
}