using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold.Observer;
using System;

namespace Cargold.FrameWork
{
    using Cargold.Infinite;
    using Cargold.Remocon;

    public abstract class UserSystem_Manager : SerializedMonoBehaviour, GameSystem_Manager.IInitializer // C
    {
        public static UserSystem_Manager Instance;

        [SerializeField] private IUserDataContainer iUserDataContainer;

        public abstract Common_C GetCommon { get; }
        public abstract Log_C GetLog { get; }
        public abstract UserData_C GetUserData { get; }
        public IUserDataContainer GetiUserDataContainer
        {
            get
            {
                if (this.iUserDataContainer == null)
                    this.iUserDataContainer = ProjectRemocon.Instance;

                return this.iUserDataContainer;
            }
        }

        private bool IsError_Func()
        {
            if (this.GetiUserDataContainer == null || this.GetiUserDataContainer.IsUserDataEmpty_Func() == true)
                return true;

            return false;
        }

        public virtual void Init_Func(int _layer)
        {
            if (_layer == 0)
            {
                Instance = this;
            }
            else if (_layer == 1)
            {
                // 기존 유저 데이터 불러오기
                if (SaveSystem.SaveSystem_Manager.Instance.TryGetUserData_Func(out string _userDataStr) == true)
                {
                    this.OnLoadUserDataStr_Func(_userDataStr);
                }

                // 신규 유저 데이터 세팅하기
                else
                {
                    UserData_C _userDataC = null;
                    if (this.IsError_Func() == false)
                        _userDataC = this.GetiUserDataContainer.GetUserData_Func();
                    else
                        _userDataC = new UserData_C();

#if !Test_Cargold
                    _CallOfficial_Func(_userDataC);
#endif

                    this.OnLoadDefaultUserData_Func(_userDataC);

                    void _CallOfficial_Func(UserData_C _userData)
                    {
                        _userData.langTypeID = (int)Application.systemLanguage;
                    }
                }
            }

            this.GetCommon.Init_Func(_layer);
        }
        protected abstract void OnLoadDefaultUserData_Func(UserData_C _userData);
        protected abstract void OnLoadUserDataStr_Func(string _userDataStr);
        protected virtual void SetUserData_Func(UserData_C _userData)
        {
            _userData.version = ProjectRemocon.Instance.buildSystem.GetVersion_Func();
        }

        #region Common
        [System.Serializable]
        public class Common_C
        {
            private UserData_C GetData => Instance.GetUserData;

            protected Observer_Action<SystemLanguage> langChangedObs;
            protected Observer_Action<bool> bgmChnagedObs;
            protected Observer_Action<bool> sfxChnagedObs;
            private Observer_Action dayPassedObs;

            public virtual void Init_Func(int _layer)
            {
                if(_layer == 0)
                {
                    this.langChangedObs = new Observer_Action<SystemLanguage>();
                    this.bgmChnagedObs = new Observer_Action<bool>();
                    this.sfxChnagedObs = new Observer_Action<bool>();
                    this.dayPassedObs = new Observer_Action();
                }
                else if(_layer == 2)
                {
                    this.SetLastOffTime_Func(false, true);

                    TimeSystem_Manager.Instance.Subscribe_OnTimer_Func(this.CallDel_OnTimeChanged_Func, false, 10000);
                    TimeSystem_Manager.Instance.Subscribe_Midnight_Func(this.CallDel_OnMidnight_Func);

                    if(Cargold.SaveSystem.SaveSystem_Manager.Instance != null)
                        Cargold.SaveSystem.SaveSystem_Manager.Instance.Subscribe_OnQuit_Func(this.CallDel_OnAppQuit_Func);
                }
            }

            public virtual SystemLanguage GetLanguageType_Func()
            {
                return (SystemLanguage)Instance.GetUserData.langTypeID;
            }
            public virtual bool GetBgm_Func() => Instance.GetUserData.isBgmOn;
            public virtual bool GetSfx_Func() => Instance.GetUserData.isSfxOn;
            public DateTime GetOfflineTime_Func()
            {
                return this.GetData.lastOffTime;
            }
            public DateTime GetOfflinePassedTime_Func(DateTime _nowTime = default)
            {
                if (_nowTime == default)
                    _nowTime = TimeSystem_Manager.Instance.Now;

                return _nowTime - this.GetData.lastOffTime;
            }

            public virtual void SetBgm_Func(bool _isOn, bool _isNotify = true, bool _isSave = true)
            {
                Instance.GetUserData.isBgmOn = _isOn;

                if (_isNotify == true)
                    this.bgmChnagedObs.Notify_Func(_isOn);

                if (_isSave == true)
                    SaveSystem.SaveSystem_Manager.Instance.Save_Func();
            }
            public virtual void SetSfx_Func(bool _isOn, bool _isNotify = true, bool _isSave = true)
            {
                Instance.GetUserData.isSfxOn = _isOn;

                if (_isNotify == true)
                    this.sfxChnagedObs.Notify_Func(_isOn);

                if(_isSave == true)
                    SaveSystem.SaveSystem_Manager.Instance.Save_Func();
            }
            public void SetLanguage_Func(SystemLanguage _langType, bool _isNotify = true, bool _isSave = true)
            {
                this.SaveLanguage_Func(_langType);

                if(_isNotify == true)
                    this.langChangedObs.Notify_Func(_langType);

                if(_isSave == true)
                    SaveSystem.SaveSystem_Manager.Instance.Save_Func();
            }
            protected virtual void SaveLanguage_Func(SystemLanguage _langType, bool _isSave = true)
            {
                Instance.GetUserData.langTypeID = (int)_langType;

                if (_isSave == true)
                    SaveSystem.SaveSystem_Manager.Instance.Save_Func();
            }
            protected void SetLastOffTime_Func(bool _isSet = true, bool _isSave = true)
            {
                DateTimeTick _lastOffTime = this.GetData.lastOffTime;
                DateTime _nowTime = TimeSystem_Manager.Instance.Now;
                if (Cargold_Library.IsPassedDay_Func(_lastOffTime, _nowTime) == true)
                {
                    this.dayPassedObs.Notify_Func();

                    this.SetDayPassed_Func();
                }

                if(_isSet == true)
                    this.GetData.lastOffTime = _nowTime;

                if (_isSave == true)
                    SaveSystem.SaveSystem_Manager.Instance.Save_Func();
            }
            protected virtual void SetDayPassed_Func() { }

            public void Subscribe_LangTypeChanged_Func(Action<SystemLanguage> _del, bool _isNotify = true)
            {
                this.langChangedObs.Subscribe_Func(_del);

                if(_isNotify == true)
                {
                    SystemLanguage _langType = this.GetLanguageType_Func();
                    _del(_langType);
                }
            }
            public void Unsubscribe_LangTypeChanged_Func(Action<SystemLanguage> _del)
            {
                this.langChangedObs.Unsubscribe_Func(_del);
            }

            public void Subscribe_Bgm_Func(Action<bool> _del, bool _isCallback = true)
            {
                this.bgmChnagedObs.Subscribe_Func(_del);

                if(_isCallback == true)
                    _del(Instance.GetUserData.isBgmOn);
            }
            public void Unscribe_Bgm_Func(Action<bool> _del)
            {
                this.bgmChnagedObs.Unsubscribe_Func(_del);
            }

            public void Subscribe_Sfx_Func(Action<bool> _del, bool _isCallback = true)
            {
                this.sfxChnagedObs.Subscribe_Func(_del);

                if (_isCallback == true)
                    _del(Instance.GetUserData.isSfxOn);
            }
            public void Unscribe_Sfx_Func(Action<bool> _del)
            {
                this.sfxChnagedObs.Unsubscribe_Func(_del);
            }

            public void Subscribe_DayPassed_Func(Action _del)
            {
                this.dayPassedObs.Subscribe_Func(_del);
            }
            public void Unsubscribe_DayPassed_Func(Action _del)
            {
                this.dayPassedObs.Unsubscribe_Func(_del);
            }

            private void CallDel_OnTimeChanged_Func(DateTime _now)
            {
                this.SetLastOffTime_Func();
            }
            private void CallDel_OnMidnight_Func()
            {
                this.SetLastOffTime_Func();
            }
            private void CallDel_OnAppQuit_Func()
            {
                this.SetLastOffTime_Func();
            }
        }
        #endregion
        #region Wealth
        [System.Serializable]
        public abstract class Wealth_C<WealthType, QuantityType> where WealthType : struct, IConvertible
        {
            protected Dictionary<WealthType, IWealthData> iWealthDataDic;
            protected Cargold.Observer.Observer_Action<WealthType, QuantityType> quantityChangedObs;

            public virtual void Init_Func(int _layer)
            {
                if(_layer == 0)
                {
                    iWealthDataDic = new Dictionary<WealthType, IWealthData>(EnumCompare.EnumCompare<WealthType>.Instance);
                    this.quantityChangedObs = new Observer_Action<WealthType, QuantityType>();
                }
            }
            public void SetData_Func(IEnumerable<IWealthData> _iWealthDataEnumer)
            {
                foreach (IWealthData _iWealthData in _iWealthDataEnumer)
                {
                    WealthType _wealthType = _iWealthData.GetWealthType;
                    this.iWealthDataDic.Add(_wealthType, _iWealthData);
                }
            }
            public void SetData_Func(WealthType _wealthType, QuantityType _quantity)
            {
                if (this.iWealthDataDic.TryGetValue(_wealthType, out IWealthData _iWealthData) == false)
                {
                    _iWealthData = this.GenerateUserWealthData_Func(_wealthType, _quantity);
                    this.iWealthDataDic.Add(_wealthType, _iWealthData);
                }
                else
                {
                    _iWealthData.SetQuantity_Func(_quantity);
                }
            }

            public virtual bool TryGetWealthControl_Func(WealthControl _wealthControl, WealthType _wealthType, QuantityType _quantity
                , bool _isJustCheck = false, bool _isNotify = true, bool _isSave = true)
            {
                if (_wealthControl == WealthControl.Earn)
                {
                    if (this.iWealthDataDic.TryGetValue(_wealthType, out IWealthData _iWealthData) == false)
                    {
                        _iWealthData = this.GenerateUserWealthData_Func(_wealthType, _quantity);
                        this.iWealthDataDic.Add(_wealthType, _iWealthData);
                    }
                    else
                    {
                        _iWealthData.AddQuantity_Func(_quantity);
                    }

                    if(_isJustCheck == false)
                    {
                        if (_isNotify == true)
                        {
                            QuantityType _addedQuantity = _iWealthData.GetQuantity;
                            this.quantityChangedObs.Notify_Func(_wealthType, _addedQuantity);
                        }

                        if (_isSave == true)
                            Cargold.SaveSystem.SaveSystem_Manager.Instance.Save_Func();
                    }

                    return true;
                }
                else if (_wealthControl == WealthControl.Spend)
                {
                    if (this.iWealthDataDic.TryGetValue(_wealthType, out IWealthData _iWealthData) == true)
                    {
                        bool _isSubtract = _iWealthData.TryGetSubtract_Func(_quantity, _isJustCheck);

                        if(_isJustCheck == false)
                        {
                            if (_isNotify == true)
                            {
                                QuantityType _addedQuantity = _iWealthData.GetQuantity;
                                this.quantityChangedObs.Notify_Func(_wealthType, _addedQuantity);
                            }

                            if (_isSave == true)
                                Cargold.SaveSystem.SaveSystem_Manager.Instance.Save_Func();
                        }

                        return _isSubtract;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Debug_C.Error_Func("_wealthControl : " + _wealthControl);
                }

                return false;
            }
            public QuantityType GetQuantity_Func(WealthType _wealthType)
            {
                if(this.iWealthDataDic.TryGetValue(_wealthType, out IWealthData _iWealthData) == false)
                {
                    _iWealthData = this.GenerateUserWealthData_Func(_wealthType, default);
                    this.iWealthDataDic.Add(_wealthType, _iWealthData);
                }

                return _iWealthData.GetQuantity;
            }
            public void Notify_WealthQuantityChanged_Func(WealthType _wealthType)
            {
                QuantityType _quantity = default;

                if (this.iWealthDataDic.TryGetValue(_wealthType, out IWealthData _iWealthData) == false)
                {
                    _iWealthData = this.GenerateUserWealthData_Func(_wealthType, default);
                    this.iWealthDataDic.Add(_wealthType, _iWealthData);
                }
                else
                {
                    _quantity = _iWealthData.GetQuantity;
                }
                
                this.quantityChangedObs.Notify_Func(_wealthType, _quantity);
            }

            public virtual void SetQuantity_Func(WealthType _wealthType, QuantityType _quantity, bool _isNotify = true, bool _isSave = true)
            {
                if (this.iWealthDataDic.TryGetValue(_wealthType, out IWealthData _iWealthData) == true)
                {
                    _iWealthData.SetQuantity_Func(_quantity);

                    if (_isNotify == true)
                    {
                        QuantityType _addedQuantity = _iWealthData.GetQuantity;
                        this.quantityChangedObs.Notify_Func(_wealthType, _addedQuantity);
                    }

                    if (_isSave == true)
                        Cargold.SaveSystem.SaveSystem_Manager.Instance.Save_Func();
                }
            }

            protected abstract IWealthData GenerateUserWealthData_Func(WealthType _wealthType, QuantityType _quantity);

            public void Subscribe_WealthQuantityChanged_Func(Action<WealthType, QuantityType> _del)
            {
                this.quantityChangedObs.Subscribe_Func(_del);
            }
            public void Subscribe_WealthQuantityChanged_Func(Action<WealthType, QuantityType> _del, WealthType _callBackType)
            {
                this.quantityChangedObs.Subscribe_Func(_del);

                if(this.iWealthDataDic.TryGetValue(_callBackType, out IWealthData _iWealthData) == true)
                {
                    _del(_callBackType, _iWealthData.GetQuantity);
                }
                else
                {
                    _del(_callBackType, default);
                }

            }
            public void Unsubscribe_WealthQuantityChanged_Func(Action<WealthType, QuantityType> _del)
            {
                this.quantityChangedObs.Unsubscribe_Func(_del);
            }

            public interface IWealthData
            {
                WealthType GetWealthType { get; }
                QuantityType GetQuantity { get; }

                void AddQuantity_Func(QuantityType _quantity);
                void SetQuantity_Func(QuantityType _quantity);
                bool TryGetSubtract_Func(QuantityType _quantity, bool _isJustCheck = false);
            }
        }
        #endregion
        #region Log
        [System.Serializable]
        public abstract class Log_C
        {
            public int GetPlayTime_Func()
            {
                return Instance.GetUserData.playtime;
            }
            public void SetAddPlayTime_Func(int _secTime)
            {
                Instance.GetUserData.playtime += _secTime;
            }
        }
        #endregion

#if UNITY_EDITOR
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
        }
#endif

        public interface IUserDataContainer
        {
            bool IsUserDataEmpty_Func();
            UserData_C GetUserData_Func();
        }

        public enum WealthControl
        {
            None = 0,

            Spend,
            Earn,
        }
    }
}