using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class TreasureBoxData : Data_C
{
     public string Key;
     [LabelText("인덱스")] public int Index;
     [LabelText("상자유형")] public EBoxType Box_Type;
     [LabelText("소모재화")] public string Cost_Type;
     [LabelText("소모개수")] public int Cost_Count;
     [LabelText("보상유형")] public ERewardType Reward_Type;
     [LabelText("보상내용1")] public int Reward_Value1;
     [LabelText("보상개수")] public int Reward_Count;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Index = _cellDataArr[1].ToInt();
        Box_Type = _cellDataArr[2].ToEnum<EBoxType>();
        Cost_Type = _cellDataArr[3];
        Cost_Count = _cellDataArr[4].ToInt();
        Reward_Type = _cellDataArr[5].ToEnum<ERewardType>();
        Reward_Value1 = _cellDataArr[6].ToInt();
        Reward_Count = _cellDataArr[7].ToInt();
    }
#endif
}