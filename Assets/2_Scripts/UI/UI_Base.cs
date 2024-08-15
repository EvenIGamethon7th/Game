using _2_Scripts.Game.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Base : MonoBehaviour
{
    [SerializeField]
    protected CameraManager.ECameraType mRenderCamera;
    
    protected void Start()
    {
        GetComponent<Canvas>().worldCamera = CameraManager.Instance.GetCamera(mRenderCamera);
        StartInit();
    }

    protected virtual void StartInit()
    {
        
    }

    public void ButtonSound()
    {
        SoundManager.Instance.Play2DSound(AddressableTable.Sound_Button);
    }
}
