using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.TileSystem
{
    public abstract class Tile : MonoBehaviour
    {
        [ReadOnly, ShowInInspector] protected int xID;
        [ReadOnly, ShowInInspector] protected int yID;
        [ReadOnly, ShowInInspector] private bool isActivate;
        protected TileSystem tileSystemClass;
        public abstract string GetTileTypeStr { get; }
        public bool IsActivate => this.isActivate;

        public virtual int GetX => this.xID;
        public virtual int GetY => this.yID;
        public TilePosData GetTilePosData => new TilePosData(this.xID, this.yID);

        public virtual void Init_Func()
        {
            this.Deactivate_Func(true);
        }

        public void Activate_Func(TilePosData _tilePosData, TileSystem _tileSystemClass, bool _isSetTile = true, bool _isSetMove = true)
        {
            this.Activate_Func(_tilePosData.X, _tilePosData.Y, _tileSystemClass, _isSetTile, _isSetMove);
        }
        public void Activate_Func(Vector2Int _tilePos, TileSystem _tileSystemClass, bool _isSetTile = true, bool _isSetMove = true)
        {
            int _posX = _tilePos.x;
            int _posY = _tilePos.y;
            this.Activate_Func(_posX, _posY, _tileSystemClass, _isSetTile, _isSetMove);
        }
        public virtual void Activate_Func(int _x, int _y, TileSystem _tileSystemClass, bool _isSetTile = true, bool _isSetMove = true)
        {
            this.SetTilePosData_Func(_x, _y);

            this.tileSystemClass = _tileSystemClass;

            if (_isSetTile == true) _tileSystemClass.AddTile_Func(this);
            if (_isSetMove == true) this.MoveDone_Func(_x, _y);

            this.ActivateDone_Func();
        }
        protected virtual void ActivateDone_Func()
        {
            this.isActivate = true;
        }

        protected void SetTilePosData_Func(TilePosData _posData)
        {
            this.SetTilePosData_Func(_posData.X, _posData.Y);
        }
        protected virtual void SetTilePosData_Func(int _posX, int _posY)
        {
            this.xID = _posX;
            this.yID = _posY;
        }

        // 이동 가능 여부
        public bool CheckMovable_Func(Tile _moveTileClass, DirectionType _dirType = DirectionType.None)
        {
            return this.CheckMovableOverride_Func(_moveTileClass, _dirType);
        }
        protected virtual bool CheckMovableOverride_Func(Tile _moveTileClass, DirectionType _dirType = DirectionType.None)
        {
            return false;
        }

        // 이동을 시도할 경우 호출됨
        public virtual void OnMove_Func(DirectionType _dirType, int _range = 1, Action<Tile> _collideTileDel = null, bool _isBundleUp = true, params string[] _checkTileTypeArr)
        {
            bool _isMovable = true;
            TilePosData _arrivePosData = default;
            for (int i = 1; i <= _range; i++)
            {
                _arrivePosData = TilePosData.GetPos_Func(this, _dirType, i);

                if (this.tileSystemClass.Move_Func(this, _arrivePosData.X, _arrivePosData.Y, _dirType, true, _isBundleUp: _isBundleUp, _checkTileTypeArr: _checkTileTypeArr) == false)
                {
                    _isMovable = false;

                    break;
                }
            }

            this.OnMove_Func(_arrivePosData, _isMovable, true, true, false, _dirType, _collideTileDel, _checkTileTypeArr);
        }
        public virtual void OnMove_Func(TilePosData _arrivePosData, DirectionType _dirType = DirectionType.None, params string[] _checkTileTypeArr)
        {
            bool _isMovable = this.tileSystemClass.Move_Func(this, _arrivePosData.X, _arrivePosData.Y, _dirType, true, _checkTileTypeArr: _checkTileTypeArr);

            this.OnMove_Func(_arrivePosData, _isMovable, true, true, false, _dirType, _checkTileTypeArr: _checkTileTypeArr);
        }

        // 이동한 경우 호출됨
        protected void OnMove_Func(TilePosData _arrivePosData, bool _isMovable, bool _isMove, bool _isSetTilePosData
            , bool _isJustCheck = false, DirectionType _dirType = DirectionType.None, Action<Tile> _collideTileDel = null, params string[] _checkTileTypeArr)
        {
            // 확인만 하는게 아닌가?
            if (_isJustCheck == false)
            {
                // 이동 가능한가?
                if (this.tileSystemClass.Move_Func(
                    this, _arrivePosData.X, _arrivePosData.Y, _dirType, _isJustCheck, _isMovable, _collideTileDel: _collideTileDel, _checkTileTypeArr: _checkTileTypeArr) == true)
                {
                    if (_isSetTilePosData == true)
                        this.SetTilePosData_Func(_arrivePosData);

                    if (_isMove == true)
                        this.MoveDone_Func(_arrivePosData.X, _arrivePosData.Y);
                }
                else
                {
                    this.MoveFail_Func();
                }
            }
        }
        protected virtual void MoveDone_Func(int _xID, int _yID)
        {
            this.transform.position = this.tileSystemClass.GetWorldPos_Func(_xID, _yID);
        }

        // 이동에 실패한 경우 호출됨
        protected abstract void MoveFail_Func();

        // 밀려난 경우 호출됨
        public abstract void Pushed_Func(Tile _pushTileClass, DirectionType _pushDir = DirectionType.None);

        public Vector2 GetWorldPos_Func()
        {
            return this.tileSystemClass.GetWorldPos_Func(this);
        }

        public bool CheckTileRange_Func(DirectionType _dirType, out TilePosData _checkPosData, int _times = 1)
        {
            int _checkTilePosX = this.GetX;
            int _checkTilePosY = this.GetY;

            switch (_dirType)
            {
                case DirectionType.Left:
                    _checkTilePosX -= _times;
                    break;
                case DirectionType.Down:
                    _checkTilePosY -= _times;
                    break;
                case DirectionType.Up:
                    _checkTilePosY += _times;
                    break;
                case DirectionType.Right:
                    _checkTilePosX += _times;
                    break;
            }

            _checkPosData = new TilePosData(_checkTilePosX, _checkTilePosY);

            return this.tileSystemClass.CheckInOfRange_Func(_checkTilePosX, _checkTilePosY);
        }
        public virtual void DeactivateWithoutRemoveTileSystem_Func()
        {
            this.Deactivate_Func(_isRemoveInTileSystem: false);
        }

        public virtual void Deactivate_Func(bool _isInit = false, bool _isRemoveInTileSystem = true)
        {
            if (_isInit == false)
            {
                if (_isRemoveInTileSystem == true)
                    this.tileSystemClass?.RemoveTile_Func(this, false);
            }

            this.isActivate = false;
        }
    }
}