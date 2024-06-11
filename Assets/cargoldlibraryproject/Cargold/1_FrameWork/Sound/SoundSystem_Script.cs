using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
 
public abstract class SoundSystem_Script : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource = null;

    public void Init_Func()
    {
        this.Deactivate_Func(true);
    }

    public abstract void Deactivate_Func(bool _isInit = false);

    public void SetOn_Func(bool _isOn)
    {
        this.audioSource.mute = !_isOn;
    }

    [Button]
    private void CallEdit_Play_Func()
    {
        this.audioSource.Play();
    }

    [Button("캐싱 ㄱㄱ")]
    private void CallEdit_Catch_Func()
    {
        this.Reset();
    }

    private void Reset()
    {
        if (this.TryGetComponent<AudioSource>(out AudioSource _audioSource) == false)
            _audioSource = this.gameObject.AddComponent<AudioSource>();

        _audioSource.playOnAwake = false;

        this.audioSource = _audioSource;
    }
}