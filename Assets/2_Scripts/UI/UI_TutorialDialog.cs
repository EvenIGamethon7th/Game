using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_TutorialDialog : MonoBehaviour
    {
        private float mPrevSpeed;
        private CancellationTokenSource mCts = new CancellationTokenSource();

        private void OnEnable()
        {
            mPrevSpeed = Time.timeScale;
            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            Time.timeScale = mPrevSpeed;
        }
    }
}