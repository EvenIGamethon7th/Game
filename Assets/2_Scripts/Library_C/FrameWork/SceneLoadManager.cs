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
    private GameObject mText;

    [SerializeField]
    private GraphicPropertyOverrideFloat mGraphicMaterialOverride;

    [SerializeField]
    private float mSpeed = 3;

    public event Action OnSceneLoad;
    public event Action SceneClear;

    public void SceneChange(string sceneName)
    {
        BGMManager.Instance.StopSound(true);
        LoadSceneAsync(sceneName).Forget();
    }

    private async UniTaskVoid LoadSceneAsync(string sceneName)
    {
        mSceneLoadAnimator.SetBool("Fade", false);
        mSceneLoadAnimator.gameObject.SetActive(true);
        mSceneLoadAnimator.Play(0);
        mSceneLoadAnimator.speed = mSpeed;
        await UniTask.WaitUntil(() => mGraphicMaterialOverride.PropertyValue <= 0f);
        SceneClear?.Invoke();
        mText.SetActive(true);
        Time.timeScale = 1;
       
        SceneManager.LoadScene("TempScene");

        await UniTask.WaitForSeconds(2f);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        await UniTask.WaitUntil(() => asyncOperation.progress >= 0.9f);
        mGraphicMaterialOverride.PropertyValue = 0f;
        await UniTask.WaitForSeconds(1f);
        mText.SetActive(false);
        mSceneLoadAnimator.SetBool("Fade", true);
        asyncOperation.allowSceneActivation = true;

        await UniTask.WaitUntil(() => mGraphicMaterialOverride.PropertyValue >= 0.99);

        mSceneLoadAnimator.gameObject.SetActive(false);

        OnSceneLoad?.Invoke();
    }
}
