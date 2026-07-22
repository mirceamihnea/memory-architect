using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ataseza-l pe usa din Room 2.
/// Deschide usa si incarca scena Main Hol DOAR daca jucatorul tine cheia in mana
/// (adica itemul selectat in inventar are itemID-ul potrivit).
/// </summary>
public class LockedDoor : MonoBehaviour
{
    [Header("Setari usa")]
    public Transform doorHinge;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Cheie necesara")]
    [Tooltip("itemID din ScriptableObject-ul cheii (ex: 'old_key')")]
    public string requiredKeyID = "old_key";

    [Header("Scena destinatie")]
    [Tooltip("Numele exact al scenei Main Hol din Build Settings")]
    public string targetSceneName = "Main Hol";
    [Tooltip("Viteza animatiei de deschidere (inainte de schimbarea scenei)")]
    public float doorOpenSpeed = 2.5f;
    [Tooltip("Secunde de pauza DUPA ce usa s-a deschis complet, inainte de a incarca scena")]
    public float delayAfterOpen = 0.5f;

    [Header("UI")]
    public GameObject interactText;       // "Apasa E" - se arata cand te uiti la usa
    public GameObject noKeyText;          // "Ai nevoie de cheie" - optional

    // ── stare interna ──────────────────────────────────────────────────────────
    private bool isOpen = false;
    private bool isUnlocking = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    // ── raza de interactiune ───────────────────────────────────────────────────
    private static readonly float InteractDistance = 3f;
    private bool playerLooking = false;

    // ──────────────────────────────────────────────────────────────────────────

    void Start()
    {
        if (interactText != null) interactText.SetActive(false);
        if (noKeyText   != null) noKeyText.SetActive(false);

        if (doorHinge != null)
        {
            closedRot = doorHinge.localRotation;
            openRot   = closedRot * Quaternion.Euler(0, openAngle, 0);
        }
        else
        {
            Debug.LogError("LockedDoor: doorHinge nu este asignat pe " + gameObject.name);
        }
    }

    void Update()
    {
        // Input si raycast — oprite dupa ce am inceput deblocarea
        if (!isUnlocking)
        {
            CheckPlayerLooking();

            if (playerLooking && Input.GetKeyDown(KeyCode.E))
                TryOpen();
        }

        // Animatia ușii rulează MEREU cat timp usa este deschisa
        // (atat in faza normala, cat si in timpul deblocarii)
        if (doorHinge != null && isOpen)
        {
            doorHinge.localRotation = Quaternion.Lerp(
                doorHinge.localRotation, openRot, Time.deltaTime * doorOpenSpeed);
        }

        // textul se uita la camera
        if (interactText != null && interactText.activeSelf && Camera.main != null)
            interactText.transform.LookAt(Camera.main.transform);

        if (noKeyText != null && noKeyText.activeSelf && Camera.main != null)
            noKeyText.transform.LookAt(Camera.main.transform);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Logica principala
    // ──────────────────────────────────────────────────────────────────────────

    void TryOpen()
    {
        if (PlayerHoldsKey())
        {
            isUnlocking = true;
            isOpen = true;

            if (interactText != null) interactText.SetActive(false);
            if (noKeyText    != null) noKeyText.SetActive(false);

            Debug.Log("<color=green>LockedDoor: Cheia e in mana — usa se deschide!</color>");
            StartCoroutine(OpenThenLoadScene());
        }
        else
        {
            Debug.Log("<color=yellow>LockedDoor: Nu tii cheia in mana!</color>");
            StartCoroutine(ShowNoKeyHint());
        }
    }

    bool PlayerHoldsKey()
    {
        if (InventorySystem.instance == null) return false;

        int sel = InventorySystem.instance.selectedIndex;
        if (sel < 0 || sel >= InventorySystem.instance.items.Count) return false;

        ItemData held = InventorySystem.instance.items[sel];
        if (held == null) return false;

        // comparam dupa itemID (sau itemName daca vrei)
        return held.itemID == requiredKeyID;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Raycasting — verifica daca jucatorul se uita la usa
    // ──────────────────────────────────────────────────────────────────────────

    void CheckPlayerLooking()
    {
        if (Camera.main == null) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        bool wasLooking = playerLooking;
        playerLooking = false;

        if (Physics.Raycast(ray, out RaycastHit hit, InteractDistance))
        {
            if (hit.collider.transform == transform ||
                hit.collider.transform.IsChildOf(transform) ||
                hit.collider.GetComponentInParent<LockedDoor>() == this)
            {
                playerLooking = true;
            }
        }

        // afisam / ascundem textul de interactiune
        if (playerLooking != wasLooking && interactText != null)
            interactText.SetActive(playerLooking);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Coroutine-uri
    // ──────────────────────────────────────────────────────────────────────────

    IEnumerator OpenThenLoadScene()
    {
        // Asteptam ca usa sa ajunga vizual la pozitia deschisa
        // (Lerp ajunge la ~99% din drum in aprox. 2-3 secunde cu speed 2.5)
        if (doorHinge != null)
        {
            float threshold = 0.5f; // grade diferenta acceptabila
            float timeout = 5f;     // max asteptam 5 secunde
            float elapsed = 0f;

            while (elapsed < timeout)
            {
                float angle = Quaternion.Angle(doorHinge.localRotation, openRot);
                if (angle < threshold) break;   // usa e practic complet deschisa

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        // Pauza scurta dupa ce usa s-a deschis, inainte de teleportare
        yield return new WaitForSeconds(delayAfterOpen);

        SceneManager.LoadScene(targetSceneName);
    }

    IEnumerator ShowNoKeyHint()
    {
        if (noKeyText != null)
        {
            noKeyText.SetActive(true);
            yield return new WaitForSeconds(2f);
            noKeyText.SetActive(false);
        }
    }
}
