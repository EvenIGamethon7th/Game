using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.EventSystem;

namespace Cargold.EventSystem
{
    public class EventStep_Action_Dialogue : EventStep_Action
    {
        public override ActionType GetActionType => ActionType.다이얼로그;

        [SerializeField, LabelText("다이얼로그 Key"), ValidateInput("CallEdit_KeyValidate_Func", "해당 다이얼로그 Key는 존재하지 않습니다."), LabelWidth(100f)]
        protected string dialKey;

        public override bool IsAutoNextStepWithAction_Func(bool _isCalledLastAction) => false;
        protected override void OnActionOverride_Func()
        {
            Dialogue.DialogueSystem_Manager.Instance.OnDialogue_Func(this.dialKey, this.OnAwnser_Func, this.OnDone_Func);
        }

        protected virtual bool OnAwnser_Func(string _dialKey, int _btnID, string _callbackKey, string _nextDialKey)
        {
            if(_nextDialKey.IsNullOrWhiteSpace_Func() == true)
            {
                if (EventSystem_Manager.Instance.IsHaveNextStep_Func(ActionType.다이얼로그) == true)
                {
                    EventSystem.EventSystem_Manager.Instance.OnStep_Func();

                    return false;
                }
            }

            return true;
        }
        protected virtual void OnDone_Func(string _groupKey, string _dialKey)
        {
            EventSystem.EventSystem_Manager.Instance.OnStep_Func();
        }
        protected override string OnLogParam_Func()
        {
            return $"Dial Key : {this.dialKey}";
        }

#if UNITY_EDITOR
        private bool CallEdit_KeyValidate_Func()
        {
            if (this.dialKey.IsNullOrWhiteSpace_Func() == true) return false;

            return Cargold.FrameWork.DataBase_Manager.Instance.GetDialogue_C.TryGetData_Func(this.dialKey, out _);
        }
        [ShowInInspector, HideLabel]
        private string CallEdit_GetDialDesc
        {
            get
            {
                if (this.dialKey.IsNullOrWhiteSpace_Func() == true)
                    return "빈 값 ~";

                if (Cargold.FrameWork.DataBase_Manager.Instance.GetDialogue_C.TryGetData_Func(this.dialKey, out Dialogue.IDialogueData _iData) == false)
                    return "메롱 ~";

                string _lczKey = _iData.GetDescLczKey;
                if (Cargold.FrameWork.DataBase_Manager.Instance.GetLocalize_C.TryGetData_Func(_lczKey, out FrameWork.ILczData _iLczData) == false)
                    return "Lcz Key Error : " + _iLczData;

                return _iLczData.GetLczStr_Func(SystemLanguage.Korean);
            }
        }

        public override string CallEdit_GetSerialize_Func(int _minPathCnt)
        {
            return StringBuilder_C.GetPath_Func(Event_Script.CallEdit_GetParseStr_Func(2)
                , _minPathCnt
                , this.dialKey.ToString()
                );
        }
        public override bool CallEdit_IsUnitTestDone_Func()
        {
            return this.CallEdit_KeyValidate_Func() == true;
        }
#endif
    }
}