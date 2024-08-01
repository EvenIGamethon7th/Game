using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class UnitEnchantData : Data_C
{
     public string Key;
     [LabelText("강화단계")] public int Enchant_Step;
     [LabelText("소모다이아")] public int Enchant_Price;
     [LabelText("상승능력치")] public float Enchant_Stat;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Enchant_Step = _cellDataArr[1].ToInt();
        Enchant_Price = _cellDataArr[2].ToInt();
        Enchant_Stat = _cellDataArr[3].ToFloat();
    }
#endif
}