using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class StoryData : Data_C
{
     public string Key;
     [LabelText("챕터별 스토리")] public int Story_Chapter;
     [LabelText("그룹")] public int Story_Group;
     [LabelText("그룹내순서")] public int Story_Order;
     [LabelText("컷씬이미지")] public string Image;
     public string Description;
     [LabelText("대사 지속시간")] public float Duration;
     [LabelText("대사간 간격")] public float Interval;
     [LabelText("화자")] public string Speaker;
     [LabelText("선택지여부")] public bool IsChoice;
     [LabelText("특수효과")] public float shadow;
     [LabelText("효과음")] public string sound;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        Story_Chapter = _cellDataArr[1].ToInt();
        Story_Group = _cellDataArr[2].ToInt();
        Story_Order = _cellDataArr[3].ToInt();
        Image = _cellDataArr[4];
        Description = _cellDataArr[5];
        Duration = _cellDataArr[6].ToFloat();
        Interval = _cellDataArr[7].ToFloat();
        Speaker = _cellDataArr[8];
        IsChoice = _cellDataArr[9].ToBool();
        shadow = _cellDataArr[10].ToFloat();
        sound = _cellDataArr[11];
    }
#endif
}