using UnityEngine;
using TMPro;

public class SafeController : MonoBehaviour
{
    [Header("Setari Cod")]
    public string correctCode = "1234";
    private string currentInput = "";
    public bool isLocked = true;

    [Header("Referinte UI")]
    public TextMeshProUGUI displayPing;
    public GameObject hintUI;
    public TextMeshProUGUI instructionsUI;

    [Header("Referinte Jucator")]
    public Transform player;
    public float interactionDistance = 2.5f;

    [Header("Referinte Sertar")]
    public GameObject drawer;
    public float openZPosition = 0.421f;
    private bool isOpening = false;

    [Header("Referinte Cheie")]
    public GameObject key;
    public float keyOpenYPosition = 0.5f;

    private bool isInWriteMode = false;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance && isLocked)
        {
            if (!isInWriteMode) hintUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                EnterWriteMode();
            }
        }
        else
        {
            hintUI.SetActive(false);
            if (isInWriteMode && distance > interactionDistance + 1f) ExitWriteMode();
        }

        if (isInWriteMode)
        {
            ListenForKeypad();
        }

        if (isOpening)
        {
            // Miscarea sertarului pe Z
            Vector3 drawerTarget = new Vector3(drawer.transform.localPosition.x, drawer.transform.localPosition.y, openZPosition);
            drawer.transform.localPosition = Vector3.Lerp(drawer.transform.localPosition, drawerTarget, Time.deltaTime * 2f);

            // Miscarea cheii pe Y
            if (key != null)
            {
                Vector3 keyTarget = new Vector3(key.transform.localPosition.x, keyOpenYPosition, key.transform.localPosition.z);
                key.transform.localPosition = Vector3.Lerp(key.transform.localPosition, keyTarget, Time.deltaTime * 2f);
            }
        }
    }

    void EnterWriteMode()
    {
        isInWriteMode = true;
        hintUI.SetActive(false);

        if (displayPing != null) 
        {
            displayPing.gameObject.SetActive(true);
            UpdateDisplay();
        }
        if (instructionsUI != null)
        {
            instructionsUI.gameObject.SetActive(true);
        }

        Debug.Log("Mod scris activat. Tasteaza cifrele!");
    }

    void ExitWriteMode()
    {
        isInWriteMode = false;
        currentInput = "";

        if (displayPing != null) displayPing.gameObject.SetActive(false);
        if (instructionsUI != null) instructionsUI.gameObject.SetActive(false);
    }

    void ListenForKeypad()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                AddDigit(i.ToString());
            }
        }
    }

    public void AddDigit(string digit)
    {
        if (currentInput.Length < correctCode.Length)
        {
            currentInput += digit;
            UpdateDisplay();
        }
        
        if (currentInput.Length == correctCode.Length) 
        {
            CheckCode();
        }
    }

    void CheckCode()
    {
        if (currentInput == correctCode)
        {
            displayPing.text = "OPEN";
            displayPing.color = Color.green;
            isOpening = true;
            isLocked = false;
            Invoke("ExitWriteMode", 1f);
        }
        else
        {
            displayPing.text = "Wrong code";
            displayPing.color = Color.red;
            Invoke("ClearScreen", 1f);
            Invoke("ExitWriteMode", 1f);
        }
    }

    void ClearScreen()
    {
        currentInput = "";
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (displayPing != null)
        {
            displayPing.color = Color.white;
            // Afișează _____ când nu s-a tastat nimic, altfel cifrele reale
            displayPing.text = string.IsNullOrEmpty(currentInput) ? "_____" : currentInput;
        }

        if (instructionsUI != null)
        {
            instructionsUI.text = string.IsNullOrEmpty(currentInput) ? "Insert the code" : "";
        }
    }

    void Start()
    {
        if (displayPing != null) displayPing.gameObject.SetActive(false);
        if (instructionsUI != null) instructionsUI.gameObject.SetActive(false);
    }
}