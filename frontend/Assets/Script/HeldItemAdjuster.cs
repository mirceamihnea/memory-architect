using UnityEngine;

/// <summary>
/// Tool de debug: ajusteaza in timp real pozitia, rotatia si scala item-ului tinut in mana.
/// Pune acest script pe acelasi obiect ca HeldItemController.
/// In Play Mode, modifica valorile din Inspector si apasa "Apply To ItemData" pentru a salva.
/// </summary>
public class HeldItemAdjuster : MonoBehaviour
{
    [Header("=== HELD ITEM ADJUSTER (Debug Tool) ===")]
    [Space(5)]
    public bool enableAdjustment = true;

    [Header("Pozitie in mana")]
    public Vector3 position = new Vector3(0.55f, -0.15f, 0.45f);

    [Header("Rotatie in mana")]
    public Vector3 rotation = new Vector3(15f, 280f, 0f);

    [Header("Scala in mana")]
    public Vector3 scale = new Vector3(1f, 1f, 1f);

    [Header("Pas de ajustare (pentru taste)")]
    public float positionStep = 0.05f;
    public float rotationStep = 5f;
    public float scaleStep = 0.1f;

    private HeldItemController heldItemController;
    private string lastItemName = "";

    void Start()
    {
        heldItemController = GetComponent<HeldItemController>();
    }

    void Update()
    {
        if (!enableAdjustment) return;
        if (heldItemController == null) return;

        // Detectam daca s-a schimbat itemul - incarcam valorile din ItemData
        string currentName = heldItemController.currentItemName ?? "";
        if (currentName != lastItemName)
        {
            lastItemName = currentName;
            LoadCurrentItemValues();
        }

        // Gasim obiectul tinut in mana
        Transform handPoint = heldItemController.handPoint;
        if (handPoint == null || handPoint.childCount == 0) return;

        Transform heldItem = handPoint.GetChild(handPoint.childCount - 1);

        // Ajustam cu taste (hold Left Shift + taste):
        // Shift + Arrow Keys = pozitie X/Z
        // Shift + Page Up/Down = pozitie Y
        // Shift + Numpad +/- = scala
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Pozitie
            if (Input.GetKeyDown(KeyCode.UpArrow))    position.z += positionStep;
            if (Input.GetKeyDown(KeyCode.DownArrow))  position.z -= positionStep;
            if (Input.GetKeyDown(KeyCode.LeftArrow))  position.x -= positionStep;
            if (Input.GetKeyDown(KeyCode.RightArrow)) position.x += positionStep;
            if (Input.GetKeyDown(KeyCode.PageUp))     position.y += positionStep;
            if (Input.GetKeyDown(KeyCode.PageDown))   position.y -= positionStep;

            // Scala
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                scale += Vector3.one * scaleStep;
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                scale -= Vector3.one * scaleStep;

            // Rotatie
            if (Input.GetKeyDown(KeyCode.Q)) rotation.y -= rotationStep;
            if (Input.GetKeyDown(KeyCode.R)) rotation.y += rotationStep;
        }

        // Aplicam valorile in timp real
        heldItem.localPosition = position;
        heldItem.localRotation = Quaternion.Euler(rotation);
        heldItem.localScale = scale;

        // Apasa F5 pentru a printa valorile in consola (copy-paste in ItemData)
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("<color=lime>=== HELD ITEM VALUES (copy to ItemData) ===</color>\n" +
                "Item: " + heldItemController.currentItemName + "\n" +
                "Held Position: " + position.ToString("F3") + "\n" +
                "Held Rotation: " + rotation.ToString("F3") + "\n" +
                "Held Scale: " + scale.ToString("F3"));
        }
    }

    void OnGUI()
    {
        if (!enableAdjustment) return;
        if (heldItemController == null || string.IsNullOrEmpty(heldItemController.currentItemName)) return;

        // Afisam info pe ecran
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 14;
        style.normal.textColor = Color.yellow;

        float y = 10;
        GUI.Label(new Rect(10, y, 500, 25), "=== HELD ITEM ADJUSTER ===", style);
        y += 20;
        GUI.Label(new Rect(10, y, 500, 25), "Item: " + heldItemController.currentItemName, style);
        y += 20;
        GUI.Label(new Rect(10, y, 500, 25), "Pos: " + position.ToString("F3"), style);
        y += 20;
        GUI.Label(new Rect(10, y, 500, 25), "Rot: " + rotation.ToString("F3"), style);
        y += 20;
        GUI.Label(new Rect(10, y, 500, 25), "Scale: " + scale.ToString("F3"), style);
        y += 20;

        style.normal.textColor = Color.cyan;
        GUI.Label(new Rect(10, y, 500, 25), "Shift+Arrows=pos | Shift+PgUp/Dn=Y | Shift+Q/R=rot", style);
        y += 20;
        GUI.Label(new Rect(10, y, 500, 25), "Shift+Numpad+/-=scale | F5=print values", style);
    }

    /// <summary>
    /// Incarca valorile din ItemData al item-ului curent in Adjuster.
    /// </summary>
    private void LoadCurrentItemValues()
    {
        if (InventorySystem.instance == null) return;

        int idx = InventorySystem.instance.selectedIndex;
        if (idx >= 0 && idx < InventorySystem.instance.items.Count)
        {
            ItemData item = InventorySystem.instance.items[idx];
            if (item != null)
            {
                position = item.heldPosition;
                rotation = item.heldRotation;
                scale = item.heldScale;
                Debug.Log("<color=lime>ADJUSTER:</color> Loaded values from " + item.itemName);
            }
        }
    }
}
