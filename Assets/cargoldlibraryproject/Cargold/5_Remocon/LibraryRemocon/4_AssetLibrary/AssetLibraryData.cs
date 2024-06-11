using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
 
namespace Cargold
{
    public partial class LibraryRemocon
    {
        [FoldoutGroup(AssetLibraryData.KorStr), Indent(AssetLibraryData.IndentLv)]
        public partial class AssetLibraryData // 메인
        {
            public const string KorStr = "외부에셋";
            public const int IndentLv = LibraryRemocon.IndentLv + 1;

            [InlineProperty, HideLabel] public AdvertiseData advertiseData = new AdvertiseData();
            [InlineProperty, HideLabel] public InappPurchaseData inappPurchaseData = new InappPurchaseData();
            [InlineProperty, HideLabel] public TrackingLogData trackingLogData = new TrackingLogData();

            public void Init_Func()
            {
                this.advertiseData.Init_Func();
                this.inappPurchaseData.Init_Func();
                this.trackingLogData.Init_Func();
            }
        }
    }
}