using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold
{
    /// <summary>
    /// 수학적인 로직 함수들이 모여있는 전역 클래스입니다.
    /// </summary>
    public static class Calc_C
    {
        /// <summary>
        /// 추가 경험치에 따른 최종 레벨이 몇인지?
        /// </summary>
        /// <param name="_currentLv">현재 레벨</param>
        /// <param name="_currentExp">현재 경험치</param>
        /// <param name="_addExp">추가 경험치</param>
        /// <param name="_lvUpDataArr">레벨 데이터 배열</param>
        /// <param name="_afterLv">최종 레벨</param>
        /// <param name="_afterExp">최종 경험치</param>
        /// <param name="_isMaxLv">최대 레벨 도달 여부</param>
        public static void CalcLv_Func
            (int _currentLv, int _currentExp, int _addExp, ICalc_LvUpData[] _lvUpDataArr, out int _afterLv, out int _afterExp, out bool _isMaxLv, int _maxLv = -1)
        {
            if (_maxLv == -1)
                _maxLv = _lvUpDataArr.Length;
            else
            {
                if(_lvUpDataArr.Length <= _maxLv)
                    Debug.LogError($"만렙이 배열 크기보다 높습니다. 배열크기 : {_lvUpDataArr.Length} / 만렙 : {_maxLv}");
            }

            int _remainExp = _currentExp + _addExp;
            ICalc_LvUpData _lvUpData = null;
            int _dataID = _currentLv - 1;
            _isMaxLv = false;

            while (true)
            {
                if (_dataID < _maxLv)
                {
                    if(_lvUpDataArr.TryGetItem_Func(_dataID, out _lvUpData) == true)
                    {
                        if (_lvUpData.GetNeedExp <= _remainExp)
                        {
                            _dataID++;

                            _remainExp -= _lvUpData.GetNeedExp;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogError($"배열 크기를 초과했습니다. 배열크기 : {_lvUpDataArr.Length} / 순회 ID : {_dataID}");
                        break;
                    }
                }
                else
                {
                    _isMaxLv = true;

                    break;
                }
            }

            _afterLv = _dataID + 1;
            _afterExp = _remainExp;
        }

        public interface ICalc_LvUpData
        {
            public int GetNeedExp { get; }
        }
    }
}