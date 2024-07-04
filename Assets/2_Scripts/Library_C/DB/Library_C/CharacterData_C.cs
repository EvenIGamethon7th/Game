using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;
using _2_Scripts.Game.Unit;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class CharacterData : Data_C
{
     public string Key;
     [LabelText("이름")] public string nameKey;
     [LabelText("공격력")] public float atk;
     [LabelText("공격속도")] public float atkSpeed;
     [LabelText("마법공격력")] public float matk;
     [LabelText("랭크")] public int rank;
     [LabelText("캐릭터팩")] public string characterPack;
     [LabelText("공격사거리")] public float range;
     [LabelText("소환재화")] public int cost;


#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        nameKey = _cellDataArr[1];
        atk = _cellDataArr[2].ToFloat();
        atkSpeed = _cellDataArr[3].ToFloat();
        matk = _cellDataArr[4].ToFloat();
        rank = _cellDataArr[5].ToInt();
        characterPack = _cellDataArr[6];
        range = _cellDataArr[7].ToFloat();
        cost = _cellDataArr[8].ToInt();
    }
#endif
}