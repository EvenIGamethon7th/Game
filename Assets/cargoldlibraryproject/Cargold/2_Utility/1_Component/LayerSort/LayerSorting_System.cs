using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace LayerSort
{
    using Cargold.FrameWork;
    using Cargold.TileSystem;
    using UnityEngine.Rendering;

    public abstract class LayerSorting_System<T> : MonoBehaviour, GameSystem_Manager.IInitializer
    {
        [ShowInInspector, ReadOnly] private Dictionary<T, int> layerGapDic;

        // 타일간 레이어 간격
        [ShowInInspector, ReadOnly] private int layerGap;

        protected int GetLayerGap { get { return this.layerGap; } }
        /// <summary>
        /// 타입 간 레이어 간격값. 하나의 타입에 많은 레이어 구분이 필요할 경우 재정의하여 값을 10보다 키우면 됨
        /// </summary>
        protected virtual int GetTypeGap => 10;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                layerGapDic = new Dictionary<T, int>();

                T[] _typeArr = Init_LayerType_Func();

                Init_TileGap_Func(_typeArr);
            }
        }

        /// <summary>
        /// 레이어 타입 종류들. 배열 앞일 수록 레이어가 앞으로 설정됩니다.
        /// </summary>
        /// <returns></returns>
        protected abstract T[] Init_LayerType_Func();

        // 타일간 레이어 간격값
        private void Init_TileGap_Func(params T[] _typeArr)
        {
            int _typeGap = this.GetTypeGap;

            for (int i = _typeArr.Length - 1; i >= 0; i--)
            {
                int _keyCount = 0;

                _keyCount = this.layerGapDic.Keys.Count;

                int _layerRangeValue = _typeGap * _keyCount;

                this.layerGapDic.Add_Func(_typeArr[i], _layerRangeValue);

                // 새로운 타입이 추가되었으므로 타일간 레이어 간격도 그만큼 확장한다.
                this.layerGap += _typeGap;
            }
        }

        public void SetLayerSort_Func(SpriteRenderer _spriteRend, T _layerType, TilePosData _posData, int _layerExtraID = 0)
        {
            this.SetLayerSort_Func(_spriteRend, _layerType, _posData.Y, _layerExtraID);
        }
        public void SetLayerSort_Func(SpriteRenderer _spriteRend, T _layerType, Tile _tileClass, int _layerExtraID = 0)
        {
            this.SetLayerSort_Func(_spriteRend, _layerType, _tileClass.GetY, _layerExtraID);
        }

        /// <summary>
        /// Srdr의 레이어를 조정합니다.
        /// </summary>
        /// <param name="_spriteRend"></param>
        /// <param name="_layerType"></param>
        /// <param name="_posY">값이 낮을 수록 레이어 앞으로 나옵니다.</param>
        /// <param name="_extraLayerID">추가적으로 레이어를 앞으로 빼고 싶은 값</param>
        public void SetLayerSort_Func(SpriteRenderer _spriteRend, T _layerType, int _posY, int _extraLayerID = 0)
        {
            int _typeGap = 1;
            if (this.layerGapDic.TryGetValue(_layerType, out _typeGap) == false)
                Debug_C.Warning_Func("다음 타입의 레이어는 초기화되지 않았습니다. : " + _layerType);

            // 스프라이트의 타일 Y값만큼 타일 간격을 곱하여 레이어를 정렬한다.
            int _layerSortID = _posY * this.layerGap;

            // 스프라이트의 타입만큼 레이어를 조금 더 정렬한다.
            _layerSortID += _typeGap;

            // 정렬값을 역전하여 Y축 값이 작을 수록 레이어가 앞에 나오도록 한다.
            _layerSortID *= -1;

            // 임의 레이어값만큼 레이어를 조금 더 정렬한다.
            _layerSortID += _extraLayerID;

            _spriteRend.sortingOrder = _layerSortID;

            Debug_C.Log_Func("_layerType : " + _layerType + " / _posY : " + _posY + " / _layerSortID : " + _layerSortID);
        }
    }
}