using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill
{
    public class LightLine : MonoBehaviour
    {
        private LineRenderer mLine;
        private Material mMaterial;
        private CancellationTokenSource mCts = new CancellationTokenSource();

        private readonly string Name = "_Duration";

        private void Awake()
        {
            mLine = GetComponent<LineRenderer>();
            mMaterial = mLine.material;

            gameObject.SetActive(false);
        }

        public async UniTask LightingTransition(Vector3 start, Vector3 end, float time)
        {
            float curTime = 0;
            mLine.SetPosition(0, start);
            mLine.SetPosition(1, end);

            while (curTime < time)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCts.Token);
                curTime += UnityEngine.Time.deltaTime;
                mMaterial.SetFloat(Name, curTime / time);
            }

            Clear();
        }

        private void Clear()
        {
            if (mCts != null && mCts.IsCancellationRequested)
            {
                mCts.Dispose();
                mCts = null;
                mCts = new CancellationTokenSource();
            }
            gameObject.SetActive(false);
            mMaterial.SetFloat(Name, 0);
        }
    }
}