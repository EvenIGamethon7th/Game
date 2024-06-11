using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
namespace Cargold.Example
{
    public enum 재화타입
    {
        None = 0,
    }

    public struct 재화밸류
    {
        public static 재화밸류 operator +(재화밸류 inf, 재화밸류 value)
        {
            return default;
        }

        public static 재화밸류 operator -(재화밸류 inf, 재화밸류 value)
        {
            return default;
        }

        public static bool operator <=(재화밸류 left, 재화밸류 right)
        {
            return default;
        }
        public static bool operator >=(재화밸류 left, 재화밸류 right)
        {
            return default;
        }
    }
}