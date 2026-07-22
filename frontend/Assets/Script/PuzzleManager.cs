using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Conditii Victorie")]
    public List<string> requiredItems = new List<string> { "floare", "Mask 2", "papusa", "book" };

    [System.Serializable]
    public struct ItemMapping 
    {
        public string itemName;
        public GameObject prefab;
        public Vector3 customScale; 
        public Vector3 customOffset;
        public bool useCustomRotation; // New checkbox
        public Vector3 customRotation;
        public ItemData itemData;
    }
    
    [Header("Toate Obiectele din Joc")]
    public List<ItemMapping> allItemPrefabs = new List<ItemMapping>();

    [Header("Victorian Painting Reward")]
    public GameObject victorianPainting;
    public Vector3 paintingTargetPos;
    public Vector3 paintingTargetRot;
    public Vector3 paintingTargetScale = Vector3.one; // New field
    public float moveSpeed = 2f;

    [Header("Scala Default (daca customScale e zero)")]
    public Vector3 defaultScale = new Vector3(0.1f, 0.1f, 0.1f);

    private List<string> currentlyPlacedItems = new List<string>();
    private bool isSolved = false;

    void Update()
    {
        if (isSolved && victorianPainting != null)
        {
            // Misca, roteste si redimensioneaza pictura catre tinta intr-un mod placut (Smooth)
            victorianPainting.transform.position = Vector3.Lerp(victorianPainting.transform.position, paintingTargetPos, Time.deltaTime * moveSpeed);
            victorianPainting.transform.rotation = Quaternion.Lerp(victorianPainting.transform.rotation, Quaternion.Euler(paintingTargetRot), Time.deltaTime * moveSpeed);
            victorianPainting.transform.localScale = Vector3.Lerp(victorianPainting.transform.localScale, paintingTargetScale, Time.deltaTime * moveSpeed);
        }
    }

    [Header("Actiuni la Finalizare")]
    public DoorInteraction exitDoor;

    public ItemMapping? GetMappingFor(string name)
    {
        if (allItemPrefabs == null) return null;

        foreach (var mapping in allItemPrefabs)
        {
            if (string.Equals(mapping.itemName, name, System.StringComparison.OrdinalIgnoreCase))
            {
                if (mapping.customScale == Vector3.zero)
                {
                    var fixedMapping = mapping;
                    fixedMapping.customScale = defaultScale;
                    return fixedMapping;
                }

                return mapping;
            }
        }
        return null;
    }

    public void OnItemPlaced(string itemName)
    {
        if (!currentlyPlacedItems.Contains(itemName))
            currentlyPlacedItems.Add(itemName);

        CheckPuzzleCompletion();
    }

    public void OnItemRemoved(string itemName)
    {
        currentlyPlacedItems.Remove(itemName);
        Debug.Log("Item scos din puzzle: " + itemName);
    }

    private void CheckPuzzleCompletion()
    {
        int correctCount = 0;
        foreach (string required in requiredItems)
        {
            if (currentlyPlacedItems.Contains(required))
                correctCount++;
        }

        if (correctCount >= requiredItems.Count && !isSolved)
        {
            isSolved = true;
            Debug.Log("<color=gold>Puzzle Finalizat!</color> Pictura se misca...");

            if (ProgressManager.Instance != null)
                ProgressManager.Instance.SaveProgress(2);
        }
    }
}