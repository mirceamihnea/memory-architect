using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTeleport : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("A intrat ceva in trigger");

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER DETECTAT");

            SceneManager.LoadScene(sceneName);
        }
    }
}