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
     [LabelText("방어력")] public float def;
     [LabelText("마법방어력")] public float mdef;
     [LabelText("이동속도")] public float speed;
     [LabelText("공격력")] public int atk;
     [LabelText("보상재화")] public string reward_type;
     [LabelText("보상개수")] public int reward_count;
     [LabelText("이미지")] public string image;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        nameKey = _cellDataArr[1];
        hp = _cellDataArr[2].ToFloat();
        def = _cellDataArr[3].ToFloat();
        mdef = _cellDataArr[4].ToFloat();
        speed = _cellDataArr[5].ToFloat();
        atk = _cellDataArr[6].ToInt();
        reward_type = _cellDataArr[7];
        reward_count = _cellDataArr[8].ToInt();
        image = _cellDataArr[9];
    }
#endif
}