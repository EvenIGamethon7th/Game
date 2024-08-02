// using _2_Scripts.Utils;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UniRx.Triggers;
// using UniRx;
// using UnityEngine;
// using _2_Scripts.UI;
// using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;
// using System.Linq;
//
// public class RTToPNG : MonoBehaviour
// {
//     [SerializeField]
//     RenderTexture texture;
//     [SerializeField]
//     string mFileName;
//
//     [SerializeField]
//     int mWidth = 200;
//     [SerializeField]
//     int mHeight = 240;
//
//     WaitForEndOfFrame _wof = new WaitForEndOfFrame();
//
//     private List<CharacterInfo> mUserCharacterList = new List<CharacterInfo>();
//
//     [SerializeField]
//     List<UI_UnitButton> mCharacters = new List<UI_UnitButton>();
//
//     UI_UnitButton mCurrentCharacter;
//
//     [SerializeField]
//     RectTransform mCanvas;
//
//     private bool isInitList = false;
//     private bool isInitButton = false;
//
//     private void Start()
//     {
//         MessageBroker.Default.Receive<TaskMessage>()
//                 .Where(message => message.Task == ETaskList.CharacterDataResourceLoad).Subscribe(
//                     _ =>
//                     {
//                         foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is CharacterInfo))
//                         {
//                             mUserCharacterList.Add(resource.Value as CharacterInfo);
//                         }
//                         isInitList = true;
//                     }).AddTo(this);
//
//         MessageBroker.Default.Receive<TaskMessage>()
//             .Where(message => message.Task == ETaskList.DefaultResourceLoad).Subscribe(
//                 _ =>
//                 {
//                     isInitButton = true;
//                 }).AddTo(this);
//
//         IDisposable dispose = null;
//
//         dispose = this.UpdateAsObservable().Where(_ => isInitButton && isInitList).Subscribe(
//             _ =>
//             {
//                 foreach (var character in mUserCharacterList)
//                 {
//                     foreach (var data in character.CharacterEvolutions)
//                     {
//                         var btn = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_EditUnitButton, Vector2.zero).GetComponent<UI_UnitButton>();
//                         mCharacters.Add(btn);
//                         var rt = btn.GetComponent<RectTransform>();
//                         rt.parent = mCanvas;
//                         rt.localScale = Vector3.one;
//                         rt.localPosition = Vector3.zero;
//                         rt.anchoredPosition = new Vector2(0, 960);
//                         rt.sizeDelta = new Vector2(128, 128);
//
//                         btn.UpdateGraphic(data.Value.GetData);
//                         //btn.gameObject.SetActive(false);
//                     }
//                 }
//
//                 for (int i = 0; i <  mCharacters.Count; i++)
//                 {
//                     mCharacters[i].gameObject.SetActive(false);
//                 }
//
//                 dispose.Dispose();
//             }).AddTo(this);
//     }
//
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.W))
//             StartCoroutine(Save());
//     }
//
//     public void SetRenderTextureSize(int width, int height)
//     {
//         texture.width *= width;
//         texture.height *= height;
//     }
//
//     IEnumerator Save()
//     {
//         for (int i = 0; i < mCharacters.Count; ++i)
//         {
//             int num = i;
//             yield return _wof;
//             Debug.Log("Start");
//             mFileName = num.ToString(); 
//             mCurrentCharacter?.gameObject.SetActive(false);
//             mCurrentCharacter = mCharacters[i];
//             mCurrentCharacter.gameObject.SetActive(true);
//         
//             Texture2D tex = new Texture2D(texture.width, texture.height);
//             tex.alphaIsTransparency = true;
//             RenderTexture.active = texture;
//             tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
//             tex.Apply();
//             TextureBilinear(tex);
//             Debug.Log("End");
//         }
//     }
//
//     void TextureBilinear(Texture2D tex)
//     {
//         Texture2D modifyTex = new Texture2D(mWidth, mHeight, TextureFormat.BGRA32, false);
//         modifyTex.alphaIsTransparency = true;
//         modifyTex.filterMode = FilterMode.Point;
//         Color col;
//
//         int mWidthBound = mWidth >> 1;
//         int mHeightBound = mHeight >> 1;
//         int mWidthMin = (tex.width >> 1) - mWidthBound;
//         int mHeightMin = (tex.height >> 1) - mHeightBound;
//
//         for (int i = mWidthMin; i < mWidthMin + mWidth; ++i)
//         {
//             for (int j = mHeightMin; j < mHeightMin + mHeight; ++j)
//             {
//                 col = tex.GetPixel(i, j);
//
//                 modifyTex.SetPixel(i - mWidthMin, j - mHeightMin, col);
//             }
//         }
//
//         modifyTex.Apply();
//
//         System.IO.File.WriteAllBytes(Application.dataPath + $"/1_Scenes/Temp/{mFileName}.PNG", modifyTex.EncodeToPNG());
//     }
// }
