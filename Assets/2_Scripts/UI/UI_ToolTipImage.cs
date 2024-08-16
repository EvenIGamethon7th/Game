using Cysharp.Threading.Tasks;
using Plugins.Animate_UI_Materials;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_ToolTipImage : MonoBehaviour
    {
        [SerializeField]
        private List<Texture2D> mCharacterImages;
        [SerializeField]
        private Material mMaterial;
        [SerializeField]
        private TextMeshProUGUI mText;

        private StringBuilder mTextBuffer = new StringBuilder("Loading...");

        public async UniTask ActiveAsync(float time = 0.7f)
        {
            gameObject.SetActive(true);
            mMaterial.SetTexture("_Texture", mCharacterImages[Random.Range(0, mCharacterImages.Count)]);
            float originTime = time;
            float temp;
            mMaterial.SetFloat("_Transtion", 1);
            while (time > 0)
            {
                await UniTask.DelayFrame(1);
                time -= Time.deltaTime;
                temp = time / originTime;
                mMaterial.SetFloat("_Transtion", temp);
            }
            mText.gameObject.SetActive(true);
            TextAsync().Forget();
        }

        public async UniTask DisactiveAsync(float time = 0.7f)
        {
            mText.gameObject.SetActive(false);
            float originTime = time;
            mMaterial.SetFloat("_Transtion", 0);
            while (time > 0)
            {
                await UniTask.DelayFrame(1);
                time -= Time.deltaTime;
                mMaterial.SetFloat("_Transtion", 1f - time / originTime);
            }
            gameObject.SetActive(false);
        }

        private async UniTask TextAsync()
        {
            int count = 1;

            while (gameObject.activeInHierarchy)
            {
                mText.text = mTextBuffer.ToString(0, count);
                await UniTask.DelayFrame(12);
                count = (count + 1) % mTextBuffer.Length;
            }
            mText.gameObject.SetActive(false);
        }
    }
}