using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;
using Cargold.Observer;

namespace Cargold.Dialogue
{
    public abstract class DialogueSystem_Manager : SerializedMonoBehaviour, FrameWork.GameSystem_Manager.IInitializer
    {
        public static DialogueSystem_Manager Instance;

        [SerializeField] protected Cargold.Dialogue.UI_Dialogue_Script uiDialogueClass = null;
        [ShowInInspector, ReadOnly] private IDialogueData currentDialogueData;
        [ShowInInspector, ReadOnly] private Func<string, int, string, string, bool> onBtnSelectDel;
        [ShowInInspector, ReadOnly] private Action<string, string> doneDel;

        public bool IsActivate => this.uiDialogueClass.IsActivate;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                Instance = this;

                this.Deactivate_Func(true);
            }

            if(this.uiDialogueClass != null)
            {
                this.uiDialogueClass.Init_Func(_layer);

                if(_layer == 0)
                    this.uiDialogueClass.SetData_Func(this);
            }
        }

        public virtual void Activate_Func()
        {
            
        }

        /// <summary>
        /// 다이얼로그 시작
        /// </summary>
        /// <param name="_dataKey">재생할 다이얼로그 Data Key</param>
        /// <param name="_onBtnSelectDel">선택지 콜백.
        /// string : Dial Key, int : 선택지 ID, string : 선택지 콜백 Key, string : 선택지 연계 Dial Key, Return : 다음 Dial 출력 여부 </param>
        /// <param name="_doneDel">다이얼로그 그룹이 끝난 후 콜백 1. string : 끝난 그룹 Key, 2. string : 끝난 다이얼로그 Key</param>
        public void OnDialogue_Func(string _dataKey, Func<string, int, string, string, bool> _onBtnSelectDel = null, Action<string, string> _doneDel = null)
        {
            if(Cargold.FrameWork.DataBase_Manager.Instance.GetDialogue_C.TryGetData_Func(_dataKey, out IDialogueData _iDialogueData) == true)
            {
                this.OnDialogue_Func(_iDialogueData, _onBtnSelectDel, _doneDel);
            }
            else
            {
                Debug_C.Log_Func("다음 key의 다이얼로그 데이터는 없음 : " + _dataKey);
            }
        }
        public void OnDialogue_Func(string _dataKey, Action _doneDel)
        {
            this.OnDialogue_Func(_dataKey, null, (_a, _b) => _doneDel());
        }

        public virtual void OnDialogue_Func(IDialogueData _iDialogueData
            , Func<string, int, string, string, bool> _onBtnSelectDel = null
            , Action<string, string> _doneDel = null)
        {
            this.currentDialogueData = _iDialogueData;
            this.onBtnSelectDel = _onBtnSelectDel;
            this.doneDel = _doneDel;

            if (this.uiDialogueClass.IsActivate == false)
                this.uiDialogueClass.Activate_Func(_iDialogueData);
            else
                this.uiDialogueClass.SetDial_Func(_iDialogueData);
        }
        public void OnAwnser_Func(int _id, string _callbackKey, bool _isSkip = false)
        {
            if(_id == -1)
                Debug_C.Log_Func($"다음으로", Debug_C.PrintLogType.Dialogue);
            else
                Debug_C.Log_Func($"대답 ID : {_id}", Debug_C.PrintLogType.Dialogue);

            IDialogueData _nextIDialogueData = null;

            if (this.currentDialogueData.IsEmpty_Func() == false)
            {
                // 선택지에 따른 다음 다이얼로그
                if (this.currentDialogueData.GetBtnDataArr.TryGetItem_Func(_id, out DialogueButtonData _iBtnData) == true)
                {
                    string _dialogueDataKey = _iBtnData.GetNextDialogueDataKey;

                    // 다음 다이얼로그 데이터 Key가 있는가?
                    if (_dialogueDataKey.IsNullOrWhiteSpace_Func() == false)
                        Cargold.FrameWork.DataBase_Manager.Instance.GetDialogue_C.TryGetData_Func(_dialogueDataKey, out _nextIDialogueData);
                }
            }

            // 다음 다이얼로그 데이터가 없는가?
            if (_nextIDialogueData == null)
                // 같은 그룹의 다음 다이얼로그 데이터를 가져오기
                Cargold.FrameWork.DataBase_Manager.Instance.GetDialogue_C.TryGetNextData_Func(this.currentDialogueData, out _nextIDialogueData);

            if (this.onBtnSelectDel != null)
            {
                string _nextDialKey = null;
                if (_nextIDialogueData != null)
                    _nextDialKey = _nextIDialogueData.GetKey;

                string _currentDialKey = this.currentDialogueData.GetKey;
                if (this.onBtnSelectDel.Invoke(_currentDialKey, _id, _callbackKey, _nextDialKey) == false)
                {
                    return;
                }
            }

            Debug_C.Log_Func("OnAwnser_Func) 2 _id : " + _id + " / _callbackKey : " + _callbackKey + " / _nextIDialogueData : " + _nextIDialogueData,
                Debug_C.PrintLogType.Dialogue);

            // 다음 다이얼로그 데이터가 있는가?
            if (_nextIDialogueData != null && _isSkip == false)
                this.OnDialogue_Func(_nextIDialogueData, this.onBtnSelectDel, this.doneDel);
            else
                this.OnDialogueDone_Func();
        }
        protected virtual void OnDialogueDone_Func()
        {
            this.uiDialogueClass.Deactivate_Func();
        }
        public void OnBtn_Func(int _id)
        {
            this.uiDialogueClass.OnBtn_Func(_id);
        }

        public virtual void OnDeactivateUI_Func()
        {
            Action<string, string> _doneDel = this.doneDel;
            IDialogueData _currentDialogueData = this.currentDialogueData;

            this.onBtnSelectDel = null;
            this.doneDel = null;
            this.currentDialogueData = null;

            if (_doneDel != null)
            {
                string _groupKey = _currentDialogueData.GetGroupKey;
                string _key = _currentDialogueData.GetKey;
                _doneDel(_groupKey, _key);
            }
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {
                this.OnDialogueDone_Func();
            }

            this.onBtnSelectDel = null;
            this.doneDel = null;
            this.currentDialogueData = null;
        }

#if UNITY_EDITOR
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;

            if(this.uiDialogueClass == null)
                this.uiDialogueClass = GameObject.FindObjectOfType<Cargold.Dialogue.UI_Dialogue_Script>();
        }

        [Button("다이얼로그 활성화 (UID)")]
        private void CallEdit_OnDialogue_Func(string _key)
        {
            this.OnDialogue_Func(_key);
        } 
#endif
    }

    public static class DialogueExtension
    {
        public static bool IsEmpty_Func(this IDialogueData _iData)
        {
            if (_iData.GetBtnDataArr.IsHave_Func() == false)
                return false;

            if (_iData.GetBtnDataArr[0] == null)
                return true;

            return _iData.GetBtnDataArr[0].GetLczKey.IsNullOrWhiteSpace_Func() == true;
        }
    }
}