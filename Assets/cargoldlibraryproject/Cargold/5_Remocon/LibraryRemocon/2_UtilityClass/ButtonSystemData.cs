using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

#if DoTween_C
namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class UtilityClassData
        {
            [InlineProperty, HideLabel] public ButtonSystemData buttonSystemData = new ButtonSystemData();

            [FoldoutGroup(ButtonSystemData.KorStr), Indent(UtilityClassData.IndentLv)]

            public class ButtonSystemData : ScriptGenerate
            {
                public const string KorStr = "버튼";
                public const string Str = "UI_Button";
                public const int IndentLv = UtilityClassData.IndentLv + 1;

                public static ButtonSystemData Instance => UtilityClassData.Instance.buttonSystemData;

                protected override string GetClassNameDefault => "UI_Button_Script";
                protected override Type GetExampleType => typeof(Cargold.Example.UI버튼);

                public override void Init_Func()
                {
                    base.Init_Func();

                    base.subFolderPathArr = new string[1] { ButtonSystemData.Str };
                }
            }
        }
    }
} 
#endif