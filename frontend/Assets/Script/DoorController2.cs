using UnityEngine;

public class DoorController2 : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 2f;
    public GameObject hintUI;
    public GameObject lockedUI;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance)
        {
            bool hasKey = InventorySystem.instance != null && 
                         InventorySystem.instance.items.Exists(i => i.itemName == "Old Key");

            if (hasKey)
            {
                if (hintUI != null) hintUI.SetActive(true);
                if (lockedUI != null) lockedUI.SetActive(false);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenDoor();
                }
            }
            else
            {
                if (hintUI != null) hintUI.SetActive(false);
                if (lockedUI != null) lockedUI.SetActive(true);
            }
        }
        else
        {
            if (hintUI != null) hintUI.SetActive(false);
            if (lockedUI != null) lockedUI.SetActive(false);
        }
    }

    void OpenDoor()
    {
        Debug.Log("Usa deschisa!");
    }
}