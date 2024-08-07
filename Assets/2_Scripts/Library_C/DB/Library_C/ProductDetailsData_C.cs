using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class ProductDetailsData : Data_C
{
     public string Key;
     [LabelText("표시이름")] public string name;
     [LabelText("표시설명")] public string desc;
     [LabelText("아이콘이미지")] public Sprite Icon;

    

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        name = _cellDataArr[1];
        desc = _cellDataArr[2];
        Icon = Editor_C.GetLoadAssetAtPath_Func<Sprite>(_cellDataArr[3]);
    }
#endif
}