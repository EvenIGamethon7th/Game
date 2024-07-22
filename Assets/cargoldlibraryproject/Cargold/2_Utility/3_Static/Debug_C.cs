using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold
{
    public static partial class Debug_C
    {
        private const string Colon = " : ";

        [SerializeField] private static IDebug_C iDebugC;

        public static void Init_Func(IDebug_C _iDebugC) => Debug_C.iDebugC = _iDebugC;

        public static bool IsLogType_Func(PrintLogType _logType)
        {
            if (Debug_C.iDebugC == null)
            {
// #if UNITY_EDITOR
//                 if (Application.isPlaying == false)
//                     return true;
// #endif

                return false;
            }
            else
            {
                return Debug_C.iDebugC.IsLogType_Func(_logType);
            }
        }

        [System.Diagnostics.Conditional("Test_Cargold")]
        public static void Log_Func(object _obj, PrintLogType _logType = PrintLogType.Common)
        {
            Log_Func(_obj.ToString(), _logType);
        }
        [System.Diagnostics.Conditional("Test_Cargold")]
        public static void Log_Func(string _str, PrintLogType _logType = PrintLogType.Common)
        {
#if UNITY_EDITOR
            if (IsLogType_Func(_logType) == true)
                Log();
#else
            Log();
#endif

            void Log()
            {
                _str = StringBuilder_C.Append_Func(_logType.ToString(), Colon, _str);
                Debug.Log(_str);
            }
        }

        [System.Diagnostics.Conditional("Test_Cargold")]
        public static void Warning_Func(object _obj, PrintLogType _logType = PrintLogType.Common)
        {
            Warning_Func(_obj.ToString(), _logType);
        }
        [System.Diagnostics.Conditional("Test_Cargold")]
        public static void Warning_Func(string _str, PrintLogType _logType = PrintLogType.Common)
        {
#if UNITY_EDITOR
            if (IsLogType_Func(_logType) == true)
                Log();
#else
            Log();
#endif

            void Log()
            {
                _str = StringBuilder_C.Append_Func(_logType.ToString(), Colon, _str);
                Debug.LogWarning(_str);
            }
        }

        [System.Diagnostics.Conditional("Test_Cargold")]
        public static void Error_Func(object _obj, PrintLogType _logType = PrintLogType.Common)
        {
            Error_Func(_obj.ToString(), _logType);
        }
        [System.Diagnostics.Conditional("Test_Cargold")]
        public static void Error_Func(string _str, PrintLogType _logType = PrintLogType.Common)
        {
#if UNITY_EDITOR
            if (IsLogType_Func(_logType) == true)
                Log();
#else
            Log();
#endif

            void Log()
            {
                _str = StringBuilder_C.Append_Func(_logType.ToString(), Colon, _str);
                Debug.LogError(_str);
            }
        }

        public interface IDebug_C
        {
            bool IsLogType_Func(PrintLogType _logType = PrintLogType.Common);
        }
    }
}