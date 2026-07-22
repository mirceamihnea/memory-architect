using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public Transform player;
    public float pickupDistance = 1.5f;
    public ItemData itemData;

    private bool isPicked = false;

    void Update()
    {
        if (isPicked) return;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= pickupDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUp();
            }
        }
    }

    void PickUp()
    {
        if (InventorySystem.instance == null)
        {
            Debug.LogError("KeyPickup: InventorySystem.instance is missing!");
            return;
        }

        isPicked = true;
        
        if (itemData != null)
        {
            InventorySystem.instance.AddItem(itemData);
        }
        else
        {
            Debug.LogWarning("KeyPickup: itemData is missing! Adding string only if possible...");
        }
        
        gameObject.SetActive(false);
        Debug.Log("Cheie luata!");
    }
}