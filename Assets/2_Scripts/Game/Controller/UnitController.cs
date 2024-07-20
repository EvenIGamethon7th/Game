using System;
using System.Collections.Generic;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.Map.Tile;
using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using _2_Scripts.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _2_Scripts.Game.Controller
{
    public class UnitController : MonoBehaviour
    {
        private TileSlot mSelectTileSlot;
        private UnitGroup mSelectUnitGroup;
        private Indicator mIndicator;
        private GameObject mSelectCircle;
        private IDisposable mTempSubscribe;

        private bool mHasLongTouch = false;
        
        private GameMessage<UnitGroup> mSelectUnitMessage;

        
        public GraphicRaycaster uiRaycaster;
        public EventSystem eventSystem;
        
        
        private void Start()
        {
            WaitResourceLoad();
            var mouseDownStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0));
            var mouseUpStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonUp(0));
            
            //롱 터치 시 목적지 위치를 프레임마다 받아옴
            this.UpdateAsObservable()
                .Where(_ => mHasLongTouch)
                .Subscribe(_ =>
                {
                    var dstSlot = MapManager.Instance.GetClickTileSlotDetailOrNull();
                    if (dstSlot != null && dstSlot.IsNormalUnit)
                    {
                        mIndicator.SetIndicator(mSelectTileSlot.transform.position, dstSlot.transform.position);
                    }
                    
                });

            mouseDownStream.Subscribe(_ =>
            {
                if (IsPointerOverUIPopUp()) return;

                mSelectTileSlot = MapManager.Instance.GetClickTileSlotDetailOrNull();
                mSelectUnitGroup = mSelectTileSlot?.OccupantUnit;
                mSelectUnitMessage = new GameMessage<UnitGroup>(EGameMessage.SelectCharacter, null);
                MessageBroker.Default.Publish(mSelectUnitMessage);
                if (mSelectTileSlot != null && mSelectTileSlot.IsNormalUnit)
                {
                    mTempSubscribe = mouseUpStream
                        .Buffer(TimeSpan.FromMilliseconds(150))
                        .Take(1)
                        .Where(x => x.Count == 0)
                        .Subscribe(_ =>
                        {
                            mHasLongTouch = true;
                            mTempSubscribe.Dispose();
                        });
                }
            });

            mouseDownStream
                .SelectMany(_ => mouseUpStream.First())
                .Subscribe(_ =>
                {
                    if (mHasLongTouch)
                    {
                        //위치 이동 및 자리 변경
                        TileSlot dstSlot = MapManager.Instance.GetClickTileSlotDetailOrNull();
                        if (dstSlot != null && dstSlot != mSelectTileSlot && dstSlot.IsNormalUnit)
                        {
                            UnitGroup dstUnit = dstSlot.OccupantUnit;
                            if (dstUnit != null)
                            {
                                mSelectTileSlot.SetOccupantUnit(dstUnit);
                            }
                            else
                            {
                                mSelectTileSlot.SetOccupantUnit(null);
                            }
                            dstSlot.SetOccupantUnit(mSelectUnitGroup);
                            mSelectTileSlot = dstSlot;
                        }
                        mHasLongTouch = false;
                        mIndicator?.SetActive(false);
                    }

                    else
                    {
                        mSelectTileSlot = MapManager.Instance.GetClickTileSlotDetailOrNull();
                        var selectUnitGroup = mSelectTileSlot?.GetComponent<TileSlot>().OccupantUnit;

                        if (mSelectUnitGroup != null)
                            mSelectUnitGroup.IsSelect = false;

                        if (selectUnitGroup != null)
                        {
                            //TODO: UI에 정보 올리기 SelectCharacter 메모리 풀링 사용해야할듯 메세지도
                            mSelectUnitMessage = new GameMessage<UnitGroup>(EGameMessage.SelectCharacter, selectUnitGroup);
                            MessageBroker.Default.Publish(mSelectUnitMessage);
                            mSelectCircle.transform.parent = selectUnitGroup.transform;
                            mSelectCircle.transform.position = selectUnitGroup.transform.position;
                            mSelectCircle.SetActive(true);
                            selectUnitGroup.IsSelect = true;
                        }

                        else
                        {
                            if (IsPointerOverUIPopUp())
                            {
                                return;
                            }
                            mSelectUnitMessage = new GameMessage<UnitGroup>(EGameMessage.SelectCharacter, null);
                            MessageBroker.Default.Publish(mSelectUnitMessage);
                            mSelectCircle.transform.parent = null;
                            mSelectCircle.SetActive(false);
                        }

                        mSelectUnitGroup = selectUnitGroup;
                    }
                });
            
        }

        private void WaitResourceLoad()
        {
            mTempSubscribe = MessageBroker.Default.Receive<TaskMessage>()
                .Where(message => message.Task == ETaskList.DefaultResourceLoad)
                .Subscribe(_ =>
                {
                    mIndicator = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Indicator, Vector2.zero).GetComponent<Indicator>();
                    mSelectCircle = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_SelectCircle, Vector2.zero);
                    mSelectCircle.SetActive(false);
                    mTempSubscribe.Dispose();
                });
        }
        
        private bool IsPointerOverUIPopUp()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(eventSystem);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(eventDataCurrentPosition, results);
            if(results.Count == 0) 
                return false;
            
            return results[0].gameObject.CompareTag("PopUp");
        }
        
    }
}