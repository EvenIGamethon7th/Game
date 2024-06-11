using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.SDK.Purchase;
using Cargold.DB.TableImporter;
#if Purchase_Unity_C
using UnityEngine.Purchasing; 
#endif

namespace Cargold.Example
{
    public class DB_인앱
    {
        public abstract class DBM : Cargold.FrameWork.DataBase_Manager
        {
            public Cargold.SDK.Purchase.IDB_Inapp 재정의;

            /**/
            public override Cargold.SDK.Purchase.IDB_Inapp GetInapp_C => this.재정의;
            /**/
        }
    }

    public class DB_이름 : Cargold.DB.TableImporter.DataGroup_C<이름Data>/**/, Cargold.SDK.Purchase.IDB_Inapp/**/
    {
        /**/
        Cargold.SDK.Purchase.IInappData[] Cargold.SDK.Purchase.IDB_Inapp.GetDataArr_Func()
        {
            return base.dataArr;
        }

        bool ILibraryDataGroup<Cargold.SDK.Purchase.IInappData>.TryGetData_Func(string _key, out Cargold.SDK.Purchase.IInappData _iData)
        {
            if (base.dataDic.ContainsKey(_key) == true)
            {
                _iData = base.dataDic[_key];
                return true;
            }
            else
            {
                _iData = null;
                return false;
            }
        }
        /**/
    }

    public class 이름Data : Cargold.DB.TableImporter.Data_C/**/, Cargold.SDK.Purchase.IInappData/**/
    {
        public string Key;

        /**/
        string Cargold.SDK.Purchase.IInappData.GetKey => this.Key;
        public string GetGoogleID => /*0*/default; // 중요 : ID에는 소문자, 숫자, 밑줄 또는 마침표만 사용할 수 있습니다. 예외처리 ㄱ?
        public string GetAppleID => /*1*/default;
        public int GetProductTypeID => /*2*/default;
#if Purchase_Unity_C
        UnityEngine.Purchasing.ProductType Cargold.SDK.Purchase.IInappData.GetProductType => (UnityEngine.Purchasing.ProductType)this.GetProductTypeID; 
#endif
        /**/

#if UNITY_EDITOR
        public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
        {
            throw new System.NotImplementedException();
        }
#endif
    }
}