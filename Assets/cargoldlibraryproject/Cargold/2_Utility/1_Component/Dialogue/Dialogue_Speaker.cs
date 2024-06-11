using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Dialogue
{
    public abstract class Dialogue_Speaker : MonoBehaviour
    {
        public virtual void Init_Func()
        {
            this.Deactivate_Func(true);
        }

        public virtual void Activate_Func(string _speakerAniTrigger)
        {
            this.gameObject.SetActive(true);
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }

            this.gameObject.SetActive(false);
        }
    }

    public enum SpeakerAniType
    {
        None = 0,

        Idle,
    }
}