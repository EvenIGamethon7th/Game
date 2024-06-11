using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
 
namespace Cargold
{
    public enum LogicType
    {
        None = 0,

        AND = 10,

        OR = 20,
    }

    public static partial class Extention_C
    {
        public static bool IsLogicallyTrue_Func<T>(this T[] _arr, LogicType _logicType) where T : ILogic_C
        {
            if (_logicType == LogicType.AND)
            {
                foreach (var _item in _arr)
                {
                    if (_item.IsLogicallyTrue_Func() == false)
                        return false;
                }

                return true;
            }
            else if(_logicType == LogicType.OR)
            {
                foreach (var _item in _arr)
                {
                    if (_item.IsLogicallyTrue_Func() == true)
                        return true;
                }

                return false;
            }
            else
            {
                Debug_C.Error_Func("_logicType : " + _logicType);
                return false;
            }
        }
    }

    public interface ILogic_C
    {
        bool IsLogicallyTrue_Func();
    }
}