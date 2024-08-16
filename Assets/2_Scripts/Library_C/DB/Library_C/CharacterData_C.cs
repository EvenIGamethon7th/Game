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
     [LabelText("마력")] public float matk;
     [LabelText("랭크")] public int rank;
     [LabelText("캐릭터데이터")] public string characterData;
     [LabelText("공격사거리")] public float range;
     [LabelText("소환재화")] public int cost;
     [LabelText("캐릭터팩")] public string characterPack;
     [LabelText("아카데미수업")] public int academyClass;
     [LabelText("스킬1")] public string Skill1;
     [LabelText("스킬2")] public string Skill2;
     [LabelText("클래스타입")] public string ClassType;
     [LabelText("아이콘")] public Sprite Icon;
     [LabelText("일러스트")] public Sprite illustration;
     [LabelText("졸업대사")] public string speech;

    

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
        academyClass = _cellDataArr[11].ToInt();
        Skill1 = _cellDataArr[12];
        Skill2 = _cellDataArr[13];
        ClassType = _cellDataArr[14];
        Icon = Editor_C.GetLoadAssetAtPath_Func<Sprite>(_cellDataArr[15]);
        illustration = Editor_C.GetLoadAssetAtPath_Func<Sprite>(_cellDataArr[16]);
        speech = _cellDataArr[17];
    }
#endif
}