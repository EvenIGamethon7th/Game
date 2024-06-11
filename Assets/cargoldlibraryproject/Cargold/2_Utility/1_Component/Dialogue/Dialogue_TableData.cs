using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.FrameWork
{
    public partial class DataBase_Manager // C - Dial
    {
        public abstract Cargold.Dialogue.IDB_Dialogue GetDialogue_C { get; }
    }
}

namespace Cargold.Dialogue
{
    public interface IDB_Dialogue : DB.TableImporter.ILibraryDataGroup<IDialogueData>
    {
        bool TryGetNextData_Func(Cargold.Dialogue.IDialogueData _endIData, out Cargold.Dialogue.IDialogueData _nextIData);
    }

    public interface IDialogueData
    {
        string GetKey { get; }
        string GetGroupKey { get; }
        string GetNameLczKey { get; }
        string GetDescLczKey { get; }
        string GetSpeakerType { get; }
        string GetSpeckerAniTrigger { get; }
        DialogueButtonData[] GetBtnDataArr { get; }
        int GetGroupID { get; }
        bool IsDimOff { get; }

        void SetGroupID_Func(int _id);
    }

    public abstract class DialogueButtonData
    {
        public abstract string GetLczKey { get; }
        public abstract FrameWork.UI_C.BtnType GetBtnType { get; }
        public abstract string GetNextDialogueDataKey { get; }
        public abstract string GetCallbackKey { get; }

#if UNITY_EDITOR
        public abstract void CallEdit_OnDataImport_Func(string _cellData); 
#endif
    }

    [System.Serializable]
    public class BtnData
    {
        [SerializeField, LabelText("버튼 Lcz"), InlineProperty] private Cargold.FrameWork.LczPreviewData lczPreviewData = new FrameWork.LczPreviewData();
        [LabelText("버튼 종류")] public FrameWork.UI_C.BtnType btnType = FrameWork.UI_C.BtnType.Neutral;
        [LabelText("다음 대화 Key")] public string dialogueDataKey = null;

        public string GetBtnLczStr => this.lczPreviewData.GetLczStr;

        public void Init_Func()
        {

        }

#if UNITY_EDITOR
        public void CallEdit_OnDataImport_Func()
        {
            if (Cargold.FrameWork.DataBase_Manager.Instance.GetDialogue_C.TryGetData_Func(this.dialogueDataKey, out IDialogueData _data) == false)
                Debug_C.Error_Func("_key : " + this.dialogueDataKey);
        }
#endif
    }
}
