using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.Remocon;
using Cargold.FrameWork;

public class UserSystem_Manager : Cargold.FrameWork.UserSystem_Manager
{
    public static new UserSystem_Manager Instance;

    [SerializeField] private UserData userData = null;

    public Common common;
    public Log log;
    public Wealth wealth;

    public override Common_C GetCommon => this.common;
    public override Log_C GetLog => this.log;
    public override UserData_C GetUserData => this.userData;

    public override void Init_Func(int _layer)
    {
        base.Init_Func(_layer);

        if(_layer == 0)
        {
            Instance = this;
        }

        this.wealth.Init_Func(_layer);
    }

    protected override void OnLoadUserDataStr_Func(string _userDataStr)
    {
        UserData _userData = null;
#if UNITY_2020_1_OR_NEWER
        _userData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(_userDataStr);
#else
        _userData = JsonUtility.FromJson<UserData>(_userDataStr);
#endif

        _userData.version = ProjectRemocon.Instance.buildSystem.GetVersion_Func();

        base.SetUserData_Func(_userData);

        this.userData = _userData;
    }

    protected override void OnLoadDefaultUserData_Func(UserData_C _userDataC)
    {
        UserData _userData = _userDataC as UserData;

        base.SetUserData_Func(_userData);

        this.userData = _userData;
    }

    #region Common
    [System.Serializable]
    public class Common : Common_C
    {

    } 
    #endregion
    #region Log
    [System.Serializable]
    public class Log : Log_C
    {

    }
        #endregion
    #region Wealth
    [System.Serializable]
    public class Wealth : Wealth_C<WealthType, int>
    {
        public override void Init_Func(int _layer)
        {
            base.Init_Func(_layer);

            if(_layer == 1)
            {
                UserData _userData = UserSystem_Manager.Instance.GetUserData as UserData;
                base.SetData_Func(_userData.userWealthDataList);
            }
        }

        protected override IWealthData GenerateUserWealthData_Func(WealthType _itemType, int _quantity)
        {
            UserWealthData _userWealthData = new UserWealthData(_itemType, _quantity);

            Instance.userData.userWealthDataList.Add(_userWealthData);

            if (Cargold.SaveSystem.SaveSystem_Manager.Instance is null == false)
                Cargold.SaveSystem.SaveSystem_Manager.Instance.Save_Func();

            return _userWealthData;
        }
    } 
    #endregion
}
