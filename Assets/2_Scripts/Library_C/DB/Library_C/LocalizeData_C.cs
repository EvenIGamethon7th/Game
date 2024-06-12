using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

[System.Serializable]
public partial class LocalizeData : Data_C, Cargold.FrameWork.ILczData
{
     public string Key;
     [LabelText("한글")] public string ko;
     [LabelText("영어")] public string en;
     [LabelText("일본어")] public string ja;

    
            string Cargold.FrameWork.ILczData.GetLczStr_Func(SystemLanguage _langType)
            {
                switch (_langType)
                {
                    default:
                    case SystemLanguage.Korean:            return ko;

                    case SystemLanguage.English:            return en;
                    case SystemLanguage.Japanese:            return ja;
                    /*3*/
                    /*4*/
                    /*5*/
                    /*6*/
                    /*7*/
                    /*8*/
                    /*9*/
                    /*10*/
                    /*11*/
                    /*12*/
                    /*13*/
                    /*14*/
                    /*15*/
                    /*16*/
                    /*17*/
                    /*18*/
                    /*19*/
                    /*20*/
                    /*21*/
                    /*22*/
                    /*23*/
                    /*24*/
                    /*25*/
                    /*26*/
                    /*27*/
                    /*28*/
                    /*29*/
                    /*30*/

                    case SystemLanguage.Unknown:
                        Debug_C.Error_Func("다음 언어는 지원하지 않습니다. : " + _langType);
                        return default;
                }
            }
            

#if UNITY_EDITOR
    public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
    {
        Key = _cellDataArr[0];
        ko = _cellDataArr[1];
        en = _cellDataArr[2];
        ja = _cellDataArr[3];
    }
#endif
}