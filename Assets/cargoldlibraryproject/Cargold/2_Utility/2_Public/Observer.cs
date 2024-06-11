using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.Observer
{
    // 구독자 관리 클래스
    [System.Serializable]
    public class Observer_Manager<SubscriberType>
    {
        [SerializeField] protected List<SubscriberType> subscriberList;

        public Observer_Manager()
        {
            subscriberList = new List<SubscriberType>();
        }

        // 구독
        public bool Subscribe_Func(SubscriberType _subscriber, int _insertID = -1, bool _isEnableOverlap = false)
        {
            bool _isContainListener = subscriberList.Contains(_subscriber);

            bool _isAddable = true;
            if (_isEnableOverlap == false)
            {
                if (_isContainListener == true)
                {
                    _isAddable = false;

                    Debug_C.Warning_Func("중복 구독 : " + _subscriber);
                }
            }

            if (_isAddable == true)
            {
                if (_insertID == -1)
                    subscriberList.Add(_subscriber);
                else
                    subscriberList.Insert(_insertID, _subscriber);
            }

            return _isContainListener;
        }

        // 구독 전체 해지
        public bool UnsubscribeAll_Func()
        {
            // 구독 전체 해제
            // 구독자가 있는가?

            if (0 < subscriberList.Count)
            {
                subscriberList.Clear();

                return true;
            }
            else
            {
                return false;
            }
        }

        // 특정 구독자만 해지
        public bool Unsubscribe_Func(SubscriberType _subscriber, bool _isLog = true)
        {
            if (0 < this.subscriberList.Count && this.subscriberList.Contains(_subscriber) == true)
            {
                this.subscriberList.Remove(_subscriber);

                return true;
            }
            else
            {
                if (_isLog == true)
                    Debug_C.Warning_Func("해지할 대상이 애초에 구독하고 있지 않음 : " + _subscriber);

                return false;
            }
        }

        // 특정 구독자의 구독 여부
        public bool IsSubscribed_Func(SubscriberType _subscriber)
        {
            if (this.subscriberList.Count == 0)
                return false;

            return this.subscriberList.Contains(_subscriber);
        }

        public bool IsAnySubcriber_Func()
        {
            return 0 < this.subscriberList.Count;
        }

        // 구독자 숫자
        public int GetSubscriberNum_Func()
        {
            return this.subscriberList.Count;
        }

        public bool HasSubscriber { get { return 0 < this.subscriberList.Count ? true : false; } }

        // 모든 구독자에게 접근
        public void AccessWholeSubscriber_Func(Action<SubscriberType> _del)
        {
            foreach (SubscriberType _subscriber in this.subscriberList)
                _del(_subscriber);
        }
    }
    #region Action 0
    public class Observer_Action : Observer_Manager<Action>
    {
        public bool Notify_Func()
        {
            // 등록된 모든 구독자에게 알림
            // 구독자가 있는지 확인

            if (0 < subscriberList.Count)
            {
                for (int i = subscriberList.Count - 1; 0 <= i; --i)
                {
                    subscriberList[i]();
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion
    #region Action 1
    [System.Serializable]
    public class Observer_Action<T> : Observer_Manager<Action<T>>
    {
        public bool Notify_Func(T _t)
        {
            // 등록된 모든 구독자에게 알림
            // 구독자가 있는지 확인

            if (0 < subscriberList.Count)
            {
                for (int i = subscriberList.Count - 1; 0 <= i; --i)
                {
                    subscriberList[i](_t);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion
    #region Action 2
    [System.Serializable]
    public class Observer_Action<T1, T2> : Observer_Manager<Action<T1, T2>>
    {
        public bool Notify_Func(T1 _t1, T2 _t2)
        {
            // 등록된 모든 구독자에게 알림
            // 구독자가 있는지 확인

            if (0 < subscriberList.Count)
            {
                for (int i = subscriberList.Count - 1; 0 <= i; --i)
                {
                    subscriberList[i](_t1, _t2);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion
    #region Action 3
    [System.Serializable]
    public class Observer_Action<T1, T2, T3> : Observer_Manager<Action<T1, T2, T3>>
    {
        public bool Notify_Func(T1 _t1, T2 _t2, T3 _t3)
        {
            // 등록된 모든 구독자에게 알림
            // 구독자가 있는지 확인

            if (0 < subscriberList.Count)
            {
                for (int i = subscriberList.Count - 1; 0 <= i; --i)
                {
                    subscriberList[i](_t1, _t2, _t3);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion
}