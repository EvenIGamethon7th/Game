using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class MonsterData : Data_C
{
     public string Key;
     [LabelText("이름")] public string nameKey;
     [LabelText("체력")] public float hp;
     [LabelText("공격력")] public float atk;
     [LabelText("방어력")] public float def;
     [LabelText("이동속도")] public float speed;
     [LabelText("마법방어력")] public float mdef;
     [LabelText("경험치")] public int exp;
     [LabelText("보상")] public int reward;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        nameKey = _cellDataArr[1];
        hp = _cellDataArr[2].ToFloat();
        atk = _cellDataArr[3].ToFloat();
        def = _cellDataArr[4].ToFloat();
        speed = _cellDataArr[5].ToFloat();
        mdef = _cellDataArr[6].ToFloat();
        exp = _cellDataArr[7].ToInt();
        reward = _cellDataArr[8].ToInt();
    }
#endif
}