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
     [LabelText("그룹(첫번째 이미지)")] public int Group;
     [LabelText("그룹내순서")] public int Order;
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
        Group = _cellDataArr[1].ToInt();
        Order = _cellDataArr[2].ToInt();
        Image = _cellDataArr[3];
        Description = _cellDataArr[4];
        Duration = _cellDataArr[5].ToFloat();
        Interval = _cellDataArr[6].ToFloat();
        Speaker = _cellDataArr[7];
        IsChoice = _cellDataArr[8].ToBool();
        shadow = _cellDataArr[9].ToFloat();
        sound = _cellDataArr[10];
    }
#endif
}