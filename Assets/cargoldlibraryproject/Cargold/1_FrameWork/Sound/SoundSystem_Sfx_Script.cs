using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Cargold.FrameWork.Sound_C;

namespace Cargold.FrameWork
{
    public class SoundSystem_Sfx_Script : SoundSystem_Script
{
    public void Activate_Func(SfxData _sfxData)
    {
        base.audioSource.volume = _sfxData.volume;
        base.audioSource.PlayOneShot(_sfxData.clip);
    }

    public override void Deactivate_Func(bool _isInit = false)
    {
        base.audioSource.volume = 0f;
    }
}
}
