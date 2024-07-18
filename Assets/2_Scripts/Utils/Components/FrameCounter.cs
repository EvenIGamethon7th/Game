using TMPro;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{
    private float deltaTime = 0f;

    [SerializeField] private int size = 25;
    [SerializeField] private Color color = Color.red;
    [SerializeField] private TextMeshProUGUI text;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float ms = deltaTime * 1000f;
        float fps = 1.0f / deltaTime;
         text.text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);
    }
    
}