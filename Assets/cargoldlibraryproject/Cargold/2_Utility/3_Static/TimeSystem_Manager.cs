using UnityEngine;
using System;
using System.Collections;
using Cargold;

namespace Cargold
{
    public class TimeSystem_Manager : Cargold.TimeSystem.TimeSystem_Manager
    {
        protected static TimeSystem_Manager instance;
        public static TimeSystem_Manager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TimeSystem_Manager();

                return instance;
            }
        }

        public TimeSystem_Manager() : base()
        {
            UnbiasedTimeManager.UnbiasedTime.Init();
            GameObject _timeObj = UnbiasedTimeManager.UnbiasedTime.Instance.gameObject;
            GameObject.DontDestroyOnLoad(_timeObj);

            instance = this;
        }

        public override DateTime Now
        {
            get
            {
                if (UnbiasedTimeManager.UnbiasedTime.Instance.IsFailed == true)
                    UnbiasedTimeManager.UnbiasedTime.Instance.Initialize();

                DateTime _nowTime = UnbiasedTimeManager.UnbiasedTime.Instance.dateTime;

                // 구글 NTP 서버에 접속하지 못하면 반환값이 1900년도임
                if (_nowTime.Year == 1900)
                    _nowTime = DateTime.Now; // 로컬 타임을 예외적으로 삽입한다.
                else
                    _nowTime += DateTimeOffset.Now.Offset; // 정상적인 값일 시 구글NTP로 반환된 값은 UTC +0임. 따라서 국가별로 시간을 보정하기 위해 offset 추가. 한국 기준 UTC +9

#if UNITY_EDITOR
                _nowTime = base.GetOffsetTime_Func(_nowTime);
#endif
                return _nowTime;
            }
        }

        public void Init_Func()
        {

        }
    }
}

namespace Cargold.TimeSystem
{
    using FrameWork;
    using System.Collections.Generic;

    public class TimeSystem_Manager
    {
        public const string Str = "TimeSystem";
        public const string KorStr = "타임시스템";

        private Observer.Observer_Action midnightObs = new Observer.Observer_Action();
        protected DateTimeTick playTimeTick;
        private Dictionary<int, Observer.Observer_Action<DateTime>> timerObsDic;

        public virtual DateTime Now
        {
            get
            {
                DateTime _nowTime = DateTime.Now + DateTimeOffset.Now.Offset;
#if UNITY_EDITOR
                _nowTime = this.GetOffsetTime_Func(_nowTime);
#endif
                return _nowTime;
            }
        }

        /// <summary>
        /// 앱을 키고나서 현재까지의 플레이 타임
        /// </summary>
        public DateTime GetPlayTime
        {
            get
            {
                long _ticks = this.Now.Ticks - this.playTimeTick.tick;
                if (_ticks < 0L)
                    _ticks = 0L;

                return new DateTime(_ticks);
            }
        }

        public TimeSystem_Manager()
        {
            this.timerObsDic = new Dictionary<int, Observer.Observer_Action<DateTime>>();

            Coroutine_C.StartCoroutine_Func(OnMidnight_Cor());

            Coroutine_C.StartCoroutine_Func(OnTimer_Cor());

            this.playTimeTick = this.Now;
        }

        public void ResetPlayTime_Func()
        {
            this.playTimeTick = this.Now;
            Debug.Log("ResetPlayTime_Func) this.playTimeTick : " + this.playTimeTick.GetTime);
        }
        private IEnumerator OnMidnight_Cor()
        {
            DateTime _midnightTime = this.GetMidNight_Func();

            while (true)
            {
                if (_midnightTime < this.Now)
                {
                    _midnightTime = this.GetMidNight_Func();

                    this.midnightObs.Notify_Func();
                }

                yield return Coroutine_C.GetWaitForSeconds_Cor(.5f, true);
            }
        }
        private IEnumerator OnTimer_Cor(int _milSec = 500)
        {
            float _interval = _milSec * 0.001f;

            if (this.timerObsDic.TryGetValue(_milSec, out Observer.Observer_Action<DateTime> _obs) == false)
            {
                _obs = new Observer.Observer_Action<DateTime>();
                this.timerObsDic.Add(_milSec, _obs);
            }

            while (true)
            {
                _obs.Notify_Func(this.Now);

                yield return Coroutine_C.GetWaitForSeconds_Cor(_interval, true);
            }
        }

        public DateTime GetMidNight_Func()
        {
            return this.Now.GetMidNight_Func();
        }

        /// <summary>
        /// 자정까지 남은 시간
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetRemainMidNight_Func()
        {
            DateTime _now = this.Now;
            DateTime _midnight = _now.GetMidNight_Func();
            return _midnight - _now;
        }
        /// <summary>
        /// 특정 요일까지 남은 시간
        /// </summary>
        /// <param name="_dayOfWeek">특정 요일</param>
        /// <returns></returns>
        public DateTime GetNextDayOfWeek_Func(DayOfWeek _dayOfWeek = DayOfWeek.Monday)
        {
            return this.Now.GetNextDayOfWeek_Func(_dayOfWeek);
        }
        /// <summary>
        /// 익월 1일까지 남은 시간
        /// </summary>
        /// <returns></returns>
        public DateTime GetNextBeginningOfMonth_Func()
        {
            return this.Now.GetNextBeginningOfMonth_Func();
        }

        /// <summary>
        /// 지정한 날짜의 DateTime
        /// </summary>
        /// <param name="_days">며칠 뒤?</param>
        /// <param name="_isMidnight">자정 시간으로?</param>
        /// <returns></returns>
        public DateTime GetAfterDay_Func(int _days, bool _isMidnight = false)
        {
            return this.Now.GetAfterDay_Func(_days, _isMidnight);
        }
        public DateTime GetOffsetTime_Func(DateTime _dateTime)
        {
#if UNITY_EDITOR && Test_Cargold
            if(DataBase_Manager.Instance != null)
            {
                _dateTime = _dateTime.AddDays(DataBase_Manager.Instance.GetDefine_C.testAddDay);
                _dateTime = _dateTime.AddHours(DataBase_Manager.Instance.GetDefine_C.testAddHour);
                _dateTime = _dateTime.AddMinutes(DataBase_Manager.Instance.GetDefine_C.testAddMin);
                _dateTime = _dateTime.AddSeconds(DataBase_Manager.Instance.GetDefine_C.testAddSec);
            }
#endif

            return _dateTime;
        }

        public void Subscribe_Midnight_Func(Action _del)
        {
            midnightObs.Subscribe_Func(_del);
        }
        public bool TrySubscribe_Midnight_Func(Action _del)
        {
            if (midnightObs.IsSubscribed_Func(_del) == false)
            {
                midnightObs.Subscribe_Func(_del);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Unsubscribe_Midnight_Func(Action _del)
        {
            midnightObs.Unsubscribe_Func(_del);
        }

        public bool IsSubscribe_OnTimer_Func(Action<DateTime> _del, int _milSec = 500)
        {
            if (this.timerObsDic.TryGetValue(_milSec, out Observer.Observer_Action<DateTime> _obs) == true)
                return _obs.IsSubscribed_Func(_del);

            return false;
        }
        public void Subscribe_OnTimer_Func(Action<DateTime> _del, bool _isCallback = true, int _milSec = 500)
        {
            if (this.timerObsDic.TryGetValue(_milSec, out Observer.Observer_Action<DateTime> _obs) == false)
            {
                _obs = new Observer.Observer_Action<DateTime>();
                this.timerObsDic.Add(_milSec, _obs);

                Coroutine_C.StartCoroutine_Func(OnTimer_Cor(_milSec));
            }

            _obs.Subscribe_Func(_del);

            if (_isCallback == true)
                _del(this.Now);
        }
        public void Unsubscribe_OnTimer_Func(Action<DateTime> _del, int _milSec = 500)
        {
            if (this.timerObsDic.TryGetValue(_milSec, out Observer.Observer_Action<DateTime> _obs) == true)
                _obs.Unsubscribe_Func(_del);
        }
    }
}