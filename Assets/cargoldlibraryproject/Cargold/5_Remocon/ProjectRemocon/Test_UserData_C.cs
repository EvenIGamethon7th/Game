using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;

namespace Cargold.Remocon
{
    [System.Serializable]
    public abstract class Test_UserData_C : SerializedMonoBehaviour
    {
        [LabelText("메모"), Multiline, SerializeField] private string uid;
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr), FoldoutGroup(CargoldLibrary_C.GetLibraryKorStrS + "Test"), LabelText("언어"), EnumPaging, SerializeField]
        private SystemLanguage langType = SystemLanguage.Korean;

        public UserData_C GetUserData_Func()
        {
            UserData_C _userDataC = this.GetUserOverrideData_Func();

            _userDataC.lastOffTime = TimeSystem_Manager.Instance.Now;

#if Test_Cargold
            _CallTest_Func();
#endif

            return _userDataC;

            void _CallTest_Func()
            {
                _userDataC.langTypeID = (int)this.langType;
            }
        }

        protected abstract UserData_C GetUserOverrideData_Func();

#if UNITY_EDITOR
        [Button("유저 데이터 초기화"), GUIColor("CallEdit_SaveKey_Func"), PropertyOrder(-1)]
        public void ResetUserData_Func()
        {
            SaveSystem.SaveSystem_Manager.CallEdit_RemoveSave_Func(); 
        }

        private Color CallEdit_SaveKey_Func()
        {
            if(SaveSystem.SaveSystem_Manager.IsHaveSaveData_Func() == true)
            {
                return Color.yellow;
            }
            else
            {
                return Color.white;
            }
        }
#endif
    } 
}