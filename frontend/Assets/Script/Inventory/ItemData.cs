using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;

    public Sprite icon;
    public GameObject heldPrefab;

    [TextArea]
    public string description;

    [Header("Held In Hand Settings")]
    public Vector3 heldPosition = new Vector3(0.55f, -0.15f, 0.45f);
    public Vector3 heldRotation = new Vector3(15f, 280f, 0f);
    public Vector3 heldScale = new Vector3(1f, 1f, 1f);
}