using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_AutoButton : Button
{
    private Material[] mMats;
    private float mOutlineThick;

    protected override void Awake()
    {
        base.Awake();
        var img = GetComponentsInChildren<Image>();
        mMats = new Material[img.Length];
        mOutlineThick = 0.01f;
        for (int i = 0; i <  img.Length; i++)
        {
            mMats[i] = img[i].material;
        }
    }

    public void IsAuto(bool isAuto)
    {
        for (int i = 0; i < mMats.Length; ++i)
        {
            mMats[i].SetFloat("_OutlineThickness", isAuto ? mOutlineThick : 0);
        }
    }
}
