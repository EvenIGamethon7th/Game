using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class UtilityClassData
        {
            [InlineProperty, HideLabel] public FocusSystemData focusSystemData = new FocusSystemData();

            [FoldoutGroup(FocusSystemData.KorStr), Indent(UtilityClassData.IndentLv)]
            public partial class FocusSystemData : ScriptGenerate // 메인
            {
                public const string KorStr = "포커스시스템";
                public const string Str = "FocusSystem";
                public const int IndentLv = UtilityClassData.IndentLv + 1;

                public static FocusSystemData Instance => UtilityClassData.Instance.focusSystemData;

                public string GetClassName => this.GetClassNameDefault;

                protected override string GetClassNameDefault => typeof(Cargold.UI.Focus.UI_Focus_Manager).Name;
                protected override Type GetExampleType => typeof(Cargold.Example.포커스UI);

                public override void Init_Func()
                {
                    base.Init_Func();

                    base.subFolderPathArr = new string[1] { FocusSystemData.Str };
                }

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, Func<string, string> _codeModifyDel, string _className = null, string _scriptName = null, params string[] _subFolderStrArr)
                {
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _className, _scriptName, _subFolderStrArr);

                    base.CallEdit_Duplicate_Func(Editor_C.AssetType.Prefab, FocusSystemData.Str, "Prefab");
                    base.CallEdit_Duplicate_Func(Editor_C.AssetType.AnimationClip, FocusSystemData.Str, "Main");
                    base.CallEdit_Duplicate_Func(Editor_C.AssetType.AnimationClip, FocusSystemData.Str, "Arrow");
                }
#endif
            }
        }
    }
}