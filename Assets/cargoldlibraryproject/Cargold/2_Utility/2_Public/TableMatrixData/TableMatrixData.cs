using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Sirenix.Utilities;

namespace Cargold.TableMatrixData
{
    [System.Serializable]
    public abstract class TableMatrixData<DataKey, DataType> where DataType : TableMatrixData<DataKey, DataType>.Data
    {
        private const string MaxSizeStr = "전체 크기";
        private const int RowHeight = 50;

        [SerializeField] protected DataKey dataKey;

        [TableMatrix(DrawElementMethod = "CallEdit_Tool_Func", ResizableColumns = false, RowHeight = TableMatrixData<DataKey, DataType>.RowHeight), SerializeField]
        private DataType[,] dataArrArr;

        [FoldoutGroup(MaxSizeStr)] public int maxX;
        [FoldoutGroup(MaxSizeStr)] public int maxY;

        public TableMatrixData()
        {
            this.dataArrArr = new DataType[0, 0];
        }

        public DataType GetDate_Func(int _x, int _y)
        {
            int _modifyY = this.maxY - _y - 1;
            return this.dataArrArr[_x, _modifyY];
        }
        public int GetLengthX_Func()
        {
            return this.dataArrArr.GetLength(0);
        }
        public int GetLengthY_Func()
        {
            return this.dataArrArr.GetLength(1);
        }

        protected virtual DataType OnSelectData_Func(DataType _data)
        {
            return _data;
        }
        protected abstract DataType OnGenerateData_Func();
        public abstract bool IsVaildDataKey_Func(DataKey _dataKey);

        [FoldoutGroup(MaxSizeStr), Button("조정하기")]
        private void ReSize_Func()
        {
            DataType[,] _dataArrArr = new DataType[this.maxX, this.maxY];

            for (int _x = 0; _x < this.maxX; _x++)
            {
                for (int _y = 0; _y < this.maxY; _y++)
                {
                    DataType _data = null;

                    if (_x < this.dataArrArr.GetLength(0) && _y < this.dataArrArr.GetLength(1))
                    {
                        _data = this.dataArrArr[_x, _y];
                    }
                    else
                    {
                        _data = this.OnGenerateData_Func();
                    }

                    _dataArrArr[_x, _y] = _data;
                }
            }

            this.dataArrArr = _dataArrArr;
        }
        protected virtual DataKey GetDataKey_Func()
        {
            return this.dataKey;
        }

#if UNITY_EDITOR
        private DataType CallEdit_Tool_Func(Rect rect, DataType _tileData)
        {
            DataKey _dataKey = this.GetDataKey_Func();

            if (this.IsVaildDataKey_Func(_dataKey) == false)
                return _tileData;

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                _tileData.dataKey = _dataKey;
                _tileData = this.OnSelectData_Func(_tileData);
                GUI.changed = true;
                Event.current.Use();
            }

            Texture _texture = _tileData.CallEdit_GetTexture_Func();

            UnityEditor.EditorGUI.DrawPreviewTexture(rect.Padding(1), _texture);

            return _tileData;
        }
#endif

        [System.Serializable]
        public abstract class Data
        {
            public DataKey dataKey;
#if UNITY_EDITOR
            public virtual Texture CallEdit_GetTexture_Func()
            {
                return Cargold.FrameWork.DataBase_Manager.Instance.GetDefine_C.emptyTexture;
            }
#endif
        }
    } 
}