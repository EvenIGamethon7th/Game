using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.Dialogue;

namespace Cargold.Example {
    public class 다이얼로그시스템 : Cargold.Dialogue.DialogueSystem_Manager
    {
        public static new 다이얼로그시스템 Instance;

        public override void Init_Func(int _layer)
        {
            base.Init_Func(_layer);

            if (_layer == 0)
            {
                Instance = this;
            }
        }
    }
} // End