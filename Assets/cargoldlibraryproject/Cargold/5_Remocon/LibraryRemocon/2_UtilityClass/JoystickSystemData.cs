using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;
using Cargold.UI.Joystick;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class UtilityClassData
        {
            [InlineProperty, HideLabel] public JoystickSystemData joystickSystemData = new JoystickSystemData();

            [FoldoutGroup(JoystickSystemData.KorStr), Indent(UtilityClassData.IndentLv)]
            public partial class JoystickSystemData : ScriptGenerate
            {
                public const string KorStr = "조이스틱";
                public const string Str = "UI_Joystick";
                public const int IndentLv = UtilityClassData.IndentLv + 1;

                public static JoystickSystemData Instance => UtilityClassData.Instance.joystickSystemData;

                [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("컨트롤러 타입")]
                private UI_Joystick_Controller_Script.ControllerType controllerType = UI_Joystick_Controller_Script.ControllerType.InputSystem;
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "이름"), SerializeField, LabelText("Model")]
                private string uiClassNameStr_Model = "UI_Joystick_Model_Script";
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "이름"), SerializeField, LabelText("View")]
                private string uiClassNameStr_View = "UI_Joystick_View_Script";
                [FoldoutGroup(CargoldLibrary_C.OptionalS + "이름"), SerializeField, LabelText("Controller")]
                private string uiClassNameStr_Controller = "UI_Joystick_Controller_Script";

                public string GetClassName => this.GetClassNameDefault;
                public string GetUiClassNameStr_Model { get => uiClassNameStr_Model; }
                public string GetUiClassNameStr_View { get => uiClassNameStr_View; }
                public string GetUiClassNameStr_Controller { get => uiClassNameStr_Controller; }

                protected override string GetClassNameDefault => typeof(Cargold.UI.Joystick.UI_Joystick_Model_Script).Name;
                protected override Type GetExampleType => typeof(Cargold.Example.UI조이스틱_Model);

                public override void Init_Func()
                {
                    base.Init_Func();

                    base.subFolderPathArr = new string[1] { JoystickSystemData.Str };
                }

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, Func<string, string> _codeModifyDel, string _className = null, string _scriptName = null, params string[] _subFolderStrArr)
                {
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _className, _scriptName, _subFolderStrArr);

                    base.CallEdit_Generate_Func(typeof(Cargold.Example.UI조이스틱_Model), this.uiClassNameStr_Model);
                    base.CallEdit_Generate_Func(typeof(Cargold.Example.UI조이스틱_View), this.uiClassNameStr_View);

                    switch (this.controllerType)
                    {
                        case UI_Joystick_Controller_Script.ControllerType.InputSystem:
                            base.CallEdit_Generate_Func(typeof(Cargold.Example.UI조이스틱_Controller_InputSystem), this.uiClassNameStr_Controller);
                            break;

                        case UI_Joystick_Controller_Script.ControllerType.PointerHandler:
                            base.CallEdit_Generate_Func(typeof(Cargold.Example.UI조이스틱_Controller_PointerHandler), this.uiClassNameStr_Controller);
                            break;

                        case UI_Joystick_Controller_Script.ControllerType.None:
                        default:
                            break;
                    }

                    base.CallEdit_Duplicate_Func<PropertyAdapter_Joystick_View>(Editor_C.AssetType.Prefab, JoystickSystemData.Str, "Prefab");
                } 
#endif
            }
        }
    }
}