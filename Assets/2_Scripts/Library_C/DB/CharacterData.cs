using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;
using OfficeOpenXml.FormulaParsing.Excel.Operators;
using Unity.VisualScripting;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

public partial class CharacterData
{
    [LabelText("아카데미 졸업 여부")] public bool isAlumni;
    [LabelText("졸업 공격력")] public float alumniAtk;
    [LabelText("졸업 공격속도")] public float alumniAtkSpeed;
    [LabelText("졸업 마법공격력")] public float alumniMatk;

    protected override void Init_Project_Func()
    {
        base.Init_Project_Func();

        /* 런타임 즉시 이 함수가 호출됩니다.
         * 이 스크립트는 덮어쓰이지 않습니다.
         * 임의의 데이터 재가공을 원한다면 이 밑으로 코드를 작성하시면 됩니다.
         */
    }

    public static CharacterData operator +(CharacterData data1, CharacterData data2)
    {
        CharacterData newData = new();
        newData.atk = data1.atk + data2.atk;
        newData.matk = data1.matk + data2.matk;
        newData.atkSpeed = data1.atkSpeed + data2.atkSpeed;
        newData.rank = data1.rank;
        newData.characterPack = data1.characterPack;
        newData.range = data1.range;
        newData.nameKey = data1.nameKey;
        newData.Key = data1.Key;
        newData.isAlumni = data1.isAlumni || data2.isAlumni;
        newData.alumniAtk = data1.alumniAtk + data2.alumniAtk;
        newData.alumniAtkSpeed = data1.alumniAtkSpeed + data2.alumniAtkSpeed;
        newData.alumniMatk = data1.alumniMatk + data2.alumniMatk;

        return newData;
    }

    public void AddAlumniInfo(CharacterData data)
    {
        isAlumni = isAlumni || data.isAlumni;
        alumniAtk += data.alumniAtk;
        alumniAtkSpeed +=  data.alumniAtkSpeed;
        alumniMatk +=  data.alumniMatk;
    }

#if UNITY_EDITOR
    public override void CallEdit_OnDataImportDone_Func()
    {
        base.CallEdit_OnDataImportDone_Func();

        /* 테이블 임포트가 모두 마무리된 뒤 마지막으로 이 함수가 호출됩니다.
         * 이 스크립트는 덮어쓰이지 않습니다.
         * 임의의 데이터 재가공을 원한다면 이 밑으로 코드를 작성하시면 됩니다.
         */
    }
#endif

}