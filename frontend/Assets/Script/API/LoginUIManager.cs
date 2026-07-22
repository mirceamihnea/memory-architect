using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Gestioneaza interfata de Login si Register.
/// Ataseaza acest script pe un GameObject "LoginUIManager" in scena "LoginScene".
/// </summary>
public class LoginUIManager : MonoBehaviour
{
    // ─── PANELS ────────────────────────────────────────────────────
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    // ─── LOGIN ─────────────────────────────────────────────────────
    [Header("Login Fields")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public Button loginButton;
    public Button goToRegisterButton;

    // ─── REGISTER ──────────────────────────────────────────────────
    [Header("Register Fields")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerConfirmPasswordInput;
    public Button registerButton;
    public Button goToLoginButton;

    // ─── FEEDBACK ──────────────────────────────────────────────────
    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 3f;

    // ─── SCENE ─────────────────────────────────────────────────────
    [Header("Scene Transition")]
    [Tooltip("Numele scenei de Main Menu — trebuie adaugat in Build Settings")]
    public string mainMenuSceneName = "MainMenu";

    // ───────────────────────────────────────────────────────────────

    private void Start()
    {
        ClearFeedback();
        ShowLoginPanel();

        if (loginButton != null)        loginButton.onClick.AddListener(OnLoginClicked);
        if (registerButton != null)     registerButton.onClick.AddListener(OnRegisterClicked);
        if (goToRegisterButton != null) goToRegisterButton.onClick.AddListener(ShowRegisterPanel);
        if (goToLoginButton != null)    goToLoginButton.onClick.AddListener(ShowLoginPanel);
    }

    // ─── PANEL SWITCH ──────────────────────────────────────────────

    public void ShowLoginPanel()
    {
        if (loginPanel != null)    loginPanel.SetActive(true);
        if (registerPanel != null) registerPanel.SetActive(false);
        ClearFeedback();
    }

    public void ShowRegisterPanel()
    {
        if (loginPanel != null)    loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(true);
        ClearFeedback();
    }

    // ─── LOGIN ─────────────────────────────────────────────────────

    private void OnLoginClicked()
    {
        string email    = loginEmailInput != null ? loginEmailInput.text.Trim() : "";
        string password = loginPasswordInput != null ? loginPasswordInput.text : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowFeedback("Introdu email-ul și parola!", Color.red);
            return;
        }

        SetButtonsInteractable(false);
        ShowFeedback("Se conectează...", Color.yellow);

        AuthManager.Instance.Login(email, password, (response) =>
        {
            SetButtonsInteractable(true);
            if (response.success)
            {
                ShowFeedback("Login reușit! Se încarcă meniul...", Color.green);
                StartCoroutine(LoadMainMenuAfterDelay(0.8f));
            }
            else
            {
                ShowFeedback("Eroare: " + response.error, Color.red);
            }
        });
    }

    // ─── REGISTER ──────────────────────────────────────────────────

    private void OnRegisterClicked()
    {
        Debug.Log("[Register] Buton apasat!");

        string email    = registerEmailInput != null ? registerEmailInput.text.Trim() : "";
        string password = registerPasswordInput != null ? registerPasswordInput.text : "";
        string confirm  = registerConfirmPasswordInput != null ? registerConfirmPasswordInput.text : "";

        Debug.Log($"[Register] email='{email}' pass len={password.Length} confirm len={confirm.Length}");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowFeedback("Completeaza toate campurile!", Color.red);
            Debug.Log("[Register] STOP: campuri goale");
            return;
        }

        if (password != confirm)
        {
            ShowFeedback("Parolele nu coincid!", Color.red);
            Debug.Log("[Register] STOP: parole diferite");
            return;
        }

        if (password.Length < 6)
        {
            ShowFeedback("Parola prea scurta (min 6)!", Color.red);
            Debug.Log("[Register] STOP: parola prea scurta");
            return;
        }

        Debug.Log($"[Register] AuthManager={AuthManager.Instance != null} ApiManager={ApiManager.Instance != null}");

        SetButtonsInteractable(false);
        ShowFeedback("Se creeaza contul...", Color.yellow);

        AuthManager.Instance.Register(email, password, (response) =>
        {
            SetButtonsInteractable(true);
            if (response.success)
            {
                ShowFeedback("Cont creat! Se încarcă meniul...", Color.green);
                StartCoroutine(LoadMainMenuAfterDelay(0.8f));
            }
            else
            {
                ShowFeedback("Eroare: " + response.error, Color.red);
            }
        });
    }

    // ─── HELPERS ───────────────────────────────────────────────────

    private IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetButtonsInteractable(bool state)
    {
        if (loginButton != null)    loginButton.interactable    = state;
        if (registerButton != null) registerButton.interactable = state;
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;
        feedbackText.text  = message;
        feedbackText.color = color;
        CancelInvoke(nameof(ClearFeedback));
        Invoke(nameof(ClearFeedback), feedbackDuration);
    }

    private void ClearFeedback()
    {
        if (feedbackText != null) feedbackText.text = "";
    }
}
