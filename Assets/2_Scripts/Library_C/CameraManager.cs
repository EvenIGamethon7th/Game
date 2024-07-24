using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : Singleton<CameraManager>
{
    public enum ECameraType
    {
        Main,
        FX,
        UI
    }

    [SerializeField]
    private Camera MainCamera;

    [SerializeField]
    private Camera FXCamera;

    [SerializeField]
    private Camera UICamera;

    private List<Camera> mCameras = new List<Camera>();

    protected override void Awake()
    {
        base.Awake();
        mCameras.Add(MainCamera);
        mCameras.Add(FXCamera);
        mCameras.Add(UICamera);
    }

    protected override void ChangeSceneInit(Scene prev, Scene next)
    {
        
    }

    public Camera GetCamera(ECameraType type)
    {
        return mCameras[(int)type];
    }

    public void DoShake(ECameraType type, float time = 1, float amount = 0.1f, bool isUnscale = false)
    {
        global::Utils.DoShake(mCameras[(int)type].transform, isUnscale: isUnscale);
    }
}