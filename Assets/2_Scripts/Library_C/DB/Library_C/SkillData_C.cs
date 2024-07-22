using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class SkillData : Data_C
{
     public string Key;
     [LabelText("그룹")] public int Group;
     [LabelText("스킬명")] public string Name;
     [LabelText("설명")] public string Description;
     [LabelText("쿨타임")] public int CoolTime;
     [LabelText("패시브/액티브")] public ESkillType SkillType;
     [LabelText("등급")] public int Grade;
     [LabelText("스킬아이콘")] public Sprite icon;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Group = _cellDataArr[1].ToInt();
        Name = _cellDataArr[2];
        Description = _cellDataArr[3];
        CoolTime = _cellDataArr[4].ToInt();
        SkillType = _cellDataArr[5].ToEnum<ESkillType>();
        Grade = _cellDataArr[6].ToInt();
        icon = Editor_C.GetLoadAssetAtPath_Func<Sprite>(_cellDataArr[7]);
    }
#endif
}