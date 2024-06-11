using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Cargold.DB.TableImporter
{
    [System.Serializable]
    public abstract class DataGroup_C : SerializedScriptableObject
    {
#if UNITY_EDITOR
        public abstract void CallEdit_OnDataImport_Func(SheetData _sheetData);
#endif
    }

    [System.Serializable]
    public abstract class DataGroup_C<T> : DataGroup_C where T : Data_C, new()
    {
        /// <summary>
        /// 순회 참조할 때 배열보다는 DataDic을 Forech로 접근하시는 걸 추천합니다.
        /// </summary>
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("데이터 배열")] protected T[] dataArr;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("데이터 딕셔너리")] protected Dictionary<string, T> dataDic;
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), SerializeField, LabelText("드롭다운 목록")] protected string[] keyArr;

        public T[] GetDataArr => this.dataArr;
        public Dictionary<string, T> GetDataDic => this.dataDic;
        public int GetDataNum => this.dataArr.Length;
        public string[] GetKeyArr => this.keyArr;

        public void Init_Func()
        {
            int _id = 0;
            foreach (var item in this.dataDic)
            {
                T _data = item.Value;
                _data.Init_Func();

                this.dataArr[_id++] = _data;
            }

            this.Init_Library_Func();
            this.Init_Project_Func();
        }
        protected virtual void Init_Library_Func() { }
        protected virtual void Init_Project_Func() { }

        public virtual T GetData_Func(int _id)
        {
            if (this.dataArr.TryGetItem_Func(_id, out T _item) == true)
            {
                return _item;
            }
            else
            {
                Debug_C.Error_Func(this.name + "에서 배열의 크기를 벗어났습니다. 기존 배열 크기 / 접근한 배열 ID : " + this.dataArr.Length + " / " + _id);

                return this.dataArr[0];
            }
        }
        public virtual T GetData_Func(string _key)
        {
            if (this.dataDic.TryGetValue(_key, out T _data) == true)
                return _data;
            else
            {
                if (_key.IsNullOrWhiteSpace_Func() == true)
                    _key = "(Null)";

                Debug_C.Error_Func(this.name + "에서 다음 키를 찾지 못했습니다. " + _key);

                return this.dataArr[0];
            }
        }
        public bool TryGetData_Func(string _key, out T _data)
        {
            if(_key.IsNullOrWhiteSpace_Func() == false)
                return this.dataDic.TryGetValue(_key, out _data);
            else
            {
                _data = null;
                return false;
            }
        }
        public bool IsContain_Func(string _key)
        {
            if (_key.IsNullOrWhiteSpace_Func() == false)
                return this.dataDic.ContainsKey(_key);
            else
                return false;
        }

#if UNITY_EDITOR
        public override void CallEdit_OnDataImport_Func(SheetData _sheetData)
        {
            int _totalRowNum = _sheetData.GetRowNum;

            T[] _dataArr = new T[_totalRowNum];
            Dictionary<string, T> _dataDic = new Dictionary<string, T>();

            for (int _rowID = 0; _rowID < _totalRowNum; _rowID++)
            {
                T _data = new T();
                _dataArr[_rowID] = _data;

                string[] _cellDataArr = _sheetData.GetCellDataArr_Func(_rowID);
                _data.CallEdit_OnDataImport_Func(_cellDataArr);

                string _key = _cellDataArr[0];

                try
                {
                    _AddDic_Func();
                }
                catch
                {
                    Debug_C.Error_Func("SheetName : " + _sheetData.sheetName + " / _rowID : " + _rowID + " / _key : " + _key + " / _data : " + _data);
                }

                void _AddDic_Func()
                {
                    _dataDic.Add(_key, _data);
                }
            }

            this.dataArr = _dataArr;
            this.dataDic = _dataDic;
        }
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), Button("DB 캐싱")]
        public virtual void CallEdit_OnDataImportDone_Func()
        {
            if(this.dataDic.IsHave_Func() == true)
            {
                this.keyArr = new string[this.dataDic.Count];

                int _id = 0;
                foreach (var item in this.dataDic)
                {
                    item.Value.CallEdit_OnDataImportDone_Func();
                    this.keyArr[_id++] = item.Key;
                }

                Editor_C.SetSaveAsset_Func(this);
            }
        }
#endif
    }

    [System.Serializable]
    public abstract class Data_C
    {
        public void Init_Func()
        {
            this.Init_Library_Func();
            this.Init_Project_Func();
        }
        protected virtual void Init_Library_Func() { }
        protected virtual void Init_Project_Func() { }

#if UNITY_EDITOR
        public abstract void CallEdit_OnDataImport_Func(string[] _cellDataArr);

        [FoldoutGroup("에디터"), Button("DB 캐싱")]
        public virtual void CallEdit_OnDataImportDone_Func()
        {
             
        }
#endif
    }

    // 라이브러리용 데이터 그룹
    public interface ILibraryDataGroup<T>
    {
        bool TryGetData_Func(string _key, out T _data);
    }
}