using UnityEngine;

public class ClearInventoryOnStart : MonoBehaviour
{
    void Start()
    {
        if (InventorySystem.instance != null)
        {
            InventorySystem.instance.ClearInventory();
        }
    }
}