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
        public partial class FrameWorkData
        {
            [FoldoutGroup(SoundSystemData.KorStr), Indent(FrameWorkData.IndentLv)]
            public partial class SoundSystemData : ScriptGenerate // 메인
            {
                public const string KorStr = "사운드 시스템";
                public const int IndentLv = FrameWorkData.IndentLv + 1;

                public static SoundSystemData Instance => LibraryRemocon.Instance.frameWorkData.soundSystemData;

                protected override bool GetIsEnableDefault => true;
                protected override string GetClassNameDefault => "SoundSystem_Manager";
                protected override Type GetExampleType => typeof(Cargold.Example.사운드시스템매니저);

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, Func<string, string> _codeModifyDel, string _className = null, string _scriptName = null, params string[] _subFolderStrArr)
                {
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _className, _scriptName, _subFolderStrArr: "FrameWork");

                    this.CallEdit_GenerateSfx_Func();
                    this.CallEdit_GenerateBgm_Func();
                }

                [Button("SFX StringDropdown 생성")]
                private void CallEdit_GenerateSfx_Func()
                {
                    UtilityClassData.StringDropdownData _stringDropdownData = LibraryRemocon.Instance.utilityClassData.stringDropdownData;

                    SobjDropdown.DropdownData _initData_Sfx = new SobjDropdown.DropdownData();
                    _initData_Sfx.uid = SfxType.UI_Normal;
                    _initData_Sfx.nameStr = nameof(SfxType.UI_Normal);

                    string _sfxTypeName = typeof(SfxType).Name;
                    _stringDropdownData.CallEdit_Generate_Func(_sfxTypeName, _initData_Sfx);
                }

                [Button("BGM StringDropdown 생성")]
                private void CallEdit_GenerateBgm_Func()
                {
                    UtilityClassData.StringDropdownData _stringDropdownData = LibraryRemocon.Instance.utilityClassData.stringDropdownData;

                    string _bgmTypeName = typeof(BgmType).Name;
                    _stringDropdownData.CallEdit_Generate_Func(_bgmTypeName);
                }
#endif
            }
        }
    }
}