using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.Effect
{
    public class Effect_OneShot_Script : Effect_Script
    {
        public override EffectActivateType GetEffectActivateType => EffectActivateType.OneShot;

        protected override void ElementPlayDone_Func(Effect_Element_Script _doneElementClass)
        {
            bool _isDoneAll = true;
            foreach (Effect_Element_Script _elementClass in base.elementClassArr)
            {
                if (_elementClass.IsDone == false)
                {
                    _isDoneAll = false;
                    break;
                }
            }

            if (_isDoneAll == true)
            {
                base.Deactivate_Func();
            }
        }
    } 
}