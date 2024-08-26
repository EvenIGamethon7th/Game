using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class Reward_CHData : Data_C
{
     public string Key;
     [LabelText("최소 웨이브")] public int Min_Wave;
     [LabelText("최대 웨이브")] public int Max_Wave;
     [LabelText("보상재화")] public string Reward_Value;
     [LabelText("보상재화개수")] public int Reward_Count;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Min_Wave = _cellDataArr[1].ToInt();
        Max_Wave = _cellDataArr[2].ToInt();
        Reward_Value = _cellDataArr[3];
        Reward_Count = _cellDataArr[4].ToInt();
    }
#endif
}