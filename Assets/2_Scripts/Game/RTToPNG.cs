using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTToPNG : MonoBehaviour
{
    [SerializeField]
    RenderTexture texture;
    [SerializeField]
    string _fileName;

    [SerializeField]
    int _width = 200;
    [SerializeField]
    int _height = 240;

    WaitForEndOfFrame _wof = new WaitForEndOfFrame();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(Save());
    }

    public void SetRenderTextureSize(int width, int height)
    {
        texture.width *= width;
        texture.height *= height;
    }

    IEnumerator Save()
    {
        yield return _wof;
        Debug.Log("Start");
        Texture2D tex = new Texture2D(texture.width, texture.height);
        tex.alphaIsTransparency = true;
        RenderTexture.active = texture;
        tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        tex.Apply();
        TextureBilinear(tex);
        Debug.Log("End");
    }

    void TextureBilinear(Texture2D tex)
    {
        Texture2D modifyTex = new Texture2D(_width, _height, TextureFormat.BGRA32, false);
        modifyTex.alphaIsTransparency = true;
        modifyTex.filterMode = FilterMode.Point;
        Color col;

        for (int i = 440; i < 640; ++i)
        {
            for (int j = 840; j < 1080; ++j)
            {
                col = tex.GetPixel(i, j);

                modifyTex.SetPixel(i - 440, j - 840, col);

                //if (col.g > 0.2f && col.r < 0.1f && col.b < 0.1f)
                //{
                //    modifyTex.SetPixel(i - 440, j - 840, Color.clear);
                //}
                //else
                //{
                //    modifyTex.SetPixel(i - 440, j - 840, col);
                //}
            }
        }

        modifyTex.Apply();
        Debug.Log($"{tex.width} {tex.height}");

        System.IO.File.WriteAllBytes(Application.dataPath + $"/1_Scenes/{_fileName}.PNG", modifyTex.EncodeToPNG());
    }
}
