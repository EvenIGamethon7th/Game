using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
namespace Cargold.Example {
public class 토스트매니저UI : Cargold.UI.UI_Toast_Manager
{
    public static new 토스트매니저UI Instance;

    public override void Init_Func(int _layer)
    {
        base.Init_Func(_layer);

        if (_layer == 0)
        {
            Instance = this;
        }
    }

    protected override string GetLczStr_AdsFail_Func() => "!광고 실패...";
    protected override string GetLczStr_BtnContinuousAlarm_Func() => "!버튼을 꾹 눌러도 됩니다.";
    protected override string GetLczStr_PurchaseFail_Func() => "!결제 실패...";
    protected override string GetLczStr_AdsRemove_Func() => "!광고 제거";
    protected override string GetLczStr_SameLangTypeSelected_Func() => "!동일한 언어를 선택했습니다.";
}
} // End