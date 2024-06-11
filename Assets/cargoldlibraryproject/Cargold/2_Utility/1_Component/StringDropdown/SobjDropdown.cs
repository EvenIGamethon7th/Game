using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold
{
    public class SobjDropdown : SerializedScriptableObject 
    {
        private const string DefaultName = "추가하고자 하는 Key를 넣으셈";

#if UNITY_EDITOR
        private static List<DropdownData> DropdownDataList = new List<DropdownData>();

        [BoxGroup(Editor_C.Mandatory), SerializeField, LabelText("목록"), ListDrawerSettings(HideRemoveButton = true, HideAddButton = true, IsReadOnly = true)]
        protected List<DropdownData> dropdownDataList = new List<DropdownData>();
        [FoldoutGroup(Editor_C.Optional), SerializeField, ReadOnly] private int currentUidNum = 1;
        [FoldoutGroup(Editor_C.Optional), SerializeField, ReadOnly] private List<ValueDropdownItem<int>> valueDropdownList = new List<ValueDropdownItem<int>>();
        [FoldoutGroup(Editor_C.Optional), SerializeField, LabelText("데이터 변경 여부")] private bool isDataChanged = false;
        [FoldoutGroup(Editor_C.Optional), SerializeField, LabelText("스크립트 생성 여부")] private bool isMustScriptGenerate = false;
        [FoldoutGroup(Editor_C.Optional), SerializeField, LabelText("데이터 타입")] private string dataTypeStr = CargoldLibrary_C.GetInitError;
        [FoldoutGroup(Editor_C.Optional), SerializeField, LabelText("스크립트 생성 경로"), FolderPath] private string scriptFolderPath = CargoldLibrary_C.GetInitError;
        [FoldoutGroup(Editor_C.Optional), SerializeField, LabelText("카라리용 드롭다운 여부")] private bool isLibrary = false;
        [FoldoutGroup(Editor_C.Optional), SerializeField, LabelText("카라리용 기본 데이터 ID"), ReadOnly] private List<int> initIdList = new List<int>();

        public IEnumerable GetIEnumerable
        {
            get
            {
                this.CallEdit_Sync_Func(); 

                return this.valueDropdownList;
            }
        }

        public void Init_Func(string _dataTypeStr, string _scriptFolderPath, bool _isLibrary = false, DropdownData[] _initDataArr = null)
        {
            this.dataTypeStr = _dataTypeStr;
            this.scriptFolderPath = _scriptFolderPath;
            this.isLibrary = _isLibrary;

            DropdownData _noneData = new DropdownData();
            _noneData.nameStr = "None";
            _noneData.uid = 0;
            _noneData.stringDropdownClass = this;
            this.dropdownDataList.Add(_noneData);

            if (_initDataArr.IsHave_Func() == true)
            {
                for (int i = 0; i < _initDataArr.Length; i++)
                {
                    DropdownData _initData = _initDataArr[i];
                    _initData.stringDropdownClass = this;
                    this.dropdownDataList.Add(_initData);
                }
            }

            for (int i = 0; i < dropdownDataList.Count; i++)
            {
                ValueDropdownItem<int> _item = this.dropdownDataList[i].GetItem_Func();
                this.valueDropdownList.Add(_item);

                this.initIdList.Add(_item.Value);
            }
        }

        public string CallEdit_ToString_Func(int _uid)
        {
            for (int i = 0; i < dropdownDataList.Count; i++)
            {
                if (this.dropdownDataList[i].uid == _uid)
                    return this.dropdownDataList[i].nameStr;
            }

            return default;
        }

        [BoxGroup(Editor_C.Mandatory), Button("생성", ButtonStyle.Box)]
        private void CallEdit_AddItem_Func(string _nameKey = SobjDropdown.DefaultName)
        {
            if (_nameKey.IsCompare_Func(SobjDropdown.DefaultName) == true)
            {
                Debug_C.Log_Func("내용을 넣으셈");
                return;
            }

            if (_nameKey.Contains(" ") == false)
            {
                _AddItem_Func(_nameKey);
            }
            else
            {
                string[] _nameKeyArr = _nameKey.Split(" ");

                foreach (var _nameKeyItem in _nameKeyArr)
                {
                    _AddItem_Func(_nameKeyItem);
                }
            }

            void _AddItem_Func(string _nameKey)
            {
                int _uid = this.currentUidNum++;
                this.CallEdit_AddItem_Func(_nameKey, _uid);
            }
        }

        [FoldoutGroup(Editor_C.Optional), Button("강제 생성")]
        private void CallEdit_AddItem_Func(string _nameKey, int _uid)
        {
            bool _isUnique = true;

            for (int i = 0; i < this.dropdownDataList.Count; i++)
            {
                if(this.dropdownDataList[i].nameStr.IsCompare_Func(_nameKey) == true)
                {
                    _isUnique = false;

                    break;
                }
            }

            if(_isUnique == true)
            {
                DropdownData _newData = new DropdownData();
                _newData.stringDropdownClass = this;
                _newData.nameStr = _nameKey;
                _newData.uid = _uid;

                this.dropdownDataList.Add(_newData);

                this.CallEdit_Sync_Func(true);
            }
            else
            {
                Debug_C.Warning_Func("다음의 Item은 중복되므로 생성하지 않습니다. : " + _nameKey);
            }
        }

        [FoldoutGroup(Editor_C.Optional), Button("데이터 복구")]
        private void CallEdit_RestoreData_Func()
        {
            for (int i = 0; i < valueDropdownList.Count; i++)
            {
                ValueDropdownItem<int> _originData = this.valueDropdownList[i];

                this.dropdownDataList[i] = new DropdownData(_originData.Text, _originData.Value);
            }

            Editor_C.SetSaveAsset_Func(this);
        }

        private void CallEdit_RemoveItem_Func(DropdownData _data)
        {
            this.dropdownDataList.Remove(_data);

            this.CallEdit_Sync_Func(true);
        }

        private void CallEdit_IsDataChanged_Func()
        {
            this.isDataChanged = true;

            Editor_C.SetSaveAsset_Func(this);
        }

        [Button("데이터 갱신"), ShowIf("@isDataChanged == true"), GUIColor(.5f, 1f, .5f)]
        private void CallEdit_Sync_Func()
        {
            this.CallEdit_Sync_Func(false);
        }
        private void CallEdit_Sync_Func(bool _isForced)
        {
            if (this.isDataChanged == false && _isForced == false)
                return;

            if (this.valueDropdownList != null)
                this.valueDropdownList.Clear();
            else
                this.valueDropdownList = new List<ValueDropdownItem<int>>();

            foreach (DropdownData _dropdownData in this.dropdownDataList)
            {
                ValueDropdownItem<int> _item = _dropdownData.GetItem_Func();
                this.valueDropdownList.Add(_item);
            }

            this.isDataChanged = false;
            this.isMustScriptGenerate = true;

            Editor_C.SetSaveAsset_Func(this);
        }

        [Button("스크립트 생성"), ShowIf("@isMustScriptGenerate == true"), DisableIf("@isDataChanged == true"), GUIColor(1f, 0.5f, .5f)]
        private void CallEdit_ScriptGen_Func()
        {
            foreach (DropdownData _dropdownData in this.dropdownDataList)
            {
                if (this.initIdList.Contains(_dropdownData.uid) == false)
                    DropdownDataList.Add(_dropdownData);
            }

            LibraryRemocon.Instance.utilityClassData.stringDropdownData.CallEdit_GenerateConstString_Func(
                DropdownDataList, this.dataTypeStr, this.scriptFolderPath, this.isLibrary);

            this.isMustScriptGenerate = false;

            DropdownDataList.Clear();

            Editor_C.SetSaveAsset_Func(this);
        }
#endif

        [System.Serializable]
        public struct DropdownData
        {
            [OnValueChanged("OnValueChanged"), HideLabel, InlineButton("CallEdit_RemoveItem_Func", "제거")] public string nameStr;
            [HideInInspector] public int uid;
            [HideInInspector] public SobjDropdown stringDropdownClass;

            public DropdownData(string _nameStr, int _uid)
            {
                this.nameStr = _nameStr;
                this.uid = _uid;
                this.stringDropdownClass = null;
            }

            public ValueDropdownItem<int> GetItem_Func()
            {
                return new ValueDropdownItem<int>(this.nameStr, this.uid);
            }

#if UNITY_EDITOR
            private void CallEdit_RemoveItem_Func()
            {
                stringDropdownClass.CallEdit_RemoveItem_Func(this);
            }
            private void OnValueChanged()
            {
                stringDropdownClass.CallEdit_IsDataChanged_Func();
            } 
#endif
        }
    }
}