using System;
using UnityEngine;

namespace _2_Scripts.Utils.Components
{
    public class AnimationEndToDisable : MonoBehaviour
    {
        private Animator mAnimator;
        private bool mIsPlaying = false;
        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
        }

        public float GetClipLength()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).length;
        }

        private void OnEnable()
        {
             mAnimator.Play(0,-1,0);
             mIsPlaying = true;
        }
        private void Update()
        {
            if (mIsPlaying && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                mIsPlaying = false;
                if (transform.parent != null)
                {
                    transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(false);
                }
         
            }
            
            
        }
    }
}