using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold
{
    public partial class LibraryRemocon 
    {
        [FoldoutGroup(UtilityClassData.KorStr), Indent(UtilityClassData.IndentLv)]
        public partial class UtilityClassData // 메인
        {
            public const string KorStr = "유틸리티 클래스";
            public const int IndentLv = LibraryRemocon.IndentLv + 1;

            public static UtilityClassData Instance => LibraryRemocon.Instance.utilityClassData;

            public void Init_Func()
            {
                this.stringDropdownData.Init_Func();
                this.dialogueData.Init_Func();
            }
        }
    }
}