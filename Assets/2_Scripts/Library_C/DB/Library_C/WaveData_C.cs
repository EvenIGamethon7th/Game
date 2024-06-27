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
     [LabelText("이름")] public string monsterKey;
     [LabelText("소환갯수")] public int spawnCount;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        monsterKey = _cellDataArr[1];
        spawnCount = _cellDataArr[2].ToInt();
    }
#endif
}