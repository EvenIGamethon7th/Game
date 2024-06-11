using Cargold;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.FrameWork
{
    [System.Serializable]
    public class Localize_C : DataGroupTemplate
    {
        public const string Str = "Localize";
        public const string KrStr = "로컬라이즈";

        public override string GetTypeNameStr => Localize_C.Str;

        public string GetLcz_Func(string _lczID, SystemLanguage _languageType)
        {
            return StringBuilder_C.Append_Func("재정의를 해주세요) ", _lczID);
        }
    }
}