using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cargold.Remocon
{
    public partial class ProjectCheat // 카라리
    {
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), LabelText("타임 스케일"), OnValueChanged("CallEdit_TimeScale_Func"), CustomValueDrawer("CallEdit_TimeScaleRange_Func")]
        public float timeScale = 1f;

        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), HorizontalGroup(CargoldLibrary_C.GetLibraryKorStr + "/1"), LabelText("최소")]
        public float timeScaleMin = 0.01f;

        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), HorizontalGroup(CargoldLibrary_C.GetLibraryKorStr + "/1"), LabelText("최대")]
        public float timeScaleMax = 10f;

        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), LabelText("언어"), EnumPaging, OnValueChanged("CallEdit_LangTypeChanged_Func")]
        public SystemLanguage langType;

#if UNITY_EDITOR
        private float CallEdit_TimeScaleRange_Func(float _value, GUIContent _label)
        {
            return EditorGUILayout.Slider(_label, _value, this.timeScaleMin, this.timeScaleMax);
        } 

        private void CallEdit_TimeScale_Func()
        {
            Time.timeScale = this.timeScale;
            Debug_C.Log_Func("Time.timeScale : " + Time.timeScale);
        }

        private void CallEdit_LangTypeChanged_Func()
        {
            if(Application.isPlaying == true)
            {
                Cargold.FrameWork.UserSystem_Manager.Instance.GetCommon.SetLanguage_Func(this.langType);
            }
        }
#endif
    }
}