using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class CharacterData : Data_C
{
     public string Key;
     [LabelText("그룹")] public int Group;
     [LabelText("이름")] public string nameKey;
     [LabelText("공격력")] public float atk;
     [LabelText("공격속도")] public float atkSpeed;
     [LabelText("마법공격력")] public float matk;
     [LabelText("랭크")] public int rank;
     [LabelText("캐릭터데이터")] public string characterData;
     [LabelText("공격사거리")] public float range;
     [LabelText("소환재화")] public int cost;
     [LabelText("캐릭터팩")] public string characterPack;
     [LabelText("스킬1")] public int Skill1;
     [LabelText("스킬2")] public int Skill2;
     [LabelText("대사")] public string Description;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Group = _cellDataArr[1].ToInt();
        nameKey = _cellDataArr[2];
        atk = _cellDataArr[3].ToFloat();
        atkSpeed = _cellDataArr[4].ToFloat();
        matk = _cellDataArr[5].ToFloat();
        rank = _cellDataArr[6].ToInt();
        characterData = _cellDataArr[7];
        range = _cellDataArr[8].ToFloat();
        cost = _cellDataArr[9].ToInt();
        characterPack = _cellDataArr[10];
        Skill1 = _cellDataArr[11].ToInt();
        Skill2 = _cellDataArr[12].ToInt();
        Description = _cellDataArr[13];
    }
#endif
}