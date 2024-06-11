using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Dialogue
{
    public class PropertyAdapter_UI_Dialogue_Script : Example.PropertyAdapter
    {
#if UNITY_EDITOR
        [SerializeField] private PropertyAdapter_UI_Dialogue_Button[] adapterBtnClassArr = null; 
#endif

        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("화자 이름 Tmp")] public TextMeshProUGUI nameTmp = null;
        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("대화 내용 Tmp")] public TextMeshProUGUI descTmp = null;
        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("버튼 Group Rtrf")] public RectTransform btnGroupRtrf = null;
        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("버튼")] public UI_Dialogue_Button[] btnClassArr = null;
        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("메인 버튼")] public GameObject mainBtnObj;
        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("딤")] public GameObject dimObj;
        [SerializeField, BoxGroup(CargoldLibrary_C.Mandatory), LabelText("콘텐츠 그룹")] public GameObject contentGroupObj;
        [SerializeField, FoldoutGroup(CargoldLibrary_C.Optional), LabelText("대화 시작")] public AnimationClip startAniClip = null;
        [SerializeField, FoldoutGroup(CargoldLibrary_C.Optional), LabelText("새로운 화자")] public AnimationClip newSpeakerAniClip = null;
        [SerializeField, FoldoutGroup(CargoldLibrary_C.Optional), LabelText("반복 화자")] public AnimationClip repeatAniClip = null;
        [SerializeField, FoldoutGroup(CargoldLibrary_C.Optional), LabelText("대화 종료")] public AnimationClip doneAniClip = null;

        public override string GetLibraryClassType => LibraryRemocon.UtilityClassData.DialogueData.Instance.GetUiClassNameStr;

#if UNITY_EDITOR
        public override bool CallEdit_TryAddComponent_Func<T>(out T _component)
        {
            bool _isAdd = base.CallEdit_TryAddComponent_Func(out UI_Dialogue_Script _uiDialClass);
            _component = _uiDialClass as T;

            bool _isNullHave = false;
            if (this.adapterBtnClassArr.IsHave_Func() == false)
            {
                _isNullHave = true;
            }
            else
            {
                foreach (PropertyAdapter_UI_Dialogue_Button _adapterBtnClass in this.adapterBtnClassArr)
                {
                    if (_adapterBtnClass == null)
                    {
                        _isNullHave = true;
                        break;
                    }
                }
            }


            if (_isNullHave == true)
            {
                UI_Dialogue_Button[] _uiDialBtnClassArr = this.gameObject.GetComponentsInChildren<UI_Dialogue_Button>();

                this.adapterBtnClassArr = new PropertyAdapter_UI_Dialogue_Button[_uiDialBtnClassArr.Length];

                for (int i = 0; i < _uiDialBtnClassArr.Length; i++)
                {
                    if (_uiDialBtnClassArr[i].TryGetComponent(out this.adapterBtnClassArr[i]) == false)
                    {
                        Debug_C.Error_Func("?");
                    }
                }
            }

            if (this.adapterBtnClassArr.IsHave_Func() == true)
            {
                PropertyAdapter_UI_Dialogue_Button[] _adapterBtnClassArr = this.adapterBtnClassArr;
                this.adapterBtnClassArr = null;

                this.btnClassArr = new UI_Dialogue_Button[_adapterBtnClassArr.Length];

                for (int i = 0; i < _adapterBtnClassArr.Length; i++)
                {
                    PropertyAdapter_UI_Dialogue_Button _btnClass = _adapterBtnClassArr[i];
                    if (_btnClass.CallEdit_TryAddComponent_Func(out UI_Dialogue_Button _uiBtnClass) == true)
                    {
                        this.btnClassArr[i] = _uiBtnClass;
                    }
                }
            }

            return _isAdd;
        }
#endif
    }
}