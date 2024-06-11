using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.Remocon;
using Cargold.FrameWork;
namespace Cargold.Example {
public class 유저매니저 : Cargold.FrameWork.UserSystem_Manager
{
    public static new 유저매니저 Instance;

    [SerializeField] private 유저데이터 userData = null;

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
        유저데이터 _userData = null;
#if UNITY_2020_1_OR_NEWER
        _userData = Newtonsoft.Json.JsonConvert.DeserializeObject<유저데이터>(_userDataStr);
#else
        _userData = JsonUtility.FromJson<유저데이터>(_userDataStr);
#endif

        _userData.version = ProjectRemocon.Instance.buildSystem.GetVersion_Func();

        base.SetUserData_Func(_userData);

        this.userData = _userData;
    }

    protected override void OnLoadDefaultUserData_Func(UserData_C _userDataC)
    {
        유저데이터 _userData = _userDataC as 유저데이터;

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
    public class Wealth : Wealth_C<재화타입, 재화밸류>
    {
        public override void Init_Func(int _layer)
        {
            base.Init_Func(_layer);

            if(_layer == 1)
            {
                유저데이터 _userData = 유저매니저.Instance.GetUserData as 유저데이터;
                base.SetData_Func(_userData.userWealthDataList);
            }
        }

        protected override IWealthData GenerateUserWealthData_Func(재화타입 _itemType, 재화밸류 _quantity)
        {
            유저재화데이터 _userWealthData = new 유저재화데이터(_itemType, _quantity);

            Instance.userData.userWealthDataList.Add(_userWealthData);

            if (Cargold.SaveSystem.SaveSystem_Manager.Instance is null == false)
                Cargold.SaveSystem.SaveSystem_Manager.Instance.Save_Func();

            return _userWealthData;
        }
    } 
    #endregion
}
} // End