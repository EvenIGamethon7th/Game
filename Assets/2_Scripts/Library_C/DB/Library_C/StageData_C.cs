using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class StageData : Data_C
{
     public string Key;
     [LabelText("이름")] public string[] waveList;
     [LabelText("체력 가중치")] public float hpWeight;
     [LabelText("방어력 가중치")] public float defWeight;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        string[] _strArr1 = _cellDataArr[1].Split(',');
        waveList = new string[_strArr1.Length];
        waveList = _strArr1;
        hpWeight = _cellDataArr[2].ToFloat();
        defWeight = _cellDataArr[3].ToFloat();
    }
#endif
}