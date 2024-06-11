using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
namespace Cargold.Example {
#if Ads_Unity_C
public class 광고_유니티 : Cargold.SDK.Ads.AdsSystem_UnityAds
{
    public override bool IsAdsRemove_Func()
    {
        return false;
    }
}
#endif
} // End