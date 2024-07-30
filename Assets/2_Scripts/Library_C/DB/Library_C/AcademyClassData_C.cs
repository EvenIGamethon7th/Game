using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class AcademyClassData : Data_C
{
     public string Key;
     [LabelText("그룹")] public int Group;
     [LabelText("적용 클래스")] public EClassType Apply_Class;
     [LabelText("이름")] public string AcademyClassKey;
     [LabelText("이름")] public string Name;
     [LabelText("성공 시 상승 능력치")] public string Stat_type1;
     [LabelText("성공 시 상승 수치")] public float Stat_value1;
     [LabelText("대성공 시 상승 수치")] public float Stat_value2;
     [LabelText("성공확률")] public float Success_pro;
     [LabelText("대성공확률")] public float Great_pro;
     [LabelText("실패확률")] public float Fail_pro;
     [LabelText("1타강사 성공")] public float Success_pro_item1;
     [LabelText("1타강사 대성공")] public float Great_pro_item1;
     [LabelText("1타강사 실패")] public float Fail_pro_item1;
     [LabelText("2타강사 성공")] public float Success_pro_item2;
     [LabelText("2타강사 대성공")] public float Great_pro_item2;
     [LabelText("2타강사 실패")] public float Fail_pro_item2;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Group = _cellDataArr[1].ToInt();
        Apply_Class = _cellDataArr[2].ToEnum<EClassType>();
        AcademyClassKey = _cellDataArr[3];
        Name = _cellDataArr[4];
        Stat_type1 = _cellDataArr[5];
        Stat_value1 = _cellDataArr[6].ToFloat();
        Stat_value2 = _cellDataArr[7].ToFloat();
        Success_pro = _cellDataArr[8].ToFloat();
        Great_pro = _cellDataArr[9].ToFloat();
        Fail_pro = _cellDataArr[10].ToFloat();
        Success_pro_item1 = _cellDataArr[11].ToFloat();
        Great_pro_item1 = _cellDataArr[12].ToFloat();
        Fail_pro_item1 = _cellDataArr[13].ToFloat();
        Success_pro_item2 = _cellDataArr[14].ToFloat();
        Great_pro_item2 = _cellDataArr[15].ToFloat();
        Fail_pro_item2 = _cellDataArr[16].ToFloat();
    }
#endif
}