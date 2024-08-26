using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class WaveData : Data_C
{
     public string Key;
     [LabelText("스테이지그룹")] public int Stage_Group;
     [LabelText("이름")] public string monsterKey;
     [LabelText("소환갯수")] public int spawnCount;
     [LabelText("보스웨이브여부")] public bool isBoss;
     [LabelText("적용스탯")] public string apply_stat;
     [LabelText("스탯(체력)가중치")] public float weight;
     [LabelText("얼음몬스터여부")] public bool isIceMonster;
     [LabelText("스탯(방어력)가중치")] public float weight_def;
     [LabelText("스탯(마방)가중치")] public float weight_mdef;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Stage_Group = _cellDataArr[1].ToInt();
        monsterKey = _cellDataArr[2];
        spawnCount = _cellDataArr[3].ToInt();
        isBoss = _cellDataArr[4].ToBool();
        apply_stat = _cellDataArr[5];
        weight = _cellDataArr[6].ToFloat();
        isIceMonster = _cellDataArr[7].ToBool();
        weight_def = _cellDataArr[8].ToFloat();
        weight_mdef = _cellDataArr[9].ToFloat();
    }
#endif
}