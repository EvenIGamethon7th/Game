using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Base : MonoBehaviour
{
    [SerializeField]
    protected CameraManager.ECameraType mRenderCamera;
    
    protected void Start()
    {
        GetComponent<Canvas>().worldCamera = CameraManager.Instance.GetCamera(mRenderCamera);
        StartInit();
    }

    protected abstract void StartInit();
}
