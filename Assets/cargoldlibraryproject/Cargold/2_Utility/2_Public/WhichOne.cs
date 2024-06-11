using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.WhichOne
{
    [System.Serializable]
    public class WhichOne<T> where T : class, IWhichOne
    {
        [SerializeField] protected T whichOne;
        
        public bool IsHave => this.whichOne != null;

        public WhichOne()
        {
            whichOne = null;
        }
        public void Selected_Func(T _whichOne, bool _isCancelWhenTwiceSelected)
        {
            this.Selected_Func(_whichOne, _isCancelWhenTwiceSelected, out _);
        }
        public void Selected_Func(T _whichOne, bool _isCancelWhenTwiceSelected, out bool _isChange)
        {
            // 인자값을 선택 개체로 등록하고 '선택'이벤트 전달.
            // 만약 기 개체를 선택한 경우 선택 개체에게 중복 선택임을 알림
            // 만약 이미 선택 개체가 있다면, 기 선택 개체에게 '선택 해제'이벤트 전달

            if (this.whichOne == null)
            {
                _isChange = true;

                this.whichOne = _whichOne;

                _whichOne.Selected_Func();
            }
            else
            {
                if (this.whichOne == _whichOne)
                {
                    _whichOne.Selected_Func(true);

                    if (_isCancelWhenTwiceSelected == true)
                    {
                        _isChange = true;

                        this.ClearWhichOne_Func();
                    }
                    else
                        _isChange = false;
                }
                else
                {
                    _isChange = true;

                    this.whichOne.SelectCancel_Func();

                    this.whichOne = _whichOne;

                    _whichOne.Selected_Func();
                }
            }
        }
        public void SelectCancel_Func()
        {
            // 선택 해제. 선택 개체에게 선택 해제 이벤트 알림

            if (this.whichOne is null == false)
                this.whichOne.SelectCancel_Func();

            this.whichOne = null;
        }
        public T GetWhichOne_Func()
        {
            // 선택 개체 반환

            return this.whichOne;
        }
        public void SetWhichOne_Func(T _iWhichOne)
        {
            this.whichOne = _iWhichOne;
        }
        public bool Compare_Func(T _check)
        {
            // 인자값과 선택 개체가 동일한가?

            return this.whichOne == _check;
        }

        [Obsolete]
        public bool HasWhichOne_Func()
        {
            // 선택한 개체가 있는가?

            return this.whichOne is null == false;
        }
        public bool TryGetWhichOne_Func(out T _whichOne)
        {
            _whichOne = this.whichOne;

            return this.whichOne != null;
        }
        public void ClearWhichOne_Func()
        {
            this.whichOne = null;
        }
    }

    public interface IWhichOne
    {
        void Selected_Func(bool _isRepeat = false); // 선택됨
        void SelectCancel_Func(); // 선택 해제됨
    }

    // 1. 선택 순서를 기록하고 이를 역행하면서 선택 해제하고 싶다면?
    /*
     * List를 써서 순서 기록
     * 순서를 역순으로 돌아갈 수 있음
     * List Clear 시점은 Select 값이 없을 때?
     */

    // 2. 선택 개수를 2개 이상인 경우엔?
    /*
     * 선택 개수를 초과할 경우 가장 먼저 선택된 객체가 해제?
     */
}