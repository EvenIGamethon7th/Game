using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold.Dialogue;
using Cargold.FrameWork;

namespace Cargold.Example
{
    public class DB_다이얼로그
    {
        public abstract class DBM : Cargold.FrameWork.DataBase_Manager
        {
            public Dialogue.IDB_Dialogue 재정의;

            /**/
            public override Cargold.Dialogue.IDB_Dialogue GetDialogue_C => this.재정의;
            /**/
        }

        public class DB_이름 : Cargold.DB.TableImporter.DataGroup_C<이름Data>/**/, Cargold.Dialogue.IDB_Dialogue/**/
        {
            /**/
            [ShowInInspector, ReadOnly] private Dictionary<string, List<Cargold.Dialogue.IDialogueData>> groupKeyByDataListDic;

            protected override void Init_Library_Func()
            {
                base.Init_Library_Func();

                if (this.groupKeyByDataListDic == null)
                    this.groupKeyByDataListDic = new Dictionary<string, List<Cargold.Dialogue.IDialogueData>>();
                else
                    this.groupKeyByDataListDic.Clear();

                foreach (Cargold.Dialogue.IDialogueData _iData in base.dataArr)
                {
                    string _groupKey = _iData.GetGroupKey;
                    if (_groupKey.IsNullOrWhiteSpace_Func() == true)
                        continue;

                    if (this.groupKeyByDataListDic.TryGetValue(_groupKey, out List<Cargold.Dialogue.IDialogueData> _list) == false)
                    {
                        _list = new List<Cargold.Dialogue.IDialogueData>();
                        this.groupKeyByDataListDic.Add(_groupKey, _list);
                    }

                    int _groupID = _list.Count;
                    _iData.SetGroupID_Func(_groupID);

                    _list.Add(_iData);
                }
            }
            bool Cargold.DB.TableImporter.ILibraryDataGroup<Cargold.Dialogue.IDialogueData>.TryGetData_Func(string _key, out Cargold.Dialogue.IDialogueData _data)
            {
                if (base.dataDic != null && base.dataDic.ContainsKey(_key) == true)
                {
                    _data = base.dataDic[_key];
                    return true;
                }
                else
                {
                    _data = null;
                    return false;
                }
            }
            public bool TryGetNextData_Func(Cargold.Dialogue.IDialogueData _endIData, out Cargold.Dialogue.IDialogueData _nextIData)
            {
                string _groupKey = _endIData.GetGroupKey;
                if(_groupKey.IsNullOrWhiteSpace_Func() == false && this.groupKeyByDataListDic.TryGetValue(_groupKey, out List<Cargold.Dialogue.IDialogueData> _list) == true)
                {
                    int _nextGroupID = _endIData.GetGroupID + 1;

                    if(_list.TryGetItem_Func(_nextGroupID, out _nextIData) == true)
                    {
                        return true;
                    }
                }

                _nextIData = null;
                return false;
            }
            /**/
        }

        public class 이름Data : Cargold.DB.TableImporter.Data_C/**/, Cargold.Dialogue.IDialogueData/**/
        {
            public string Key;

            /**/
        [ShowInInspector, ReadOnly, LabelText("그룹 ID")] public int groupID;

        string Cargold.Dialogue.IDialogueData.GetKey => this.Key;
        string Cargold.Dialogue.IDialogueData.GetGroupKey => /*5*/default;
        string Cargold.Dialogue.IDialogueData.GetNameLczKey => /*0*/default;
        string Cargold.Dialogue.IDialogueData.GetDescLczKey => /*1*/default;
        string Cargold.Dialogue.IDialogueData.GetSpeakerType => /*2*/default;
        string Cargold.Dialogue.IDialogueData.GetSpeckerAniTrigger => /*3*/default;
        Cargold.Dialogue.DialogueButtonData[] Cargold.Dialogue.IDialogueData.GetBtnDataArr => /*4*/default;
        bool Cargold.Dialogue.IDialogueData.IsDimOff => /*6*/default;
        int Cargold.Dialogue.IDialogueData.GetGroupID => this.groupID;


        void Cargold.Dialogue.IDialogueData.SetGroupID_Func(int _id)
        {
            this.groupID = _id;
        }
            /**/

#if UNITY_EDITOR
            public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
            {
                throw new System.NotImplementedException();
            }
#endif
        }
    } 
}