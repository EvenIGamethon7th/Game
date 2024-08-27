using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class WaveStatData : Data_C
{
     public string Key;
     [LabelText("웨이브")] public int wave;
     [LabelText("체력")] public float hp;
     [LabelText("방어력")] public float def;
     [LabelText("이동속도")] public float speed;
     [LabelText("공격력")] public int atk;
     [LabelText("보상재화1")] public string reward_type1;
     [LabelText("보상개수1")] public int reward_count1;
     [LabelText("보상재화2")] public string reward_type2;
     [LabelText("보상개수2")] public int reward_count2;
     [LabelText("마법방어력")] public float mdef;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        wave = _cellDataArr[1].ToInt();
        hp = _cellDataArr[2].ToFloat();
        def = _cellDataArr[3].ToFloat();
        speed = _cellDataArr[4].ToFloat();
        atk = _cellDataArr[5].ToInt();
        reward_type1 = _cellDataArr[6];
        reward_count1 = _cellDataArr[7].ToInt();
        reward_type2 = _cellDataArr[8];
        reward_count2 = _cellDataArr[9].ToInt();
        mdef = _cellDataArr[10].ToFloat();
    }
#endif
}