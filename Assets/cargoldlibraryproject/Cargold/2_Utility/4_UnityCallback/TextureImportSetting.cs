using UnityEditor;

#if UNITY_EDITOR
public class TextureImportSetting : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        //TextureImporter textureImporter = (TextureImporter)assetImporter;
        // 스프라이트 세팅
        //textureImporter.textureType = TextureImporterType.Sprite;
        // 밈맵 비활성화
        //textureImporter.mipmapEnabled = false;

        //textureImporter.filterMode = UnityEngine.FilterMode.Point;

        //textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
    }
}
#endif