using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Utils
{
    public enum EEditMessage
    {

    }

    public struct EditMessage<T1, T2>
    {
        public T1 Value1 { get; private set; }
        public T2 Value2 { get; private set; }

        public EditMessage(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}