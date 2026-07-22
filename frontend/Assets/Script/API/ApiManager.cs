using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [Header("API Settings")]
    public string baseUrl = "http://localhost:3000/api";
    
    public string AuthToken { get; private set; }

    private void Awake()
    {
        // Singleton pattern with deparenting to survive scene load
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // IMPORTANT: Deparent so DontDestroyOnLoad works
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAuthToken(string token)
    {
        AuthToken = token;
        Debug.Log("Token-ul de autentificare a fost salvat.");
    }

    public void ClearAuthToken()
    {
        AuthToken = null;
    }

    public IEnumerator PostRequest(string endpoint, string jsonPayload, Action<string> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/{endpoint}";
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(AuthToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {AuthToken}");
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                string errorDetail = request.downloadHandler.text;
                onError?.Invoke($"HTTP: {request.error} | Detalii: {errorDetail}");
            }
        }
    }

    public IEnumerator GetRequest(string endpoint, Action<string> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/{endpoint}";
        using (UnityWebRequest request = new UnityWebRequest(url, "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(AuthToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {AuthToken}");
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                string errorDetail = request.downloadHandler.text;
                onError?.Invoke($"HTTP: {request.error} | Detalii: {errorDetail}");
            }
        }
    }
}
