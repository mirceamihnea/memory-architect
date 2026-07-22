using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;

    public GameObject inventoryPanel;

    public HeldItemController heldItemController;

    [Header("Inventory Slot Icons (UI Images)")]
    [Tooltip("Trageți aici obiectele UI Image pentru fiecare slot din hotbar.")]
    public Image[] slotIcons;

    [Header("Slot Selection Colors")]
    public Color normalSlotColor = new Color(1f, 1f, 1f, 0.3f);
    public Color selectedSlotColor = new Color(1f, 0.85f, 0f, 0.9f);
    public Color emptySlotColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);

    private bool inventoryOpen = false;

    public List<ItemData> items = new List<ItemData>();
    public int selectedIndex = -1;

   void Awake()
{
    instance = this;

    items = new List<ItemData>(InventoryData.savedItems);
}

    void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        // Initializam toate sloturile ca goale
        RefreshAllSlotIcons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryOpen = !inventoryOpen;

            if (inventoryPanel != null) inventoryPanel.SetActive(inventoryOpen);

            Cursor.visible = inventoryOpen;

            Cursor.lockState = inventoryOpen
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        }

        // Selectie iteme cu taste 1-9 si 0 (functioneaza MEREU, nu doar cand ai iteme)
        // Taste 1-9 = sloturi 0-8, tasta 0 = slot 9
        for (int i = 0; i < 10; i++)
        {
            KeyCode key = (i < 9) ? (KeyCode.Alpha1 + i) : KeyCode.Alpha0;
            int slotIndex = i;

            if (Input.GetKeyDown(key))
            {
                if (slotIndex < items.Count)
                {
                    // Slotul are item - il selectam (sau deselectam daca e deja selectat)
                    if (selectedIndex == slotIndex)
                    {
                        // Apasam din nou pe acelasi slot = deselectam (maini goale)
                        selectedIndex = -1;
                    }
                    else
                    {
                        selectedIndex = slotIndex;
                    }
                }
                else
                {
                    // Slot gol - deselectam orice item (maini goale)
                    selectedIndex = -1;
                }

                UpdateHeldItem();
                RefreshSlotHighlight();
            }
        }

        // Scroll intre iteme (doar daca avem iteme)
        if (!inventoryOpen && items.Count > 0)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                selectedIndex = (selectedIndex + 1) % items.Count;
                UpdateHeldItem();
                RefreshSlotHighlight();
            }
            else if (scroll < 0f)
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = items.Count - 1;
                UpdateHeldItem();
                RefreshSlotHighlight();
            }
        }
    }

    public void AddItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("InventorySystem: Attempted to add a null item!");
            return;
        }

        if (!items.Contains(item))
        {
            items.Add(item);
InventoryData.savedItems = new List<ItemData>(items);
            // Daca mainile sunt goale (nimic selectat), selectam automat noul item.
            // Daca tinem deja ceva in mana, lasam selectia curenta si punem itemul
            // pe primul slot liber fara sa intrerupem ce tinem.
            bool handsEmpty = selectedIndex < 0;
            if (handsEmpty)
            {
                selectedIndex = items.Count - 1;
                UpdateHeldItem();
            }

            // Actualizam iconitele sloturilor
            RefreshAllSlotIcons();

            Debug.Log("<color=green>INVENTORY ADD:</color> " + item.itemName +
                " | Slot: " + (items.Count - 1) +
                " | Total items: " + items.Count +
                " | Selected: " + selectedIndex +
                " | Has icon: " + (item.icon != null) +
                " | SlotIcons assigned: " + (slotIcons != null ? slotIcons.Length.ToString() : "NULL"));
        }
        else
        {
            Debug.LogWarning("<color=yellow>INVENTORY:</color> Item already in inventory: " + item.itemName);
        }
    }

    public void EquipItem(ItemData item)
    {
        if (item == null || item.heldPrefab == null)
            return;

        if (heldItemController != null)
        {
            heldItemController.HoldItem(item.heldPrefab, item.itemName);
        }
        else
        {
            Debug.LogError("InventorySystem: heldItemController is not assigned!");
        }

        inventoryOpen = false;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
InventoryData.savedItems = new List<ItemData>(items);
            // Resetam selectia daca am sters itemul curent
            selectedIndex = -1;
            if (heldItemController != null) heldItemController.ClearHand();

            // Actualizam iconitele sloturilor
            RefreshAllSlotIcons();
if (items.Count > 0)
{
    selectedIndex = 0;
    UpdateHeldItem();
}
            Debug.Log("Removed item: " + item.itemName);
        }
    }
public void ClearInventory()
{
    items.Clear();

    selectedIndex = -1;

    if (heldItemController != null)
    {
        heldItemController.ClearHand();
    }

    RefreshAllSlotIcons();
}
    public void UpdateHeldItem()
    {
        if (selectedIndex >= 0 && selectedIndex < items.Count)
        {
            ItemData current = items[selectedIndex];
            if (heldItemController != null && current.heldPrefab != null)
            {
                heldItemController.HoldItem(current);
            }
        }
        else
        {
            // Maini goale
            if (heldItemController != null) heldItemController.ClearHand();
        }
    }

    /// <summary>
    /// Actualizeaza iconitele tuturor sloturilor din hotbar.
    /// Sloturile ocupate primesc sprite-ul item-ului, cele goale sunt ascunse.
    /// </summary>
    private void RefreshAllSlotIcons()
    {
        if (slotIcons == null || slotIcons.Length == 0)
        {
            Debug.LogWarning("<color=red>INVENTORY:</color> slotIcons array is null or empty! Assign slot Image references in Inspector.");
            return;
        }

        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] == null)
            {
                Debug.LogWarning("<color=red>INVENTORY:</color> slotIcons[" + i + "] is null!");
                continue;
            }

            if (i < items.Count && items[i] != null)
            {
                if (items[i].icon != null)
                {
                    // Slotul are un item cu iconita - afisam
                    slotIcons[i].sprite = items[i].icon;
                    slotIcons[i].enabled = true;
                    slotIcons[i].color = Color.white;

                    // Facem si GameObject-ul activ (in caz ca era dezactivat)
                    slotIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    // Item fara iconita
                    slotIcons[i].sprite = null;
                    slotIcons[i].enabled = true;
                    slotIcons[i].color = new Color(1f, 1f, 1f, 0.5f);
                    slotIcons[i].gameObject.SetActive(true);
                    Debug.LogWarning("<color=yellow>SLOT " + i + ":</color> item " + items[i].itemName + " has no icon sprite!");
                }
            }
            else
            {
                // Slot gol - ascundem iconita
                slotIcons[i].sprite = null;
                slotIcons[i].enabled = false;
            }
        }

        // Actualizam si highlight-ul slotului selectat
        RefreshSlotHighlight();
    }

    /// <summary>
    /// Evidentiaza slotul selectat cu o culoare diferita pe background-ul parintelui.
    /// </summary>
    private void RefreshSlotHighlight()
    {
        if (slotIcons == null) return;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] == null) continue;

            // Schimbam culoarea background-ului parintelui (slot-ul in sine)
            Image parentImage = slotIcons[i].transform.parent != null
                ? slotIcons[i].transform.parent.GetComponent<Image>()
                : null;

            if (parentImage != null)
            {
                if (i == selectedIndex)
                {
                    // Slot selectat
                    parentImage.color = new Color(selectedSlotColor.r, selectedSlotColor.g, selectedSlotColor.b, 0.5f);
                }
                else if (i < items.Count)
                {
                    // Slot cu item dar neselectat
                    parentImage.color = new Color(normalSlotColor.r, normalSlotColor.g, normalSlotColor.b, 0.3f);
                }
                else
                {
                    // Slot gol
                    parentImage.color = emptySlotColor;
                }
            }
        }
    }
}