using System.ComponentModel;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace Cargold.FrameWork
{
    public class SceneLoadManager : Singleton<SceneLoadManager>
    {
        [SerializeField]
        private Animator mSceneLoadAnimator;
        public void SceneChange(string sceneName)
        {
            LoadSceneAsync(sceneName).Forget();
        }
    
        private async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            mSceneLoadAnimator.gameObject.SetActive(true);
            mSceneLoadAnimator.Play(0);
            await UniTask.WaitForSeconds(2f);
            SceneManager.LoadScene("LoadingScene");
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            await UniTask.WaitUntil(() => asyncOperation.isDone);
            SceneManager.LoadScene(sceneName);
            mSceneLoadAnimator.gameObject.SetActive(false);
        }
    
        protected override void ChangeSceneInit(Scene prev, Scene next)
        {
        
        }
    }
}