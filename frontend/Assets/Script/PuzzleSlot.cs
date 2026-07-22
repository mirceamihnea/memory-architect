using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    [Header("Referinte")]
    public PuzzleManager puzzleManager;
    public Transform player;
    public float interactionDistance = 2.5f;

    [Header("UI (Optional)")]
    public GameObject hintUI;

    [Header("Setari Vizuale")]
    public Vector3 spawnRotation = new Vector3(0f, 90f, 0f);

    private bool isOccupied = false;
    private Camera mainCamera;
    private GameObject spawnedObject = null;
    private string placedItemName = null;
    private HeldItemController heldItemController;

    void Start()
    {
        mainCamera = Camera.main;
        heldItemController = FindFirstObjectByType<HeldItemController>();
    }

    void Update()
    {
        if (!IsLookedAt()) 
        {
            if (hintUI != null) hintUI.SetActive(false);
            return;
        }

        if (hintUI != null) hintUI.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("<color=cyan>E APASAT PE SLOT:</color> " + gameObject.name + " (isOccupied: " + isOccupied + ")");
            if (isOccupied)
                TryRetrieveItem();
            else
                TryPlaceItem();
        }
    }

    bool IsLookedAt()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > interactionDistance) return false;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            // Verificam daca am lovit slotul sau obiectul deja plasat in el
            bool isMe = hit.transform == transform || hit.transform.IsChildOf(transform);
            
            if (!isMe && spawnedObject != null)
            {
                isMe = (hit.transform == spawnedObject.transform || hit.transform.IsChildOf(spawnedObject.transform));
            }

            return isMe;
        }

        return false;
    }

    void TryPlaceItem()
    {
        if (InventorySystem.instance == null)
        {
            Debug.LogError("PuzzleSlot: InventorySystem.instance is null!");
            return;
        }

        // Verificam daca avem ceva selectat in noul sistem
        if (InventorySystem.instance.selectedIndex == -1 || InventorySystem.instance.items.Count == 0)
        {
            Debug.Log("Nu poti pune nimic: Nu tii niciun obiect in mana!");
            return;
        }

        ItemData itemToPlaceData = InventorySystem.instance.items[InventorySystem.instance.selectedIndex];
        string itemToPlace = itemToPlaceData.itemName;
        
        var mapping = puzzleManager.GetMappingFor(itemToPlace);
        if (mapping == null)
        {
            Debug.LogWarning("PuzzleSlot: Nu exista mapping in PuzzleManager pentru " + itemToPlace);
            return;
        }

        // DOAR ACUM SCOATEM DIN INVENTAR
        InventorySystem.instance.RemoveItem(itemToPlaceData);

        isOccupied = true;
        placedItemName = itemToPlace;
        if (hintUI != null) hintUI.SetActive(false);

        // Aplicam Offset-ul la pozitie
        Vector3 spawnPos = transform.position + mapping.Value.customOffset;
        
        // Aplicam Rotatia: folosim custom daca e bifat, altfel folosim spawnRotation a slotului
        Quaternion finalRot = mapping.Value.useCustomRotation ? 
                              Quaternion.Euler(mapping.Value.customRotation) : 
                              Quaternion.Euler(spawnRotation);

        GameObject inst = Instantiate(mapping.Value.prefab, spawnPos, finalRot);
        
        inst.transform.SetParent(null, worldPositionStays: true);

        Vector3 scalaFinala = mapping.Value.customScale != Vector3.zero ? mapping.Value.customScale : Vector3.one;
        inst.transform.localScale = scalaFinala;
        inst.SetActive(true);
        spawnedObject = inst;

        // Dezactivam scriptul de pickup ca sa nu interfereze cu slotul
        UniversalPickup up = inst.GetComponentInChildren<UniversalPickup>();
        if (up != null) up.enabled = false;

        puzzleManager.OnItemPlaced(itemToPlace);
        Debug.Log("<color=green>OBIECT PLASAT:</color> " + itemToPlace + " la coordonatele: " + inst.transform.position);
    }

    void TryRetrieveItem()
    {
        // Nu putem lua inapoi daca avem deja ceva in mana (optional, depinde de gameplay)
        if (InventorySystem.instance != null && InventorySystem.instance.selectedIndex != -1)
        {
            Debug.Log("Ai deja ceva in mana! Pune-l jos sau schimba-l inainte sa recuperezi acest item.");
            return;
        }

        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }

        // IL PUNEM INAPOI IN INVENTAR
        if (InventorySystem.instance != null)
        {
            var mapping = puzzleManager.GetMappingFor(placedItemName);
            if (mapping != null && mapping.Value.itemData != null)
            {
                InventorySystem.instance.AddItem(mapping.Value.itemData);
            }
            else
            {
                Debug.LogWarning("PuzzleSlot: Nu am gasit ItemData pentru " + placedItemName + " in PuzzleManager. Nu il pot returna in inventar!");
            }
        }

        // Anuntam Managerul
        puzzleManager.OnItemRemoved(placedItemName);

        Debug.Log("OBIECT RECUPERAT: " + placedItemName);

        placedItemName = null;
        isOccupied = false;
    }
}