using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.FrameWork
{
    public partial class DataBase_Manager // C - Inapp
    {
        public abstract Cargold.SDK.Purchase.IDB_Inapp GetInapp_C { get; }
    }
}

namespace Cargold.SDK.Purchase
{
#if Purchase_Unity_C
    using UnityEngine.Purchasing;
#endif

    public interface IDB_Inapp : DB.TableImporter.ILibraryDataGroup<IInappData>
    {
        public IInappData[] GetDataArr_Func();
    }

    public interface IInappData
    {
        string GetKey { get; }
        string GetGoogleID { get; }
        string GetAppleID { get; }
        int GetProductTypeID { get; }
#if Purchase_Unity_C
        UnityEngine.Purchasing.ProductType GetProductType { get; } 
#endif
    }

    public static partial class Extention_C
    {
#if Purchase_Unity_C
        public static string GetInappID_Func(this IInappData _iInappData)
        {
#if UNITY_ANDROID
            return _AOS_Func();
#elif UNITY_IOS
            return _IOS_Func();
#endif

            string _AOS_Func()
            {
                return _iInappData.GetGoogleID;
            }

            string _IOS_Func()
            {
                return _iInappData.GetAppleID;
            }
        } 
#endif
    }
}