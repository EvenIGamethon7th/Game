using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using TMPro;
using Cargold.Example;

namespace Cargold.Dialogue
{
    public abstract class UI_Dialogue_Script : Cargold.UI.UI_Script, Cargold.Example.IPropertyAdapter
    {
        [SerializeField, HideIf("@propertyAdapterClass != null")] private PropertyAdapter_UI_Dialogue_Script propertyAdapterClass;

        [SerializeField, BoxGroup("화자"), LabelText("데이터")] private Dictionary<string, Dialogue_Speaker> speakerClassDic = new Dictionary<string, Dialogue_Speaker>();
        [SerializeField, LabelText("출력 방식")] private DescPrintType descPrintType = DescPrintType.Sequential;
        protected CoroutineData descCorData;
        private bool isDescSkip;
        [ShowInInspector, FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), ReadOnly] private bool isAniStart;
        [ShowInInspector, FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), ReadOnly] private DialogueSystem_Manager dialogueSystemClass;
        [ShowInInspector, FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), LabelText("현재 화자"), ReadOnly] protected Dialogue_Speaker currentSpeakerClass;
        private string callbackKey;
        private IDialogueData iDialogueData;

        [ShowInInspector] protected TextMeshProUGUI nameTmp { get => propertyAdapterClass.nameTmp; set => propertyAdapterClass.nameTmp = value; }
        [ShowInInspector] protected TextMeshProUGUI descTmp { get => propertyAdapterClass.descTmp; set => propertyAdapterClass.descTmp = value; }
        [ShowInInspector] protected RectTransform btnGroupRtrf { get => propertyAdapterClass.btnGroupRtrf; set => propertyAdapterClass.btnGroupRtrf = value; }
        [ShowInInspector] protected UI_Dialogue_Button[] btnClassArr { get => propertyAdapterClass.btnClassArr; set => propertyAdapterClass.btnClassArr = value; }
        [ShowInInspector] protected GameObject mainBtnObj { get => propertyAdapterClass.mainBtnObj; set => propertyAdapterClass.mainBtnObj = value; }
        [ShowInInspector] protected GameObject dimObj { get => propertyAdapterClass.dimObj; set => propertyAdapterClass.dimObj = value; }
        [ShowInInspector] protected GameObject contentGroupObj { get => propertyAdapterClass.contentGroupObj; set => propertyAdapterClass.contentGroupObj = value; }
        [ShowInInspector] protected AnimationClip startAniClip { get => propertyAdapterClass.startAniClip; set => propertyAdapterClass.startAniClip = value; }
        [ShowInInspector] protected AnimationClip newSpeakerAniClip { get => propertyAdapterClass.newSpeakerAniClip; set => propertyAdapterClass.newSpeakerAniClip = value; }
        [ShowInInspector] protected AnimationClip repeatAniClip { get => propertyAdapterClass.repeatAniClip; set => propertyAdapterClass.repeatAniClip = value; }
        [ShowInInspector] protected AnimationClip doneAniClip { get => propertyAdapterClass.doneAniClip; set => propertyAdapterClass.doneAniClip = value; }

        protected virtual float GetDescSequentialInterval => .01f;

        public override void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                foreach (UI_Dialogue_Button _btnClass in this.btnClassArr)
                    _btnClass.Init_Func(this);

                foreach (var item in this.speakerClassDic)
                    item.Value.Init_Func();
            }

            base.Init_Func(_layer);
        }

        public virtual void SetData_Func(DialogueSystem_Manager _dialogueSystemClass)
        {
            this.dialogueSystemClass = _dialogueSystemClass;
        }

        public virtual void Activate_Func(IDialogueData _iDialogueData)
        {
            this.iDialogueData = _iDialogueData;

            this.Activate_Func();
        }

        protected override void ActivateDone_Func()
        {
            base.ActivateDone_Func();

            this.contentGroupObj.SetActive(true);

            this.SetDial_Func(this.iDialogueData);
        }

        public virtual void SetDial_Func(IDialogueData _iDialogueData)
        {
            // 이름
            this.SetName_Func(_iDialogueData);

            // 대사
            this.SetDesc_Func(_iDialogueData);

            // 화자
            this.SetSpeaker_Func(_iDialogueData);

            // 딤드
            this.SetDim_Func(_iDialogueData);

            this.callbackKey = null;

            // 버튼
            if (_iDialogueData.IsEmpty_Func() == true)
            {
                this.mainBtnObj.SetActive(true);

                foreach (UI_Dialogue_Button _btnClass in this.btnClassArr)
                    _btnClass.Deactivate_Func();
            }
            else
            {
                this.mainBtnObj.SetActive(false);

                if (_iDialogueData.GetBtnDataArr[0].GetLczKey.IsNullOrWhiteSpace_Func() == false)
                {
                    int _btnNum = 0;

                    for (int i = 0; i < this.btnClassArr.Length; i++)
                    {
                        UI_Dialogue_Button _btnClass = this.btnClassArr[i];
                        if (_iDialogueData.GetBtnDataArr.TryGetItem_Func(i, out Dialogue.DialogueButtonData _btnData) == true)
                        {
                            _btnNum++;
                            _btnClass.Activate_Func(_btnData);
                        }
                        else
                        {
                            _btnClass.Deactivate_Func();
                        }
                    }

                    this.btnGroupRtrf.sizeDelta = new Vector2(this.btnGroupRtrf.sizeDelta.x, _btnNum * 150f);
                }
                else
                {
                    foreach (UI_Dialogue_Button _btnClass in this.btnClassArr)
                        _btnClass.Deactivate_Func();

                    this.callbackKey = _iDialogueData.GetBtnDataArr[0].GetCallbackKey;
                }
            }
        }

        protected virtual void SetSpeaker_Func(IDialogueData _iDialogueData)
        {
            string _speakerKey = _iDialogueData.GetSpeakerType;
            this.SetSpeaker_Func(_iDialogueData, _speakerKey);
        }
        protected virtual void SetSpeaker_Func(IDialogueData _iDialogueData, string _speakerKey)
        {
            foreach (var item in this.speakerClassDic)
                item.Value.Deactivate_Func();

            AnimationClip _aniClip = null;

            if (this.speakerClassDic.TryGetValue(_speakerKey, out Dialogue_Speaker _speakerClass) == true)
            {
                string _aniType = _iDialogueData.GetSpeckerAniTrigger;

                _speakerClass.Activate_Func(_aniType);

                if (this.currentSpeakerClass == null)
                {
                    _aniClip = this.startAniClip;
                }
                else if (this.currentSpeakerClass != _speakerClass)
                {
                    _aniClip = this.newSpeakerAniClip;

                    this.OnSpeakerChange_Func(_speakerKey);
                }
                else
                {
                    _aniClip = this.repeatAniClip;
                }

                this.currentSpeakerClass = _speakerClass;
            }
            else
            {
                if(_speakerKey.IsNullOrWhiteSpace_Func() == false)
                    Debug_C.Warning_Func("다음 Key의 화자는 존재하지 않습니다. : " + _speakerKey);
            }

            if (_aniClip != null)
                this.anim.Play_Func(_aniClip);
        }
        protected virtual void OnSpeakerChange_Func(string _speakerKey)
        {

        }

        protected virtual void SetName_Func(IDialogueData _iDialogueData)
        {
            this.SetName_Func(_iDialogueData.GetNameLczKey);
        }
        protected virtual void SetName_Func(string _lczKey)
        {
            this.nameTmp.SetLcz_Func(_lczKey);
        }
        protected virtual void SetDesc_Func(IDialogueData _iDialogueData)
        {
            this.SetDesc_Func(_iDialogueData.GetDescLczKey);
        }
        protected virtual void SetDesc_Func(string _lczKey)
        {
            if(this.descPrintType == DescPrintType.Immediately)
            {
                this.descTmp.SetLcz_Func(_lczKey);

                this.OnDescDone_Func();
            }
            else if(this.descPrintType == DescPrintType.Sequential)
            {
                this.descCorData.StartCoroutine_Func(SetDesc_Sequential_Cor(_lczKey));
            }
            else
            {
                Debug_C.Error_Func("this.descPrintType : " + this.descPrintType);
            }

            
        }
        private IEnumerator SetDesc_Sequential_Cor(string _lczKey)
        {
            this.isDescSkip = false;
            string _lczStr = Cargold.FrameWork.LocalizeSystem_Manager.Instance.GetLcz_Func(_lczKey);
            float _time = this.GetDescSequentialInterval;
            if (_lczStr.IsNullOrWhiteSpace_Func() == false)
            {
                Color _tmpColor = this.descTmp.color;

                for (int i = 0; i <= _lczStr.Length; i++)
                {
                    if (this.isDescSkip == false)
                    {
                        string _showStr = _lczStr.Substring(0, i);
                        _showStr = RichText_C.SetColor_Func(_showStr, _tmpColor);
                        string _hideStr = _lczStr.Substring(i);
                        _hideStr = RichText_C.SetColor_Func(_hideStr, Color.clear);
                        this.descTmp.text = StringBuilder_C.Append_Func(_showStr, _hideStr);

                        yield return Coroutine_C.GetWaitForSeconds_Cor(_time);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            this.descTmp.text = _lczStr;

            this.descCorData.StopCorountine_Func();

            this.OnDescDone_Func();
        }
        protected virtual void OnDescDone_Func() { }
        
        protected virtual void SetDim_Func(IDialogueData _iDialogueData)
        {
            bool _isDimOff = _iDialogueData.IsDimOff;
            this.dimObj.SetActive(_isDimOff == false);
        }
        protected virtual void SetBg_Func()
        {

        }

        public void OnBtn_Func(int _id)
        {
            this.btnClassArr[_id].OnBtn_Func();
        }
        public virtual void OnBtn_Func(int _id, string _callbackKey, bool _isSkip = false)
        {
            if (this.anim != null && this.anim.isPlaying == true)
                return;

            switch (this.descPrintType)
            {
                case DescPrintType.Immediately:
                    break;

                case DescPrintType.Sequential:
                    {
                        if (_isSkip == false && this.descCorData.IsActivate == true)
                        {
                            this.isDescSkip = true;
                            return;
                        }
                    }
                    break;

                case DescPrintType.None:
                default:
                    Debug_C.Error_Func("this.descPrintType : " + this.descPrintType);
                    break;
            }

            this.dialogueSystemClass.OnAwnser_Func(_id, _callbackKey, _isSkip);
        }

        public override void Deactivate_Func()
        {
            foreach (UI_Dialogue_Button _btnClass in this.btnClassArr)
                _btnClass.OnHide_Func();

            base.Deactivate_Func();
        }
        public override void DeactivateDone_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {
                foreach (UI_Dialogue_Button _btnClass in this.btnClassArr)
                    _btnClass.Deactivate_Func();

                foreach (var item in this.speakerClassDic)
                    item.Value.Deactivate_Func();
            }

            this.currentSpeakerClass = null;
            this.isAniStart = false;
            this.iDialogueData = null;
            this.contentGroupObj.SetActive(false);

            base.DeactivateDone_Func(_isInit); // a1

            if (_isInit == false)
                this.OnDeactivateUI_Func(); // a2
        }

        protected virtual void OnDeactivateUI_Func()
        {
            this.dialogueSystemClass.OnDeactivateUI_Func();
        }

        private void CallAni_Func()
        {
            if(this.isAniStart == false)
            {
                this.isAniStart = true;
            }
            else
            {
                this.isAniStart = false;

                this.DeactivateDone_Func();
            }
        }

        public void CallBtn_Main_Func()
        {
            this.OnBtn_Func(-1, this.callbackKey);
        }
        public void CallBtn_DialSkip_Func()
        {
            this.OnBtn_Func(-1, this.callbackKey, true); 
        }

        [System.Serializable]
        public class BtnData
        {
            public TextMeshProUGUI btnTmp = null;

            public void Activate_Func(string _lczKey)
            {
                this.btnTmp.SetLcz_Func(_lczKey);
            }
        }

#if UNITY_EDITOR
        void IPropertyAdapter.CallEdit_AddComponent_Func<T>(T _exampleData)
        {
            this.propertyAdapterClass = _exampleData as PropertyAdapter_UI_Dialogue_Script;
        }
#endif
    }

    public enum DescPrintType
    {
        None = 0,

        Immediately,
        Sequential,
    }
}