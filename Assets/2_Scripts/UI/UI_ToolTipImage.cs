using Cysharp.Threading.Tasks;
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
        private List<Sprite> mCharacterImages;

        [SerializeField]
        private Image mImage;
        [SerializeField]
        private TextMeshProUGUI mText;

        private StringBuilder mTextBuffer = new StringBuilder("Loading...");

        private void OnEnable()
        {
            mImage.sprite = mCharacterImages[Random.Range(0, mCharacterImages.Count)];
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
        }
    }
}