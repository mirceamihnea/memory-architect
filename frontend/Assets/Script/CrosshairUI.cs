using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    private Texture2D dotTexture;
    private GUIStyle dotStyle;

    void Start()
    {
        // Cream un dot alb de 6x6 pixeli
        dotTexture = new Texture2D(6, 6);
        Color[] pixels = new Color[36];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        dotTexture.SetPixels(pixels);
        dotTexture.Apply();

        dotStyle = new GUIStyle();
        dotStyle.normal.background = dotTexture;
    }

    void OnGUI()
    {
        float x = (Screen.width / 2f) - 3f;
        float y = (Screen.height / 2f) - 3f;
        GUI.Box(new Rect(x, y, 6, 6), GUIContent.none, dotStyle);
    }
}
