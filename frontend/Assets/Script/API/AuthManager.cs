using UnityEngine;
using System;

// Clasele folosite pentru Serializarea JSON în Unity
[Serializable]
public class AuthRequest
{
    public string email;
    public string password;
}

[Serializable]
public class AuthResponseData
{
    public string userId;
    public string token;
    public string message;
}

[Serializable]
public class AuthResponse
{
    public bool success;
    public AuthResponseData data;
    public string error;
}

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Login(string email, string password, Action<AuthResponse> onResponse)
    {
        AuthRequest req = new AuthRequest { email = email, password = password };
        string json = JsonUtility.ToJson(req);

        StartCoroutine(ApiManager.Instance.PostRequest("auth/login", json, 
            (responseJson) => {
                AuthResponse res = JsonUtility.FromJson<AuthResponse>(responseJson);
                if (res.success && res.data != null)
                {
                    ApiManager.Instance.SetAuthToken(res.data.token);
                }
                onResponse?.Invoke(res);
            }, 
            (error) => {
                Debug.LogError("Login Error: " + error);
                onResponse?.Invoke(new AuthResponse { success = false, error = error });
            }));
    }

    public void Register(string email, string password, Action<AuthResponse> onResponse)
    {
        AuthRequest req = new AuthRequest { email = email, password = password };
        string json = JsonUtility.ToJson(req);

        StartCoroutine(ApiManager.Instance.PostRequest("auth/register", json, 
            (responseJson) => {
                AuthResponse res = JsonUtility.FromJson<AuthResponse>(responseJson);
                if (res.success && res.data != null)
                {
                    // La inregistrare serverul returneaza direct token-ul
                    ApiManager.Instance.SetAuthToken(res.data.token);
                }
                onResponse?.Invoke(res);
            }, 
            (error) => {
                Debug.LogError("Register Error: " + error);
                onResponse?.Invoke(new AuthResponse { success = false, error = error });
            }));
    }
}
