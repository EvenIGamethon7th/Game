using System;
using _2_Scripts.Game.Map.Tile;
using _2_Scripts.Game.Unit;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace _2_Scripts.Game.Controller
{
    public class UnitController : MonoBehaviour
    {
        private UnitGroup mSelectUnitGroup;
        private void Start()
        {
            
            var mouseDownStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0));
            var mouseUpStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonUp(0));

            mouseDownStream
                .SelectMany(_ => mouseUpStream.First())
                .Subscribe(_ =>
                {
                    mSelectUnitGroup = MapManager.Instance.GetClickTileSlotDetailOrNull()?.GetComponent<TileSlot>().OccupantUnit;
                    Debug.Log($"select Unit : {mSelectUnitGroup?.name}");
                });
            
        }
    }
}