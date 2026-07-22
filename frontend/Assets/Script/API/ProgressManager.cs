using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

[Serializable]
public class ProgressRequest
{
    public int roomCompleted;
}

[Serializable]
public class ProgressData
{
    public string playerId;
    public int currentRoom;
}

[Serializable]
public class ProgressResponse
{
    public bool success;
    public ProgressData data;
    public string error;
}

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    // Mapeaza numele scenei la indexul camerei pentru autosave
    private static readonly Dictionary<string, int> SceneToRoom = new Dictionary<string, int>
    {
        { "mainhol", 0 },
        { "Room_1",  1 },
        { "Room_2",  2 },
        { "Room_3",  3 },
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // IMPORTANT: Deparent so DontDestroyOnLoad works
            DontDestroyOnLoad(gameObject);
            Debug.Log("[ProgressManager] Initalizat cu succes ca DontDestroyOnLoad.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cand o scena de joc se incarca, salvam progresul automat
        if (SceneToRoom.TryGetValue(scene.name, out int roomNum))
        {
            Debug.Log($"[Autosave] Detectat incarcare scena: {scene.name}. Salvez camera {roomNum}...");
            SaveProgress(roomNum, (response) => {
                if (response != null && response.success)
                {
                    Debug.Log($"[Autosave] Progres salvat cu succes pentru camera {roomNum}.");
                }
            });
        }
    }

    public void SaveProgress(int roomCompleted, Action<ProgressResponse> onResponse = null)
    {
        ProgressRequest req = new ProgressRequest { roomCompleted = roomCompleted };
        string json = JsonUtility.ToJson(req);

        if (ApiManager.Instance == null)
        {
            Debug.LogWarning("[ProgressManager] ApiManager.Instance is null! Skipping SaveProgress.");
            onResponse?.Invoke(new ProgressResponse { success = false, error = "ApiManager not initialized" });
            return;
        }

        StartCoroutine(ApiManager.Instance.PostRequest("progress", json, 
            (responseJson) => {
                ProgressResponse res = JsonUtility.FromJson<ProgressResponse>(responseJson);
                onResponse?.Invoke(res);
            }, 
            (error) => {
                Debug.LogError("SaveProgress Error: " + error);
                onResponse?.Invoke(new ProgressResponse { success = false, error = error });
            }));
    }

    public void GetProgress(Action<ProgressResponse> onResponse)
    {
        if (ApiManager.Instance == null)
        {
            Debug.LogWarning("[ProgressManager] ApiManager.Instance is null! Skipping GetProgress.");
            onResponse?.Invoke(new ProgressResponse { success = false, error = "ApiManager not initialized" });
            return;
        }

        StartCoroutine(ApiManager.Instance.GetRequest("progress", 
            (responseJson) => {
                ProgressResponse res = JsonUtility.FromJson<ProgressResponse>(responseJson);
                onResponse?.Invoke(res);
            }, 
            (error) => {
                Debug.LogError("GetProgress Error: " + error);
                onResponse?.Invoke(new ProgressResponse { success = false, error = error });
            }));
    }
}
