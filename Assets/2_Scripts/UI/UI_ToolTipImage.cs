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
        private GraphicPropertyOverrideTexture mTex;
        [SerializeField]
        private TextMeshProUGUI mText;

        private StringBuilder mTextBuffer = new StringBuilder("Loading...");

        private void OnEnable()
        {
            mTex.PropertyValue = mCharacterImages[Random.Range(0, mCharacterImages.Count)];
            TextAsync().Forget();
        }

        private async UniTask TextAsync()
        {
            int count = 1;

            while (gameObject.activeSelf)
            {
                mText.text = mTextBuffer.ToString(0, count);
                await UniTask.DelayFrame(12);
                count = (count + 1) % mTextBuffer.Length;
            }
            mText.gameObject.SetActive(false);
        }
    }
}