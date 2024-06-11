using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;
using Cargold.Dialogue;
using Cargold.FrameWork;

namespace Cargold.Example {
    [System.Serializable]
    public partial class 다이얼로그버튼데이터 : Cargold.Dialogue.DialogueButtonData
    {
        public string lczKey;
        public UI_C.BtnType btnType;
        public string nextDialogueKey;
        public string callbackKey;

        public override string GetLczKey => this.lczKey;
        public override UI_C.BtnType GetBtnType => this.btnType;
        public override string GetNextDialogueDataKey => this.nextDialogueKey;
        public override string GetCallbackKey => this.callbackKey;

#if UNITY_EDITOR
        public override void CallEdit_OnDataImport_Func(string _cellData)
        {
            string[] _cellDataArr = _cellData.Split('!');

            this.lczKey = _cellDataArr[0];
            this.btnType = _cellDataArr[1].ToEnum<UI_C.BtnType>();
            this.nextDialogueKey = _cellDataArr[2];
            string _btnCallbackKey = _cellDataArr[3];
            this.callbackKey = _btnCallbackKey;

            if (_btnCallbackKey.IsNullOrWhiteSpace_Func() == false)
                LibraryRemocon.UtilityClassData.DialogueData.Instance.CallEdit_AddCallbackKey_Func(_btnCallbackKey);
        }
#endif
    } 
} // End