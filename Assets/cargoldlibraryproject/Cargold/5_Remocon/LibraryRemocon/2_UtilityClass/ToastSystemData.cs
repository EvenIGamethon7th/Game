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
            [InlineProperty, HideLabel] public ToastSystemData toastSystemData = new ToastSystemData();

            [FoldoutGroup(ToastSystemData.KorStr), Indent(UtilityClassData.IndentLv)]
            public partial class ToastSystemData : ScriptGenerate // 메인
            {
                public const string KorStr = "토스트";
                public const string Str = "Toast";
                public const int IndentLv = UtilityClassData.IndentLv + 1;

                public static ToastSystemData Instance => UtilityClassData.Instance.toastSystemData;

                protected override string GetClassNameDefault => "UI_Toast_Manager";
                protected override Type GetExampleType => typeof(Cargold.Example.토스트매니저UI);
                protected override bool IsActionAfterCompiled => true;

#if UNITY_EDITOR
                protected override void CallEdit_GenerateDone_Func()
                {
                    base.CallEdit_GenerateDone_Func();

                    AnimationClip _clip = base.CallEdit_Duplicate_Func<AnimationClip>(Editor_C.AssetType.AnimationClip, "Toast");
                    GameObject _uiToastObj = base.CallEdit_Duplicate_Func<GameObject>(Editor_C.AssetType.Prefab, "UI_Toast");
                    UI.UI_BaseToast_Script _toastClass = _uiToastObj.GetComponent<UI.UI_BaseToast_Script>();
                    _toastClass.CallEdit_SetAni_Func(_clip);

                    string _poolingLog = null;
                    Cargold.FrameWork.PoolingSystem_Manager _psm = GameObject.FindObjectOfType<Cargold.FrameWork.PoolingSystem_Manager>();
                    if (_psm != null)
                    {
                        _poolingLog = "풀링 매니저에 토스트UI가 자동으로 등록되었습니다.";
                        _psm.CallEdit_SetToast_Func(_toastClass);
                    }
                    else
                    {
                        _poolingLog = "풀링 매니저를 찾을 수 없으므로 수동으로 토스트UI를 등록해주시기 바랍니다.";
                    }

                    Debug_C.Log_Func(_poolingLog);
                }
#endif
            }
        }
    }
}