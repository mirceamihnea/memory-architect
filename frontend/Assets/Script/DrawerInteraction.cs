using UnityEngine;

public class DrawerInteraction : MonoBehaviour
{
    public Transform drawer;

    public Vector3 openOffset = new Vector3(-0.3f, 0f, 0f);

    public float speed = 2f;

    public float interactDistance = 3f;

    private Vector3 closedPosition;
    private Vector3 openedPosition;

    private bool opened = false;

    private Transform player;
    private Camera playerCamera;

    void Start()
    {
        if (drawer == null)
            drawer = transform;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        playerCamera = Camera.main;

        closedPosition = drawer.localPosition;
        openedPosition = closedPosition + openOffset;
    }

    void Update()
    {
        if (!opened)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            Ray ray = playerCamera.ScreenPointToRay(
                new Vector3(Screen.width / 2, Screen.height / 2)
            );

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.transform == transform)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        opened = true;
                    }
                }
            }
        }

        Vector3 targetPosition = opened ? openedPosition : closedPosition;

        drawer.localPosition = Vector3.Lerp(
            drawer.localPosition,
            targetPosition,
            Time.deltaTime * speed
        );
    }
}