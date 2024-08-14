using System;
using System.ComponentModel;
using _2_Scripts.Game.Sound;
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

    [SerializeField]
    private float mSpeed = 3;

    public event Action OnSceneLoad;
    public event Action SceneClear;

    private Sequence mFadeInSeq;
    private Sequence mFadeOutSeq;

    protected override void AwakeInit()
    {
        mFadeInSeq = DOTween.Sequence()
            .AppendCallback(() => { 
                mToolTipGroup.alpha = 0;
                mToolTipGroup.gameObject.SetActive(true);
            })
            .Append(mToolTipGroup.DOFade(1, 2))
            .Pause()
            .SetAutoKill(false);

        mFadeOutSeq = DOTween.Sequence()
            .Append(mToolTipGroup.DOFade(0, 2))
            .AppendCallback(() => mToolTipGroup.gameObject.SetActive(false))
            .Pause()
            .SetAutoKill(false);
    }

    public void SceneChange(string sceneName)
    {
        BGMManager.Instance.StopSound(true);
        LoadSceneAsync(sceneName).Forget();
    }

    private void ToolTipAnimation()
    {
        mToolTipGroup.alpha = 1;
        mFadeInSeq.Restart();
    }

    private void ToolTipAlpha()
    {
        mFadeOutSeq.Restart();
    }

    private async UniTaskVoid LoadSceneAsync(string sceneName)
    {
        mSceneLoadAnimator.SetBool("Fade", false);
        mSceneLoadAnimator.gameObject.SetActive(true);
        mSceneLoadAnimator.Play(0);
        mSceneLoadAnimator.speed = mSpeed;
        SceneClear?.Invoke();
        await UniTask.WaitUntil(() => mGraphicMaterialOverride.PropertyValue <= 0f);
        mToolTipGroup.gameObject.SetActive(true);

        Time.timeScale = 1;
       
        SceneManager.LoadScene("TempScene");

        await UniTask.WaitForSeconds(2f);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        await UniTask.WaitUntil(() => asyncOperation.progress >= 0.9f);
        mToolTipGroup.gameObject.SetActive(false);
        mGraphicMaterialOverride.PropertyValue = 0f;
        await UniTask.WaitForSeconds(1f);
        mSceneLoadAnimator.SetBool("Fade", true);
        asyncOperation.allowSceneActivation = true;

        await UniTask.WaitUntil(() => mGraphicMaterialOverride.PropertyValue >= 0.99);

        mSceneLoadAnimator.gameObject.SetActive(false);

        OnSceneLoad?.Invoke();
    }

    protected override void ChangeSceneInit(Scene prev, Scene next)
    {

    }
}
