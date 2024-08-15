using System;
using System.ComponentModel;
using _2_Scripts.Game.Sound;
using _2_Scripts.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Plugins.Animate_UI_Materials;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    [SerializeField]
    private UI_ToolTipImage mToolTip;

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
        await mToolTip.ActiveAsync();

        SceneClear?.Invoke();
        Time.timeScale = 1;
        AsyncOperation op = SceneManager.LoadSceneAsync("TempScene");
        op.allowSceneActivation = true;
        await UniTask.WaitUntil(() => op.progress >= 0.9f);
        await UniTask.WaitForSeconds(2f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        await UniTask.WaitUntil(() => asyncOperation.progress >= 0.9f);
        
        await UniTask.WaitForSeconds(1f);

        asyncOperation.allowSceneActivation = true;

        await mToolTip.DisactiveAsync();

        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name.Equals(sceneName));

        OnSceneLoad?.Invoke();
    }
}
