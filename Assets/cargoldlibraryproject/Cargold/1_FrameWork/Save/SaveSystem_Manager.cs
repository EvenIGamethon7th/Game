using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Cargold.SaveSystem
{
    using Cargold.Observer;
    using Cargold.ReserveSystem;
    using DataBase_Manager = FrameWork.DataBase_Manager;

    public abstract class SaveSystem_Manager : MonoBehaviour, FrameWork.GameSystem_Manager.IInitializer
    {
        private const string LegacySaveKey = "!C@a#r$g%o^l&d*";
        public const string Str = "세이브 시스템";

        public static SaveSystem_Manager Instance;

        private ReserveSystem reserveSystem;
        private Observer_Action quitObs;

        protected virtual float GetSaveInterval
        {
            get
            {
                if (DataBase_Manager.Instance is null == false)
                    return DataBase_Manager.Instance.GetDefine_C.saveInterval;

                return 3f;
            }
        }
        protected virtual object GetUserData
        {
            get
            {
                if (FrameWork.UserSystem_Manager.Instance is null == false)
                {
                    return FrameWork.UserSystem_Manager.Instance.GetUserData;
                }
                else
                {
                    Debug_C.Error_Func("?");

                    return null;
                }
            }
        }

        /// <summary>
        /// 저장 및 불러오기 시 사용되는 Key. 기본적으로는 프로젝트 리모콘에 연결되 유저 데이터 프리팹의 객체 이름을 사용한다.
        /// </summary>
        protected virtual string GetSaveKey => Remocon.ProjectRemocon.Instance.userData.gameObject.name;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                Instance = this;

                this.reserveSystem = new ReserveSystem(this.GetSaveInterval, CallDel_OnReserveDone_Func, this);

                this.reserveSystem.Activate_Func();

                this.quitObs = new Observer_Action();
            }
        }

        public void Save_Func(bool _isImmediately = false)
        {
            Debug_C.Log_Func("Save_Func) _isImmediately : " + _isImmediately, Debug_C.PrintLogType.Save);

            this.reserveSystem.OnReserve_Func(_isImmediately);
        }

        public bool TryGetUserData_Func(out string _userDataStr)
        {
            if(DataBase_Manager.Instance != null)
            {
                Debug_C.Log_Func("isSaveActivate : " + DataBase_Manager.Instance.GetDefine_C.isSaveActivate, Debug_C.PrintLogType.Save);
                if (DataBase_Manager.Instance.GetDefine_C.isSaveActivate == false)
                {
                    _userDataStr = default;
                    return false;
                }
            }

            string _userDataJson = null;

            if (PlayerPrefs.HasKey(LegacySaveKey) == false)
            {
                _userDataJson = PlayerPrefs.GetString(this.GetSaveKey);
            }
            else
            {
                _userDataJson = PlayerPrefs.GetString(LegacySaveKey);
                PlayerPrefs.DeleteKey(LegacySaveKey);
            }

            Debug_C.Log_Func("Load) : " + _userDataJson, Debug_C.PrintLogType.Save);
            if (string.IsNullOrWhiteSpace(_userDataJson) == false)
            {
                _userDataStr = _userDataJson;
                return true;
            }
            else
            {
                _userDataStr = default;
                return false;
            }
        }
        public void Subscribe_OnQuit_Func(System.Action _del)
        {
            this.quitObs.Subscribe_Func(_del);
        }
        public void Unsubscribe_OnQuit_Func(System.Action _del)
        {
            this.quitObs.Unsubscribe_Func(_del);
        }
        protected void OnSave_Func()
        {
            if (DataBase_Manager.Instance is null == false && DataBase_Manager.Instance.GetDefine_C.isSaveActivate == false)
                return;

            string _userDataJson = this.GetUserDataToString_Func();
            this.OnSaveDone_Func(_userDataJson);

            if(Debug_C.IsLogType_Func(Debug_C.PrintLogType.Save) == true)
                Debug_C.Log_Func("Save) : " + _userDataJson, Debug_C.PrintLogType.Save);
        }

        /// <summary>
        /// object 타입의 유저 데이터를 직렬화하는 함수
        /// </summary>
        /// <returns></returns>
        protected virtual string GetUserDataToString_Func()
        {
            string _userDataJson = null;
            object _userData = this.GetUserData;
#if UNITY_2020_1_OR_NEWER
            _userDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(_userData, new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
#else
            _userDataJson = JsonUtility.ToJson(_userData);
#endif

            return _userDataJson;
        }

        /// <summary>
        /// 최종적으로 저장을 어떻게 할지 정의하는 함수
        /// </summary>
        /// <param name="_userDataJson"></param>
        protected virtual void OnSaveDone_Func(string _userDataJson)
        {
            PlayerPrefs.SetString(this.GetSaveKey, _userDataJson);
        }

        private void CallDel_OnReserveDone_Func()
        {
            this.OnSave_Func();
        }

        public static bool IsHaveSaveData_Func()
        {
            Remocon.Test_UserData_C _testUserData = Remocon.ProjectRemocon.Instance.userData;
            if (_testUserData != null)
            {
                string _name = _testUserData.gameObject.name;
                return PlayerPrefs.HasKey(_name);
            }
            else
            {
                return false;
            }
        }

        void OnApplicationQuit()
        {
            if(this.quitObs != null)
                this.quitObs.Notify_Func();

            this.Save_Func(true);
        }
        void OnApplicationPause()
        {
            if(this.quitObs != null)
                this.quitObs.Notify_Func();

            this.Save_Func(true);
        }

#if UNITY_EDITOR
        public static void CallEdit_RemoveSave_Func()
        {
            Debug.Log("유저 데이터 초기화를 시도합니다.");

            string _name = Remocon.ProjectRemocon.Instance.userData.gameObject.name;
            if (PlayerPrefs.HasKey(_name) == false)
            {
                Debug.Log("유저 데이터가 없으므로 초기화도 안 함 ㅅㄱ");
            }
            else
            {
                PlayerPrefs.DeleteKey(_name);

                Debug.Log("유저 데이터 초기화를 완료했습니다.");
            }
        }
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
        }
#endif
    }
}