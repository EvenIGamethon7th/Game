using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.FrameWork
{
    [System.Serializable]
    public class Log_C : DataGroupTemplate
    {
        public const string KrStr = "로그";
        public const string Str = "Log";
        public override string GetTypeNameStr => Log_C.Str;

        [BoxGroup("플레이 타임"), LabelText("코루틴 간격(초)")] public float playTimeSecInterval = 10;
        [BoxGroup("플레이 타임"), LabelText("최대 시간(분)")] public int playTimeMinMax = 120;
        [BoxGroup("플레이 타임"), LabelText("기록 간격(분)")] public int playTimeMinInterval = 5;
    }
}