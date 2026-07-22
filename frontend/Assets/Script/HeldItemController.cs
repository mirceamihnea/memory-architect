using UnityEngine;

public class HeldItemController : MonoBehaviour
{
    public Transform handPoint;

    private GameObject currentItem;
    public string currentItemName;

    public void HoldItem(ItemData itemData)
    {
        if (itemData == null || itemData.heldPrefab == null)
        {
            Debug.LogWarning("HeldItemController: Attempted to hold a null item!");
            return;
        }

        if (handPoint == null)
        {
            Debug.LogError("HeldItemController: handPoint is not assigned on " + gameObject.name);
            return;
        }

        currentItemName = itemData.itemName;
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        currentItem = Instantiate(itemData.heldPrefab, handPoint);

        currentItem.transform.localPosition = itemData.heldPosition;
        currentItem.transform.localRotation = Quaternion.Euler(itemData.heldRotation);
        currentItem.transform.localScale = itemData.heldScale;

        Collider[] colliders = currentItem.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    // Backwards compatible - folosita de UniversalPickup
    public void HoldItem(GameObject prefab, string itemName)
    {
        if (prefab == null)
        {
            Debug.LogWarning("HeldItemController: Attempted to hold a null prefab!");
            return;
        }

        if (handPoint == null)
        {
            Debug.LogError("HeldItemController: handPoint is not assigned on " + gameObject.name);
            return;
        }

        currentItemName = itemName;
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        currentItem = Instantiate(prefab, handPoint);

        currentItem.transform.localPosition = new Vector3(0.55f, -0.15f, 0.45f);
        currentItem.transform.localRotation = Quaternion.Euler(15f, 280f, 0f);
        currentItem.transform.localScale = new Vector3(1f, 1f, 1f);

        Collider[] colliders = currentItem.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void ClearHand()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }
        currentItemName = null;
        Debug.Log("Mana a fost eliberata.");
    }
}