using UnityEngine;

public class NoteInteraction : MonoBehaviour
{
    public GameObject panel;

    private bool nearNote = false;
    private bool open = false;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void Update()
    {
        if (nearNote && Input.GetKeyDown(KeyCode.E))
        {
            open = !open;
            panel.SetActive(open);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            nearNote = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearNote = false;
            open = false;
            panel.SetActive(false);
        }
    }
}