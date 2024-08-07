using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class ItemData : Data_C
{
     public string Key;
     [LabelText("표시할 이름")] public string name;
     [LabelText("지급할 아이템 코드")] public string code;
     [LabelText("아이콘이미지")] public Sprite Icon;
     [LabelText("종류")] public ERewardType type;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        name = _cellDataArr[1];
        code = _cellDataArr[2];
        Icon = Editor_C.GetLoadAssetAtPath_Func<Sprite>(_cellDataArr[3]);
        type = _cellDataArr[4].ToEnum<ERewardType>();
    }
#endif
}