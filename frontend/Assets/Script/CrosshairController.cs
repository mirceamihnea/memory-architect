using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("Settings")]
    public bool lockCursor = true;
    public Color crosshairColor = Color.white;
    public float size = 5f;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnGUI()
    {
        // Deseneaza un punct mic in centrul ecranului
        float xMin = (Screen.width / 2) - (size / 2);
        float yMin = (Screen.height / 2) - (size / 2);
        
        Texture2D texture = Texture2D.whiteTexture;
        GUI.color = crosshairColor;
        GUI.DrawTexture(new Rect(xMin, yMin, size, size), texture);
    }

    void Update()
    {
        // Apasa ESC pentru a debloca mouse-ul
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Click stanga pentru a re-bloca mouse-ul
        if (Input.GetMouseButtonDown(0) && lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
