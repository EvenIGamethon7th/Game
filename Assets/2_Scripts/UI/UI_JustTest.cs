using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_JustTest : MonoBehaviour
{
    private Material mMat;
    private float mTime = 0;

    private void Awake()
    {
        mMat = GetComponent<Image>().material;
    }

    private void Update()
    {
        if (mTime > 2)
            mTime = 0;

        mTime += Time.deltaTime;

        mMat.SetFloat("_CurrentTime", mTime);
    }
}
