using Cargold.Observer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용법

// TileSystem에 관리 당할 일반 타일 클래스는 Tile_Class를 상속 받아야 한다.
// TileSystem와 일반 타일 클래스는 모두 초기화 함수(Init_Func)을 호출해야 한다.
// 타일 클래스는 TileSystem에게 관리를 받기 위해선 SetTile 함수를 호출해야 한다.

namespace Cargold.TileSystem
{
    public class TileSystem : SerializedMonoBehaviour, Cargold.FrameWork.GameSystem_Manager.IInitializer
    {
        private static List<List<Tile>> TileListList;

        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), ShowInInspector, ReadOnly, LabelText("타일 그룹 클래스")]
        private Dictionary<int, Dictionary<int, TileGroup>> tileGroupDicDic;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), ShowInInspector, ReadOnly, LabelText("타입 별 타일")]
        private Dictionary<string, List<Tile>> tileKeyByTileClassListDic;
        private Observer_Action<TilePosData, Tile> addTileClassObs;
        private Observer_Action<TilePosData, Tile> removeTileClassObs;
        private int maxTilePosX;
        private int maxTilePosY;
        [SerializeField] private bool isCheckTouch;
        [ShowInInspector, ReadOnly] private CoroutineData touchCorData;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), ShowInInspector, LabelText("최대 가로 X")] public int GetMaxTilePosX => this.maxTilePosX;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), ShowInInspector, LabelText("최대 세로 Y")] public int GetMaxTilePosY => this.maxTilePosY;
        public TilePosData GetMaxTilePos => new TilePosData(this.maxTilePosX, this.maxTilePosY);

        // 타일맵 시스템의 시작 WorldSpace위치 보정값
        public virtual Vector2 TilePos_InitData => Vector2.zero;

        // 각 타일간의 WorldSpace 간격
        public virtual Vector2 TileSpace => Vector2.one;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                this.tileGroupDicDic = new Dictionary<int, Dictionary<int, TileGroup>>();
                this.tileKeyByTileClassListDic = new Dictionary<string, List<Tile>>();
                this.addTileClassObs = new Observer_Action<TilePosData, Tile>();
                this.removeTileClassObs = new Observer_Action<TilePosData, Tile>();

                TileListList = new List<List<Tile>>();

                this.Deactivate_Func(true);
            }
        }

        public void Activate_Func(TilePosData _tilePosData, bool _isInstanceAll = true)
        {
            this.Activate_Func(_tilePosData.X, _tilePosData.Y);
        }
        public virtual void Activate_Func(int _x, int _y, bool _isInstanceAll = true)
        {
            if(_isInstanceAll == true)
            {
                this.tileGroupDicDic.Clear();

                foreach (var item in this.tileKeyByTileClassListDic)
                    item.Value.Clear();
            }

            this.maxTilePosX = _x;
            this.maxTilePosY = _y;

            if(this.isCheckTouch == true)
                this.OnCheckTouch_Func();
        }

        public void OnCheckTouch_Func()
        {
            this.touchCorData.StartCoroutine_Func(this.OnCheckTouch_Cor(), CoroutineStartType.StartWhenStop);
        }
        private IEnumerator OnCheckTouch_Cor()
        {
            while (true)
            {
                if(Input.GetMouseButtonDown(0) == true)
                {
                    Vector2 _touchWorldPos = this.GetTouchWorldPos_Func(Input.mousePosition);
                    TilePosData _tilePosData = this.GetTilePosData_Func(_touchWorldPos);
                    bool _isInOfRange = this.CheckInOfRange_Func(_tilePosData);
                    this.OnTouch_Func(_tilePosData, _isInOfRange);
                }

                yield return null;
            }
        }
        protected virtual Vector2 GetTouchWorldPos_Func(Vector2 _touchScreenPos)
        {
#if UNITY_EDITOR
            if (Cargold.CameraSystem_Manager.Instance == null)
                Debug_C.Error_Func("?");
#endif

            return Cargold.CameraSystem_Manager.Instance.GetWorldPos_Func(_touchScreenPos);
        }
        protected virtual void OnTouch_Func(TilePosData _touchedTilePosData, bool _isInOfRange) { }

        // 특정 타일을 매니저에게 알려서 관리 받도록 세팅
        public void AddTile_Func(Tile _tileClass)
        {
            int _x = _tileClass.GetX;
            int _y = _tileClass.GetY;

            if (this.CheckInOfRange_Func(_x, _y) == true)
            {
                string _tileTypeStr = _tileClass.GetTileTypeStr;
                List<Tile> _tileClassList = this.tileKeyByTileClassListDic.GetValue_Func(_tileTypeStr, () => new List<Tile>());
#if UNITY_EDITOR
                if (_tileClassList.Contains(_tileClass) == true)
                    Debug_C.Error_Func("중복 : " + _tileClass); 
#endif

                _tileClassList.Add(_tileClass);

                TileGroup _tileGroup = this.GetTileGroupClass_Func(_x, _y);
                _tileGroup.AddTile_Func(_tileClass);

                this.addTileClassObs.Notify_Func(new TilePosData(_x, _y), _tileClass);
            }
            else
            {
                Debug_C.Warning_Func("필드의 영역을 벗어났습니다. Max : " + this.GetMaxTilePos + " / Tile : " + _tileClass.GetTilePosData);
            }
        }
        public void RemoveTile_Func(Tile _tileClass, bool _isDeactivateToTile)
        {
            int _x = _tileClass.GetX;
            int _y = _tileClass.GetY;

            if (this.CheckInOfRange_Func(_x, _y) == true)
            {
                string _tileTypeStr = _tileClass.GetTileTypeStr;
                if (this.tileKeyByTileClassListDic.TryGetValue(_tileTypeStr, out List<Tile> _tileClassList) == false)
                    Debug_C.Error_Func("_tileTypeStr : " + _tileTypeStr);

                _tileClassList.Remove(_tileClass);

                TileGroup _tileGroup = this.GetTileGroupClass_Func(_x, _y);
                _tileGroup.RemoveTile_Func(_tileClass, _isDeactivateToTile);

                this.removeTileClassObs.Notify_Func(new TilePosData(_x, _y), _tileClass);
            }
            else
            {
                Debug_C.Warning_Func("필드의 영역을 벗어났습니다.");
            }
        }
        public bool Move_Func(Tile _moveTileClass, TilePosData _tilePosData
            , DirectionType _moveDir = DirectionType.None, bool _isJustCheck = false, Action<Tile> _collideTileDel = null, params string[] _checkTileTypeArr)
        {
            return this.Move_Func(_moveTileClass, _tilePosData.X, _tilePosData.Y, _moveDir, _isJustCheck, _collideTileDel: _collideTileDel, _checkTileTypeArr: _checkTileTypeArr);
        }
        
        public bool Move_Func(Tile _moveTileClass, int _arrivePosX, int _arrivePosY
            , DirectionType _moveDir = DirectionType.None, bool _isJustCheck = false, bool _isSetTileMove = true, Action<Tile> _collideTileDel = null, bool _isBundleUp = true
            , params string[] _checkTileTypeArr)
        {
            // 이동 좌표가 타일 범위를 초과했는가?
            bool _isInOfRange = this.CheckInOfRange_Func(_arrivePosX, _arrivePosY);

            if (_isInOfRange == true)
            {
                // 이동 타일의 정보
                int _moveX = _moveTileClass.GetX;
                int _moveY = _moveTileClass.GetY;

                // 도착 타일의 정보
                TileGroup _arriveTileGroupClass = this.GetTileGroupClass_Func(_arrivePosX, _arrivePosY);

                foreach (List<Tile> _tileList in TileListList)
                    _tileList.Clear();

                bool _isMovable = false;
                if (_checkTileTypeArr.IsHave_Func() == true)
                {
                    for (int i = 0; i < _checkTileTypeArr.Length; i++)
                    {
                        _arriveTileGroupClass.TryGetTile_Func(_checkTileTypeArr[i], out List<Tile> _findTileClassList);

                        // 도착 지점에 특정 타일이 있는가?
                        if (_findTileClassList.IsHave_Func() == true)
                        {
                            List<Tile> _catchTileList = TileListList.GetItem_Func(i, (_id) => new List<Tile>());

                            if (_isBundleUp == false)
                            {
                                Tile _findTileClass = _findTileClassList[0];
                                _catchTileList.Add(_findTileClass);

                                // 도착 지점의 특정 타일의 이동 가능 여부 확인
                                _isMovable = _findTileClass.CheckMovable_Func(_moveTileClass, _moveDir);

                                if (_isMovable == false)
                                    break;
                            }
                            else
                            {
                                foreach (Tile _findTileClass in _findTileClassList)
                                {
                                    _catchTileList.Add(_findTileClass);

                                    // 도착 지점의 특정 타일의 이동 가능 여부 확인
                                    if(_isMovable == true)
                                        _isMovable = _findTileClass.CheckMovable_Func(_moveTileClass, _moveDir);
                                }

                                if (_isMovable == false)
                                    break;
                            }
                        }
                        else
                        {
                            _isMovable = true;
                        }
                    }
                }
                else
                {
                    _isMovable = true;
                }

                // 단순히 확인만 할 것인가?
                if (_isJustCheck == false)
                {
                    foreach (List<Tile> _catchTileList in TileListList)
                    {
                        // 도착 지점에 특정 타일이 있는가?
                        if(_catchTileList.IsHave_Func() == true)
                        {
                            foreach (Tile _collideTile in _catchTileList)
                            {
                                // 도착 지점의 특정 타일에게 '밀려남'을 알림
                                _collideTile.Pushed_Func(_moveTileClass, _moveDir);

                                _collideTileDel?.Invoke(_collideTile);
                            }
                        }
                    }

                    if(_isSetTileMove == true)
                    {
                        // 도착 지점으로 이동 가능한가?
                        if (_isMovable == true)
                        {
                            // 타일 그룹에서 이동 타일을 제외시킴
                            TileGroup _moveTileGroupClass = this.GetTileGroupClass_Func(_moveX, _moveY);
                            _moveTileGroupClass.RemoveTile_Func(_moveTileClass, false);

                            _arriveTileGroupClass.AddTile_Func(_moveTileClass);
                        }
                    }
                }

                return _isMovable;
            }
            else
            {
                return false;
            }
        }
        public void OnClear_Func()
        {
            foreach (var _item in this.tileKeyByTileClassListDic)
                _item.Value.Clear();

            for (int _x = 0; _x < this.GetMaxTilePosX; _x++)
            {
                for (int _y = 0; _y < this.GetMaxTilePosY; _y++)
                {
                    if(this.tileGroupDicDic.TryGetValue(_x, out Dictionary<int, TileGroup> _dic) == true)
                    {
                        if(_dic.TryGetValue(_y, out TileGroup _tileGroupClass) == true)
                        {
                            _tileGroupClass.OnClear_Func();
                        }
                    }
                }
            }
        }

        // 타일 범위 안에 있는가?
        public bool CheckInOfRange_Func(TilePosData _posData)
        {
            return CheckInOfRange_Func(_posData.X, _posData.Y);
        }
        public bool CheckInOfRange_Func(int _x, int _y)
        {
            bool _isInOfRange = true;
            if (this.maxTilePosX <= _x || _x < 0)
            {
                _isInOfRange = false;

                Debug_C.Log_Func($"타일 X 최대값을 초과했습니다. 최대/호출 : {this.maxTilePosX}/{_x})", Debug_C.PrintLogType.TileSystem_C);
            }

            if (this.maxTilePosY <= _y || _y < 0)
            {
                _isInOfRange = false;
                
                Debug_C.Log_Func($"타일 Y 최대값을 초과했습니다. 최대/호출 : {this.maxTilePosY}/{_y})", Debug_C.PrintLogType.TileSystem_C);
            }

            return _isInOfRange;
        }
        public bool CheckInOfRange_Func(Vector2 _pos, bool _isScreenPos = true)
        {
            if(_isScreenPos == true)
                _pos = this.GetTouchWorldPos_Func(_pos);

            TilePosData _tilePosData = this.GetTilePosData_Func(_pos);
            return this.CheckInOfRange_Func(_tilePosData);
        }
        public int GetVaildFieldX_Func(int _x)
        {
            this.TryGetVaildFieldX_Func(_x, out _x);

            return _x;
        }
        public bool TryGetVaildFieldX_Func(int _x, out int _vaildX)
        {
            if (this.GetMaxTilePosX <= _x)
            {
                _vaildX = this.GetMaxTilePosX - 1;
                return false;
            }
            else if (_x < 0)
            {
                _vaildX = 0;
                return false;
            }
            else
            {
                _vaildX = _x;
                return true;
            }
        }
        public int GetVaildFieldY_Func(int _y)
        {
            this.TryGetVaildFieldY_Func(_y, out _y);

            return _y;
        }
        public bool TryGetVaildFieldY_Func(int _y, out int _vaildY)
        {
            if (this.GetMaxTilePosY <= _y)
            {
                _vaildY = this.GetMaxTilePosY - 1;
                return false;
            }
            else if (_y < 0)
            {
                _vaildY = 0;
                return false;
            }
            else
            {
                _vaildY = _y;
                return true;
            }
        }
        public bool TryGetTile_Func<T>(Tile _tile, string _tileTypeStr, out T _tileClass) where T : Tile
        {
            return this.TryGetTile_Func(_tile.GetTilePosData, _tileTypeStr, out _tileClass);
        }
        public bool TryGetTile_Func<T>(TilePosData _data, string _tileTypeStr, out T _tileClass) where T : Tile
        {
            return this.TryGetTile_Func(_data.X, _data.Y, _tileTypeStr, out _tileClass);
        }
        public bool TryGetTile_Func<T>(int _x, int _y, string _tileTypeStr, out T _tileClass) where T : Tile
        {
            if (CheckInOfRange_Func(_x, _y) == true)
            {
                TileGroup _tileGroupClass = this.GetTileGroupClass_Func(_x, _y);

                if (_tileGroupClass.TryGetTile_Func(_tileTypeStr, out List<Tile> _tileClassList) && _tileClassList.IsHave_Func() == true)
                {
                    _tileClass = _tileClassList[0] as T;
                    return true;
                }
                else
                {
                    _tileClass = null;
                    return false;
                }
            }
            else
            {
                Debug_C.Warning_Func("필드의 영역을 벗어났습니다.");

                _tileClass = null;

                return false;
            }
        }
        public bool[] GetTileArr_Func(TilePosData _posData, out Tile[] _tileClassArr, params string[] _tileTypeStrArr)
        {
            return GetTileArr_Func(_posData.X, _posData.Y, out _tileClassArr, _tileTypeStrArr);
        }
        public bool[] GetTileArr_Func(int _x, int _y, out Tile[] _tileClassArr, params string[] _tileTypeStrArr)
        {
            TileGroup _tileGroupClass = this.GetTileGroupClass_Func(_x, _y);

            bool[] _isReturnArr = new bool[_tileTypeStrArr.Length];
            _tileClassArr = new Tile[_tileTypeStrArr.Length];

            for (int i = 0; i < _tileTypeStrArr.Length; i++)
            {
                if(_tileGroupClass.TryGetTile_Func(_tileTypeStrArr[i], out List<Tile> _tileList) && _tileList.IsHave_Func() == true)
                {
                    _tileClassArr[i] = _tileList[0];

                    _isReturnArr[i] = true;
                }
                else
                {
                    _isReturnArr[i] = false;
                }
            }

            return _isReturnArr;
        }

        /// <summary>
        /// 직진 방향에서 타일 찾기
        /// </summary>
        public bool TryGetTile_Func<TileClass>(TilePosData _tilePosData, DirectionType _checkDir, string _tileTypeStr, out TileClass _getTileClass
            , Func<TileClass, bool> _conditionDel = null, int _loopCnt = -1) where TileClass : Tile
            => this.TryGetTile_Func(_tilePosData.X, _tilePosData.Y, _checkDir, _tileTypeStr, out _getTileClass, _conditionDel, _loopCnt);

        /// <summary>
        /// 직진 방향에서 타일 찾기
        /// </summary>
        public bool TryGetTile_Func<TileClass>(int _x, int _y, DirectionType _checkDir, string _tileTypeStr, out TileClass _getTileClass
            , Func<TileClass, bool> _conditionDel = null, int _loopCnt = -1) where TileClass : Tile
        {
            bool _isOutOfRange = true;
            switch (_checkDir)
            {
                case DirectionType.Left:
                    {
                        if (0 < _x)
                        {
                            _isOutOfRange = false;
                            _x--;
                        }
                    }
                    break;

                case DirectionType.Down:
                    {
                        if (0 < _y)
                        {
                            _isOutOfRange = false;
                            _y--;
                        }
                    }
                    break;

                case DirectionType.Up:
                    {
                        if (_y < this.GetMaxTilePosY - 1)
                        {
                            _isOutOfRange = false;
                            _y++;
                        }
                    }
                    break;

                case DirectionType.Right:
                    {
                        if (_x < this.GetMaxTilePosX - 1)
                        {
                            _isOutOfRange = false;
                            _x++;
                        }
                    }
                    break;

                default:
                    Debug_C.Error_Func("_checkDir : " + _checkDir);
                    break;
            }

            if (_isOutOfRange == false)
            {
                bool _isLoop = false;
                if (_loopCnt == -1)
                {
                    _isLoop = true;
                }
                else if(0 < _loopCnt)
                {
                    _isLoop = true;

                    _loopCnt--;
                }

                if (_isLoop == true)
                {
                    TileGroup _tileGroupClass = this.GetTileGroupClass_Func(_x, _y);
                    if (_tileGroupClass.TryGetTile_Func(_tileTypeStr, out List<Tile> _getTileClassList) == true)
                    {
                        if (_getTileClassList.IsHave_Func() == true)
                        {
                            _getTileClass = _getTileClassList[0] as TileClass;
                            if (_conditionDel == null) return true;
                            if (_conditionDel(_getTileClass) == true) return true;
                        }
                    }

                    return this.TryGetTile_Func(_x, _y, _checkDir, _tileTypeStr, out _getTileClass, _conditionDel, _loopCnt);
                }
                else
                {
                    _getTileClass = default;

                    return false;
                }
            }
            else
            {
                _getTileClass = default;

                return false;
            }
        }
        public bool TryGetTile_Func(string _tileTypeStr, out List<Tile> _tileClassList)
        {
            return this.tileKeyByTileClassListDic.TryGetValue(_tileTypeStr, out _tileClassList);
        }

        public bool TryGetNearEmptyTile_Func(TilePosData _initPosData, out TilePosData _findTilePosData,
            DirectionType _beginDirType = DirectionType.Right, int _rangeLimit = 0, bool _isClockwise = true, bool _isIncludeCenter = true, params string[] _checkTypeArr)
        {
            return this.TryGetNearEmptyTile_Func(_initPosData.X, _initPosData.Y, out _findTilePosData, _rangeLimit, _beginDirType, _isClockwise, _isIncludeCenter, _checkTypeArr);
        }
        public bool TryGetNearEmptyTile_Func(int _initX, int _initY, out TilePosData _findTilePosData
            , int _rangeLimit = 0, DirectionType _beginDirType = DirectionType.Right, bool _isClockwise = true, bool _isIncludeCenter = true, params string[] _checkTypeArr)
        {
            if(_isIncludeCenter == true)
            {
                if (this.TryGetVaildFieldX_Func(_initX, out int _x) == true && this.TryGetVaildFieldY_Func(_initY, out int _y) == true)
                {
                    TileGroup _tileGroupClass = this.GetTileGroupClass_Func(_x, _y);
                    if (_tileGroupClass.IsTileHave_Func(_checkTypeArr) == true)
                    {
                        _findTilePosData = new TilePosData(_x, _y);
                        return true;
                    }
                }
            }

            int _loopCnt = 10000;
            int _checkX = _initX;
            int _checkY = _initY;
            int _sideCheckMaxCnt = 1;
            DirectionType _searchDir = _beginDirType;

            while (true)
            {
                _loopCnt--;
                if(_loopCnt < 0)
                {
                    Debug_C.Error_Func("!");

                    _findTilePosData = default;
                    return false;
                }

                Debug_C.Log_Func("3. 거리 검색) _sideCheckMaxCnt : " + _sideCheckMaxCnt);

                bool _isOutOfRange = true;

                #region 2. 방향 검색
                for (int i = 0; i < 2;)
                {
                    _loopCnt--;
                    if (_loopCnt < 0)
                    {
                        Debug_C.Error_Func("!");

                        _findTilePosData = default;
                        return false;
                    }

                    int _sideCheckCnt = _sideCheckMaxCnt;

                    Debug_C.Log_Func("2. 방향 검색) i : " + i + " / _sideCheckMaxCnt : " + _sideCheckMaxCnt);

                    #region 1. 면 검색
                    while (true)
                    {
                        _loopCnt--;
                        if (_loopCnt < 0)
                        {
                            Debug_C.Error_Func("!");

                            _findTilePosData = default;
                            return false;
                        }

                        if (0 < _sideCheckCnt)
                        {
                            switch (_searchDir)
                            {
                                case DirectionType.Left:
                                    _checkX--;
                                    break;

                                case DirectionType.Down:
                                    _checkY--;
                                    break;

                                case DirectionType.Up:
                                    _checkY++;
                                    break;

                                case DirectionType.Right:
                                    _checkX++;
                                    break;

                                default:
                                    Debug_C.Error_Func("_searchDir : " + _searchDir);
                                    break;
                            }
                        }
                        else
                        {
                            break;
                        }

                        Debug_C.Log_Func("1. 면 검색) " + _checkX + "_" + _checkY + " / _sideCheckCnt : " + _sideCheckCnt);

                        if (this.TryGetVaildFieldX_Func(_checkX, out int _x) == true && this.TryGetVaildFieldY_Func(_checkY, out int _y) == true)
                        {
                            _isOutOfRange = false;

                            TileGroup _tileGroupClass = this.GetTileGroupClass_Func(_x, _y);
                            if (_tileGroupClass.IsTileHave_Func(_checkTypeArr) == true)
                            {
                                _findTilePosData = new TilePosData(_x, _y);
                                return true;
                            }

                            Debug_C.Log_Func("In Range");
                        }
                        else
                        {
                            Debug_C.Log_Func("Out Range");
                        }

                        _sideCheckCnt--;
                    }
                    #endregion

                    Debug_C.Log_Func("1. 면 검색 종료) : " + _searchDir);

                    DirectionType _orginSearchDir = _searchDir;

                    switch (_searchDir)
                    {
                        case DirectionType.Left:
                            {
                                if (Mathf.Abs((int)_beginDirType) == 1)
                                {
                                    i++;
                                    _sideCheckMaxCnt++;
                                }
                            
                                _searchDir = _isClockwise == true ? DirectionType.Up : DirectionType.Down;
                            }
                            break;

                        case DirectionType.Down:
                            {
                                if (Mathf.Abs((int)_beginDirType) == 2)
                                {
                                    i++;
                                    _sideCheckMaxCnt++;
                                }

                                _searchDir = _isClockwise == true ? DirectionType.Left : DirectionType.Right;
                            }
                            break;

                        case DirectionType.Up:
                            {
                                if (Mathf.Abs((int)_beginDirType) == 2)
                                {
                                    i++;
                                    _sideCheckMaxCnt++;
                                }

                                _searchDir = _isClockwise == true ? DirectionType.Right : DirectionType.Left;
                            }
                            break;

                        case DirectionType.Right:
                            {
                                if (Mathf.Abs((int)_beginDirType) == 1)
                                {
                                    i++;
                                    _sideCheckMaxCnt++;
                                }

                                _searchDir = _isClockwise == true ? DirectionType.Down : DirectionType.Up;
                            }
                            break;

                        default:
                            Debug_C.Error_Func("_searchDir : " + _searchDir);
                            break;
                    }

                    Debug_C.Log_Func("1. 면 검색, 방향 전환) Origin : " + _orginSearchDir + " / Next : " + _searchDir + " / _sideCheckMaxCnt : " + _sideCheckMaxCnt);
                }
                #endregion

                if (_isOutOfRange == true)
                    break;

                if (0 < _rangeLimit)
                {
                    int _currentDist = (_sideCheckMaxCnt - 1) / 2;
                    if (_rangeLimit < _currentDist)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            _findTilePosData = default;
            return false;
        }

        public TileGroup GetTileGroupClass_Func(TilePosData _posData)
        {
            return this.GetTileGroupClass_Func(_posData.X, _posData.Y);
        }
        public TileGroup GetTileGroupClass_Func(int _x, int _y, bool _isSafeRange = true)
        {
            if (this.CheckInOfRange_Func(_x, _y) == false)
            {
                if(_isSafeRange == true)
                {
                    _x = this.GetVaildFieldX_Func(_x);
                    _y = this.GetVaildFieldY_Func(_y);
                }
            }

            Dictionary<int, TileGroup> _tileGroupDic = this.tileGroupDicDic.GetValue_Func(_x, () => new Dictionary<int, TileGroup>());

            return _tileGroupDic.GetValue_Func(_y, () => new TileGroup(_x, _y));
        }

        public Vector2 GetWorldPos_Func(Tile _tile)
        {
            return this.GetWorldPos_Func(_tile.GetTilePosData);
        }
        public Vector2 GetWorldPos_Func(TilePosData _tilePosData)
        {
            return this.GetWorldPos_Func(_tilePosData.X, _tilePosData.Y);
        }
        public Vector2 GetWorldPos_Func(int _posX, int _posY)
        {
            float _tilePosX = _posX * this.TileSpace.x;
            float _tilePosY = _posY * this.TileSpace.y;

            Vector2 _initPos = this.TilePos_InitData;
            _tilePosX += _initPos.x;
            _tilePosY += _initPos.y;

            return new Vector2(_tilePosX, _tilePosY);
        }
        public TilePosData GetTilePosData_Func(Vector2 _worldPos)
        {
            float _x = _worldPos.x - this.TilePos_InitData.x;
            float _y = _worldPos.y - this.TilePos_InitData.y;
            int _xID = (_x / this.TileSpace.x).ToInt(true);
            int _yID = (_y / this.TileSpace.y).ToInt(true);
            return new TilePosData(_xID, _yID);
        }

        public void Subscribe_AddTile_Func(Action<TilePosData, Tile> _del)
        {
            this.addTileClassObs.Subscribe_Func(_del);
        }
        public void Unsubscribe_AddTile_Func(Action<TilePosData, Tile> _del)
        {
            this.addTileClassObs.Unsubscribe_Func(_del);
        }
        public void Subscribe_RemoveTile_Func(Action<TilePosData, Tile> _del)
        {
            this.removeTileClassObs.Subscribe_Func(_del);
        }
        public void Unsubscribe_RemoveTile_Func(Action<TilePosData, Tile> _del)
        {
            this.removeTileClassObs.Unsubscribe_Func(_del);
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if(_isInit == false)
            {

            }

            this.touchCorData.StopCorountine_Func();
        }

        public static void GetDirType_Func(TilePosData _xPosData, TilePosData _yPosData, out DirectionType _dirTypeSide, out DirectionType _dirTypeExid)
        {
            if (_xPosData.X < _yPosData.X)
            {
                _dirTypeSide = DirectionType.Right;
            }
            else if (_xPosData.X > _yPosData.X)
            {
                _dirTypeSide = DirectionType.Left;
            }
            else
            {
                _dirTypeSide = DirectionType.None;
            }

            if (_xPosData.Y < _yPosData.Y)
            {
                _dirTypeExid = DirectionType.Up;
            }
            else if (_xPosData.Y > _yPosData.Y)
            {
                _dirTypeExid = DirectionType.Down;
            }
            else
            {
                _dirTypeExid = DirectionType.None;
            }
        }
        public static bool TryGetDirType_Func(TilePosData _xPosData, TilePosData _yPosData, WayType _wayType, out DirectionType _dirType)
        {
            if (_wayType == WayType.Side)
            {
                if (_xPosData.X < _yPosData.X)
                {
                    _dirType = DirectionType.Right;
                }
                else if (_xPosData.X > _yPosData.X)
                {
                    _dirType = DirectionType.Left;
                }
                else
                {
                    _dirType = DirectionType.None;
                }

                return _dirType != DirectionType.None;
            }
            else if (_wayType == WayType.Exid)
            {
                if (_xPosData.Y < _yPosData.Y)
                {
                    _dirType = DirectionType.Up;
                }
                else if (_xPosData.Y > _yPosData.Y)
                {
                    _dirType = DirectionType.Down;
                }
                else
                {
                    _dirType = DirectionType.None;
                }

                return _dirType != DirectionType.None;
            }
            else
            {
                _dirType = default;

                Debug_C.Error_Func("_wayType : " + _wayType);

                return false;
            }
        }
        public static int GetDistance_Func(TilePosData _xPosData, TilePosData _yPosData, WayType _wayType)
        {
            int _x = 0;
            int _y = 0;

            if (_wayType == WayType.Side)
            {
                _x = _xPosData.X;
                _y = _yPosData.X;
            }
            else if (_wayType == WayType.Exid)
            {
                _x = _xPosData.Y;
                _y = _yPosData.Y;
            }
            else
            {
                Debug_C.Error_Func("_wayType : " + _wayType);
            }

            if (_x < _y)
            {
                return _y - _x;
            }
            else if (_x > _y)
            {
                return _x - _y;
            }
            else
            {
                return 0;
            }
        }
        public static TilePosData GetTilePosData_Func(Vector2 _pos, Vector2 _offset = default)
        {
            int _xPos = (_pos.x + _offset.x).ToInt();
            int _yPos = (_pos.y + _offset.y).ToInt();
            return new TilePosData(_xPos, _yPos);
        }
    }

    public enum WayType
    {
        None = 0,

        Side = 10,
        Exid = 20, // 위 아래를 의미하는 단어를 지웅 형한테 물어보니 이거 알려줌 ^^
    }

    #region TilePosData
    [System.Serializable]
    public struct TilePosData
    {
        [SerializeField] private int x;
        [SerializeField] private int y;

        public TilePosData(int _x, int _y)
        {
            this.x = _x;
            this.y = _y;
        }

        public int X { get { return this.x; } }
        public int Y { get { return this.y; } }
        public bool IsEmpty => this.x == -1 || this.y == -1;

        public int GetDistance_Func(TilePosData _targetTilePosData, bool _isVaildCross = false)
        {
            return TilePosData.GetDistance_Func(this, _targetTilePosData, _isVaildCross);
        }

        public static TilePosData GetPos_Func(Tile _tileClass, DirectionType _dirType, int _range = 1)
        {
            int _x = _tileClass.GetX;
            int _y = _tileClass.GetY;

            return TilePosData.GetPos_Func(_x, _y, _dirType, _range);
        }
        public static TilePosData GetPos_Func(TilePosData _posData, DirectionType _dirType, int _range = 1)
        {
            return TilePosData.GetPos_Func(_posData.x, _posData.y, _dirType, _range);
        }
        public static TilePosData GetPos_Func(int _x, int _y, DirectionType _dirType, int _range = 1)
        {
            switch (_dirType)
            {
                case DirectionType.Up:
                    _y += _range;
                    break;
                case DirectionType.Right:
                    _x += _range;
                    break;
                case DirectionType.Down:
                    _y -= _range;
                    break;
                case DirectionType.Left:
                    _x -= _range;
                    break;
            }

            return new TilePosData(_x, _y);
        }
        public static TilePosData GetEmpty_Func() => new TilePosData(-1, -1);
        public static TilePosData[] GetNearDataArr_Func(TilePosData _center)
        {
            return new TilePosData[]
            {
                new TilePosData(_center.X + 1, _center.Y + 1),
                new TilePosData(_center.X + 1, _center.Y),
                new TilePosData(_center.X + 1, _center.Y - 1),
                new TilePosData(_center.X, _center.Y + 1),
                new TilePosData(_center.X, _center.Y - 1),
                new TilePosData(_center.X - 1, _center.Y + 1),
                new TilePosData(_center.X - 1, _center.Y),
                new TilePosData(_center.X - 1, _center.Y - 1),
            };
        }
        public static int GetDistance_Func(TilePosData _leftPosData, TilePosData _rightPosData, bool _isVaildCross = false)
        {
            int _xGap = Mathf.Abs(_leftPosData.x - _rightPosData.x);
            int _yGap = Mathf.Abs(_leftPosData.y - _rightPosData.y);

            if (_isVaildCross == false)
            {
                return _xGap + _yGap;
            }
            else
            {
                return _xGap < _yGap ? _yGap : _xGap;
            }
        }

        public static implicit operator TilePosData(Tile _tileClass)
        {
            if (_tileClass != null)
                return _tileClass.GetTilePosData;
            else
                return default;
        }
        public static implicit operator string(TilePosData _tilePosData)
        {
            return _tilePosData.ToString();
        }
        public static bool operator ==(TilePosData _left, TilePosData _right)
        {
            return _left.x == _right.x && _left.y == _right.y;
        }
        public static bool operator !=(TilePosData _left, TilePosData _right)
        {
            return _left.x != _right.x || _left.y != _right.y;
        }


        public override string ToString()
        {
            return "(" + this.x + ", " + this.y + ")";
        }
    }
    #endregion

    public static class TileSystemExtension
    {
        public static DirectionType GetReverseDirType_Func(this DirectionType _dirType)
        {
            switch (_dirType)
            {
                case DirectionType.Left:    return DirectionType.Right;
                case DirectionType.Down:    return DirectionType.Up;
                case DirectionType.Up:      return DirectionType.Down;
                case DirectionType.Right:   return DirectionType.Left;

                default:
                    Debug_C.Error_Func("_dirType : " + _dirType);
                    return default;
            }
        }
        public static DirectionType GetDir_Func(this TilePosData _tilePosData, TilePosData _targetTilePosData)
        {
            return TileSystemExtension.GetDir_Func(_tilePosData, _targetTilePosData.X, _targetTilePosData.Y);
        }
        public static DirectionType GetDir_Func(this TilePosData _tilePosData, int _targetX, int _targetY)
        {
            int _gapX = _tilePosData.X - _targetX;
            int _gapY = _tilePosData.Y - _targetY;
            float _absGapX = System.Math.Abs(_gapX);
            float _absGapY = UnityEngine.Mathf.Abs(_gapY);

            if (_absGapX == 0 && _absGapY == 0)
            {
                return DirectionType.None;
            }
            else if(_absGapX == _absGapY)
            {
                if(_tilePosData.X < _targetX)
                {
                    return _tilePosData.Y < _targetY
                        ? Random_C.IsRandom_Func(.5f) ? DirectionType.Up : DirectionType.Right
                        : Random_C.IsRandom_Func(.5f) ? DirectionType.Down : DirectionType.Right;
                }
                else
                {
                    return _tilePosData.Y < _targetY
                        ? Random_C.IsRandom_Func(.5f) ? DirectionType.Up : DirectionType.Left
                        : Random_C.IsRandom_Func(.5f) ? DirectionType.Down : DirectionType.Left;
                }
            }

            if (_gapY == 0 || _absGapY < _absGapX)
            {
                return 0 < _gapX ? DirectionType.Left : DirectionType.Right;
            }
            else if(_gapX == 0 || _absGapX < _absGapY)
            {
                return 0 < _gapY ? DirectionType.Down : DirectionType.Up;
            }
            else
            {
                return DirectionType.None;
            }
        }
    }

    // 타일 영역이 실시간으로 커지거나 작아지는 기능 추가
}