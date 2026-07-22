using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    private int eConsumedFrame = -1;

    void Awake()
    {
        instance = this;
    }

    public bool GetEDown(string callerName)
    {
        if (Time.frameCount == eConsumedFrame) return false;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            eConsumedFrame = Time.frameCount;
            Debug.Log("Tasta E consumata de: " + callerName + " pe frame: " + Time.frameCount);
            return true;
        }
        return false;
    }


}