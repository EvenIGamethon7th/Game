using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.TileSystem
{
    public class TileGroup
    {
        [ShowInInspector, ReadOnly] private int xID;
        [ShowInInspector, ReadOnly] private int yID;
        [ShowInInspector, ReadOnly] private Dictionary<string, List<Tile>> tileGroupDic;

        public int GetX => this.xID;
        public int GetY => this.yID;

        public TileGroup(int _x, int _y)
        {
            this.tileGroupDic = new Dictionary<string, List<Tile>>();

            this.xID = _x;
            this.yID = _y;
        }
        public void AddTile_Func(Tile _tileClass)
        {
            string _tileType = _tileClass.GetTileTypeStr;
            List<Tile> _tileList = this.tileGroupDic.GetValue_Func(_tileType, () => new List<Tile>());

#if UNITY_EDITOR
            if (_tileList.Contains(_tileClass) == true)
                Debug_C.Error_Func("중복 : " + _tileClass);
            else
#endif
                _tileList.Add(_tileClass);
        }
        public void RemoveTile_Func(Tile _tileClass, bool _isDeactivateToTile)
        {
            string _tileType = _tileClass.GetTileTypeStr;

            if (this.tileGroupDic.TryGetValue(_tileType, out List<Tile> _tileList) == true)
            {
                if (_tileList.Contains(_tileClass) == true)
                {
                    _tileList.Remove(_tileClass);
                }
                else
                {
                    if (Debug_C.IsLogType_Func(Debug_C.PrintLogType.TileSystem_C) == true)
                    {
                        Debug_C.Log_Func("Remove : " + _tileClass.GetX + "_" + _tileClass.GetY, Debug_C.PrintLogType.TileSystem_C);
                        Debug_C.Log_Func("Type : " + _tileType, Debug_C.PrintLogType.TileSystem_C);
                        Debug_C.Warning_Func("다음 타일은 없습니다. : " + _tileType, Debug_C.PrintLogType.TileSystem_C);
                    }
                }

                if (_isDeactivateToTile == true)
                    _tileClass.Deactivate_Func();
            }
            else
            {
                if (Debug_C.IsLogType_Func(Debug_C.PrintLogType.TileSystem_C) == true)
                {
                    Debug_C.Log_Func("Remove : " + _tileClass.GetX + "_" + _tileClass.GetY, Debug_C.PrintLogType.TileSystem_C);
                    Debug_C.Log_Func("Type : " + _tileType, Debug_C.PrintLogType.TileSystem_C);
                    Debug_C.Warning_Func("다음 타일의 타입은 없습니다. : " + _tileType, Debug_C.PrintLogType.TileSystem_C);
                }
            }
        }
        public bool TryGetTile_Func(string _tileType, out List<Tile> _tileClassList)
        {
            bool _isResult = this.tileGroupDic.TryGetValue(_tileType, out _tileClassList);

            return _isResult;
        }
        public bool IsTileHave_Func(params string[] _checkTypeArr)
        {
            if(_checkTypeArr.IsHave_Func() == false)
            {
                return 0 < this.tileGroupDic.Count;
            }
            else
            {
                foreach (string _checkType in _checkTypeArr)
                {
                    if (this.tileGroupDic.ContainsKey(_checkType) == true)
                        return true;
                }

                return false;
            }
        }
        public void OnClear_Func()
        {
            foreach (var _item in this.tileGroupDic)
            {
                List<Tile> _tileClassList = this.tileGroupDic[_item.Key];

                foreach (Tile _tileClass in _tileClassList)
                    _tileClass.DeactivateWithoutRemoveTileSystem_Func();

                _tileClassList.Clear();
            }
        }
    }
}