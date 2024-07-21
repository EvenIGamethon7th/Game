using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class ItemData : Data_C
{
     public string Key;
     [LabelText("정렬 순서")] public int sortID;
     [LabelText("값")] public string nameKey;
     [LabelText("아이템 설명 키")] public string descKey;
     [LabelText("초기아이템")] public bool startItem;
     [LabelText("아이템 티어")] public int tier;
     [LabelText("아이템 가치")] public int value;
     [LabelText("스킬Key1")] public string skillKey1;
     [LabelText("스킬Key2")] public string skillKey2;
     [LabelText("테스트")] public TestTypeType testEnum;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        sortID = _cellDataArr[1].ToInt();
        nameKey = _cellDataArr[2];
        descKey = _cellDataArr[3];
        startItem = _cellDataArr[4].ToBool();
        tier = _cellDataArr[5].ToInt();
        value = _cellDataArr[6].ToInt();
        skillKey1 = _cellDataArr[7];
        skillKey2 = _cellDataArr[8];
        testEnum = _cellDataArr[9].ToEnum<TestTypeType>();
    }
#endif
}