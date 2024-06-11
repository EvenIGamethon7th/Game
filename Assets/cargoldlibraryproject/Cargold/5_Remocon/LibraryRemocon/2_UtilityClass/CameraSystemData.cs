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
            [InlineProperty, HideLabel] public CameraData cameraData = new CameraData();

            [FoldoutGroup(CameraData.KorStr), Indent(UtilityClassData.IndentLv)]
            public partial class CameraData : ScriptGenerate // 메인
            {
                public const string KorStr = "카메라";
                public const string Str = "Camera";
                public const int IndentLv = UtilityClassData.IndentLv + 1;

                public static CameraData Instance => UtilityClassData.Instance.cameraData;

                protected override string GetClassNameDefault => "CameraSystem_Manager";
                protected override Type GetExampleType => typeof(Cargold.Example.카메라시스템매니저);
            }
        }
    }
}