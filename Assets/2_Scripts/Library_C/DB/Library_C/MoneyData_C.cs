using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class MoneyData : Data_C
{
     public string Key;
     [LabelText("재화 유형")] public EMoneyType Type;
     [LabelText("재화명")] public string Name;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Type = _cellDataArr[1].ToEnum<EMoneyType>();
        Name = _cellDataArr[2];
    }
#endif
}