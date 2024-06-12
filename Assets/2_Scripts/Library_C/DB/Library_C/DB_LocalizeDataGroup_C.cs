using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class DB_LocalizeDataGroup : DataGroup_C<LocalizeData>, Cargold.FrameWork.IDB_Localize
{
   
            bool ILibraryDataGroup<Cargold.FrameWork.ILczData>.TryGetData_Func(string _key, out Cargold.FrameWork.ILczData _lczData)
            {
                if (base.dataDic != null && base.dataDic.ContainsKey(_key) == true)
                {
                    _lczData = base.dataDic[_key];
                    return true;
                }
                else
                {
                    _lczData = null;
                    return false;
                }
            }
            
}