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
            [InlineProperty, HideLabel] public BackKeyData backKeyData = new BackKeyData();

            [FoldoutGroup(BackKeyData.KorStr), Indent(UtilityClassData.IndentLv)]
            public class BackKeyData : ScriptGenerate // 메인
            {
                public const string KorStr = "백키 시스템";
                public const string Str = "BackKey";
                public const int IndentLv = UtilityClassData.IndentLv + 1;

                public static BackKeyData Instance => UtilityClassData.Instance.backKeyData;

                public string GetClassName => this.GetClassNameDefault;

                protected override string GetClassNameDefault => typeof(Cargold.BackKeySystem.BackKey_Manager).Name;
                protected override Type GetExampleType => typeof(Cargold.Example.백키시스템);

                public override void Init_Func()
                {
                    base.Init_Func();

                    base.subFolderPathArr = new string[1] { BackKeyData.Str };
                }
            }
        }
    }
}