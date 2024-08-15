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
    private GameObject mSceneLoadAnimator;
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
        mSceneLoadAnimator.SetActive(true);
        mGraphicMaterialOverride.PropertyValue = 1;

        while (mGraphicMaterialOverride.PropertyValue > 0)
        {
            await UniTask.DelayFrame(1);
            mGraphicMaterialOverride.PropertyValue -= Time.unscaledDeltaTime;
        }

        SceneClear?.Invoke();
        mText.SetActive(true);
        Time.timeScale = 1;

        AsyncOperation op = SceneManager.LoadSceneAsync("TempScene");
        op.allowSceneActivation = true;

        await UniTask.WaitUntil(() => op.progress >= 0.9f);
        await UniTask.WaitUntil(()=> SceneManager.GetActiveScene().name == "TempScene");
        mGraphicMaterialOverride.PropertyValue = 0f;
        await UniTask.WaitForSeconds(2f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        await UniTask.WaitUntil(() => asyncOperation.progress >= 0.9f);
        
        await UniTask.WaitForSeconds(1f);
        mText.SetActive(false);

        asyncOperation.allowSceneActivation = true;

        while (mGraphicMaterialOverride.PropertyValue < 1)
        {
            await UniTask.DelayFrame(1);
            mGraphicMaterialOverride.PropertyValue += Time.unscaledDeltaTime;
        }

        mSceneLoadAnimator.SetActive(false);

        OnSceneLoad?.Invoke();
    }
}
