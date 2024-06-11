using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using UnityEditor;

public class TextureCompression_Manager
{
#if UNITY_EDITOR
    public const string path = "Assets/Textures/Area";
    public static float[] compressionOriginValueArr = new float[] { 32f, 64f, 128f, 256f, 512f, 1024f };

    [UnityEditor.MenuItem("Cargold/Texture Origin")]
    public static void OnTextureOrigin_Func()
    {
        Debug.Log("Start");

        List<Texture> _assetList = Editor_C.GetLoadAssetListAtPath_Func<Texture>(path);
        foreach (Texture _asset in _assetList)
        {
            string _path = AssetDatabase.GetAssetPath(_asset);

            TextureImporter _textureImporter = AssetImporter.GetAtPath(_path) as TextureImporter;
            TextureImporterSettings _textureImporterSettings = new TextureImporterSettings();

            _textureImporter.ReadTextureSettings(_textureImporterSettings);
            _textureImporter.maxTextureSize = 2048;
            _textureImporter.SetTextureSettings(_textureImporterSettings);

            AssetDatabase.WriteImportSettingsIfDirty(_path);
            AssetDatabase.ImportAsset(_path, ImportAssetOptions.ForceUpdate);
        }

        Debug.Log("Done : " + _assetList.Count);
    }

    [UnityEditor.MenuItem("Cargold/TextureCompression")]
    public static void OnTextureCompression_Func()
    {
        Debug.Log("Start");

        List<Texture> _beforeAssetList = Editor_C.GetLoadAssetListAtPath_Func<Texture>(path, new string[] { "Before" });
        SetCompression(_beforeAssetList, .15f);

        Debug.Log("Before Done " + _beforeAssetList.Count);

        List<Texture> _afterAssetList = Editor_C.GetLoadAssetListAtPath_Func<Texture>(path, new string[] { "After" });
        SetCompression(_beforeAssetList, .30f);

        Debug.Log("After Done " + _afterAssetList.Count);

        void SetCompression(List<Texture> _assetList, float _compressionPer)
        {
            float[] _compressionAfterValueArr = new float[compressionOriginValueArr.Length];
            for (int i = 0; i < compressionOriginValueArr.Length; i++)
            {
                _compressionAfterValueArr[i] = compressionOriginValueArr[i] * (1f + _compressionPer);

                Debug.Log("압축 허용치 : " + _compressionAfterValueArr[i] + " / 압축 결과치 : " + compressionOriginValueArr[i]);
            }

            foreach (Texture _asset in _assetList)
            {
                int _sizeID = -1;
                int _biggerSize = _asset.width < _asset.height ? _asset.height : _asset.width;

                for (int i = 0; i < compressionOriginValueArr.Length; i++)
                {
                    if (compressionOriginValueArr[i] < _biggerSize && _biggerSize <= _compressionAfterValueArr[i])
                    {
                        _sizeID = i;
                        Debug.Log("이미지 : " + _asset.name + " / 사이즈 : " + _biggerSize + " / 압축 : " + compressionOriginValueArr[i]);
                        break;
                    }
                }

                if (0 <= _sizeID)
                {
                    string _path = AssetDatabase.GetAssetPath(_asset);

                    TextureImporter _textureImporter = AssetImporter.GetAtPath(_path) as TextureImporter;
                    TextureImporterSettings _textureImporterSettings = new TextureImporterSettings();

                    _textureImporter.ReadTextureSettings(_textureImporterSettings);
                    _textureImporter.maxTextureSize = compressionOriginValueArr[_sizeID].ToInt();
                    _textureImporter.SetTextureSettings(_textureImporterSettings);

                    AssetDatabase.WriteImportSettingsIfDirty(_path);
                    AssetDatabase.ImportAsset(_path, ImportAssetOptions.ForceUpdate);
                }
            }
        }
    } 
#endif
}