using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public Transform doorHinge;

    public float openAngle = 90f;
    public float speed = 2f;
    public float interactDistance = 5f;

    public string sceneToLoad = "mainhol";

    private bool isOpen = false;

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = doorHinge.localRotation;
        openRot = closedRot * Quaternion.Euler(0, openAngle, 0);

        Debug.Log("SCRIPT PORNIT");
    }

    void Update()
    {
        Quaternion target = isOpen ? openRot : closedRot;

        doorHinge.localRotation = Quaternion.Lerp(
            doorHinge.localRotation,
            target,
            Time.deltaTime * speed
        );

        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Debug.Log("Lovesc: " + hit.collider.name);

            if (hit.collider.transform.IsChildOf(transform))
            {
                Debug.Log("E USA");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("APAS E");

                    if (!isOpen)
                    {
                        Debug.Log("DESCHID");

                        isOpen = true;

                        Invoke(nameof(LoadScene), 1.2f);
                    }
                }
            }
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}