using System;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Plugins.Animate_UI_Materials;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

    public class SceneLoadManager : Singleton<SceneLoadManager>
    {
        [SerializeField]
        private Animator mSceneLoadAnimator;
        
        [SerializeField]
        private GraphicPropertyOverrideFloat mGraphicMaterialOverride;

        [SerializeField] 
        private CanvasGroup mToolTipGroup;




        public void SceneChange(string sceneName)
        {
            LoadSceneAsync(sceneName).Forget();
        }

        private void ToolTipAnimation()
        {
            mToolTipGroup.alpha = 0;
            mToolTipGroup.gameObject.SetActive(true);
            Sequence tooltipSequence = DOTween.Sequence();
            tooltipSequence.Append(mToolTipGroup.DOFade(1, 2));
            tooltipSequence.Join(mToolTipGroup.transform.DOMoveX(0, 2));
            tooltipSequence.SetAutoKill(false);
        }

        private void ToolTipAlpha()
        {
            mToolTipGroup.DOFade(0, 2)
                .OnComplete(() =>
                {
                    mToolTipGroup.gameObject.SetActive(false);
                });
        }
        private async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            mSceneLoadAnimator.SetBool("Fade",false);
            mSceneLoadAnimator.gameObject.SetActive(true);
            mSceneLoadAnimator.Play(0);
            
            await UniTask.WaitUntil(()=>mGraphicMaterialOverride.PropertyValue <= 0f);
            ToolTipAnimation();
            
            await UniTask.WaitForSeconds(2f);
            ToolTipAlpha();

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;
            await UniTask.WaitUntil(() => asyncOperation.progress >= 0.9f);
            mGraphicMaterialOverride.PropertyValue = 0f;
            await UniTask.WaitForFixedUpdate();
            asyncOperation.allowSceneActivation = true;
            mSceneLoadAnimator.SetBool("Fade",true);
         
            
            await  UniTask.WaitUntil(()=>mGraphicMaterialOverride.PropertyValue >= 0.99);
            mSceneLoadAnimator.gameObject.SetActive(false);
        }
    
        protected override void ChangeSceneInit(Scene prev, Scene next)
        {
        
        }
    }
