using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.HeightedGacha
{
    public static class HeightedGacha
    {
        public static IHeightedGachaElem GetGatchElem_Func(this IHeightedGachaContainer _iGachaContainer, IEnumerable<IHeightedGachaElem> _exceptElemList = null)
        {
            return GetGatchElem_Func(_iGachaContainer, out _, _exceptElemList);
        }
        public static IHeightedGachaElem GetGatchElem_Func(this IHeightedGachaContainer _iGachaContainer, out int _id, IEnumerable<IHeightedGachaElem> _exceptElemList = null)
        {
            int _totalPer = _iGachaContainer.GetTotalHeight;

            if(_exceptElemList != null)
            {
                int _decreasePer = 0;
                foreach (var _exceptElem in _exceptElemList)
                {
                    _decreasePer += _exceptElem.GetHeight;
                }

                _totalPer -= _decreasePer;
            }

            int _randPer = Random.Range(1, _totalPer + 1);

            IEnumerable<IHeightedGachaElem> _elemCollection = _iGachaContainer.GetGachaElemList;
            _id = 0;
            int _elemNum = 0;
            foreach (IHeightedGachaElem _elem in _elemCollection)
                _elemNum++;

            IHeightedGachaElem _returnElem = null;
            foreach (IHeightedGachaElem _elem in _elemCollection)
            {
                if (_exceptElemList != null)
                {
                    bool _isExcept = false;
                    foreach (var _exceptElem in _exceptElemList)
                    {
                        if(_elem == _exceptElem)
                        {
                            _isExcept = true;
                            break;
                        }
                    }

                    if(_isExcept == true)
                    {
                        _id++;
                        continue;
                    }
                }

                _randPer -= _elem.GetHeight;

                if(_randPer <= 0)
                {
                    _returnElem = _elem;
                    break;
                }
                else
                {
                    if(_elemNum <= _id + 1)
                    {
                        _returnElem = _elem;
                        break;
                    }
                    else
                    {
                        _id++;
                    }
                }
            }

            return _returnElem;
        }

        public static int GetTotalHeight_Func(this IHeightedGachaContainer _iGachaContainer)
        {
            int _totalHeight = 0;

            IEnumerable<IHeightedGachaElem> _elemArr = _iGachaContainer.GetGachaElemList;
            foreach (var _elem in _elemArr)
            {
                _totalHeight += _elem.GetHeight;
            }

            return _totalHeight;
        }
    }

    public interface IHeightedGachaContainer
    {
        public int GetTotalHeight { get; }
        public IEnumerable<IHeightedGachaElem> GetGachaElemList { get; }
    }
    public interface IHeightedGachaElem
    {
        public int GetHeight { get; }
    }
}