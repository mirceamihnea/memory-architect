using UnityEngine;

public class UniversalPickup : MonoBehaviour
{
    private Transform player;

    public float pickupDistance = 2f;

    public ItemData itemData;



    private HeldItemController heldItemController;
    private bool pickedUp = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            // Fallback: cautam controllerul de player direct
            var fps = FindFirstObjectByType<CharacterController>();
            if (fps != null) player = fps.transform;
        }

        if (player == null)
        {
            Debug.LogError("UniversalPickup: Nu am gasit Player-ul! Te rog pune tag-ul 'Player' pe caracterul tau sau asigura-te ca ai un CharacterController.");
        }
        else
        {
            Debug.Log("UniversalPickup: Player gasit cu succes: " + player.name);
        }

        heldItemController = FindFirstObjectByType<HeldItemController>();
    }

    void Update()
    {
        if (pickedUp || player == null) return;
        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance <= pickupDistance)
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
                if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
                {
                    bool isHitMe = hit.transform == transform || 
                                   hit.transform.IsChildOf(transform) || 
                                   (hit.transform.GetComponentInParent<UniversalPickup>() == this);
                    
                    if (isHitMe)
                    {
                        // Testam tasta E direct
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            Debug.Log("<color=cyan>TASTA E DETECTATA PE:</color> " + gameObject.name);
                            Pickup();
                        }
                    }
                }
                else
                {
                    // Daca raycastul nu loveste nimic sau loveste altceva (ex: un perete), 
                    // dar suntem FOARTE aproape, permitem pickup-ul ca fallback.
                    if (distance <= 1.5f && Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("<color=yellow>PICKUP fallback (distanta mica):</color> " + (itemData != null ? itemData.itemName : gameObject.name));
                        Pickup();
                    }
                }
            }
        }
    }

    void Pickup()
    {
        if (itemData == null)
        {
            Debug.LogError("UniversalPickup: itemData is not assigned on " + gameObject.name);
            return;
        }

        pickedUp = true;

        if (InventorySystem.instance != null)
        {
            InventorySystem.instance.AddItem(itemData);
        }

        // Incercam sa deactivam tot prefab-ul (obiectul curent sau parintele)
        if (transform.parent != null && transform.parent.gameObject.name.Contains(gameObject.name.Substring(0, Mathf.Min(5, gameObject.name.Length))))
        {
            transform.parent.gameObject.SetActive(false);
        }
        
        gameObject.SetActive(false);
    }
}