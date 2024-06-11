using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold
{
    public static class DateTime_C
    {
        public static DateTime GetToSec_Func(float _seconds)
        {
            return new DateTime().AddSeconds(_seconds);
        }
        public static DateTime GetToSec_Func(int _seconds)
        {
            return new DateTime().AddSeconds(_seconds);
        }
    } 
}