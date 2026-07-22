using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Gestioneaza Main Menu-ul jocului.
/// Ataseaza acest script pe un GameObject "MainMenuManager" in scena "MainMenu".
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // ─── BUTOANE PRINCIPALE ────────────────────────────────────────
    [Header("Menu Buttons")]
    public Button startGameButton;
    public Button continueButton;
    public Button optionsButton;
    public Button quitButton;

    // ─── PANOURI MODALE ────────────────────────────────────────────
    [Header("Modal Panels")]
    public GameObject optionsPanel;

    // ─── OPTIONS ───────────────────────────────────────────────────
    [Header("Options Controls")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;
    public Button closeOptionsButton;
    public Button saveOptionsButton;


    // ─── SCENE ─────────────────────────────────────────────────────
    [Header("Scene Names")]
    [Tooltip("Scena de start (prima camera / mainhol)")]
    public string firstSceneName = "mainhol";

    [Tooltip("Scena de login — pentru butonul Logout")]
    public string loginSceneName = "LoginScene";

    // ─── PLAYER INFO ───────────────────────────────────────────────
    [Header("Player Info (optional)")]
    [Tooltip("Text care afiseaza email-ul/username-ul jucatorului logat")]
    public TextMeshProUGUI playerNameText;

    // ─── PREFS KEYS ────────────────────────────────────────────────
    private const string KEY_MASTER = "vol_master";
    private const string KEY_MUSIC  = "vol_music";
    private const string KEY_SFX    = "vol_sfx";
    private const string KEY_FS     = "fullscreen";

    // ───────────────────────────────────────────────────────────────

    private void Start()
    {
        // Butoane principale
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGame);
        if (continueButton  != null) continueButton.onClick.AddListener(OnContinue);
        if (optionsButton   != null) optionsButton.onClick.AddListener(OpenOptions);
        if (quitButton      != null) quitButton.onClick.AddListener(OnQuit);

        // Butoane modale
        if (closeOptionsButton != null) closeOptionsButton.onClick.AddListener(CloseOptions);
        if (saveOptionsButton  != null) saveOptionsButton.onClick.AddListener(SaveOptions);

        // Inchide toate modalele la start
        CloseOptions();

        // Incarca setarile salvate
        LoadOptions();

        // Afiseaza playerul logat
        if (playerNameText != null && ApiManager.Instance != null)
        {
            bool hasToken = !string.IsNullOrEmpty(ApiManager.Instance.AuthToken);
            playerNameText.text = hasToken ? "Conectat ✓" : "Guest";
        }
    }

    // ─── BUTOANE PRINCIPALE ────────────────────────────────────────

    public void OnStartGame()
    {
        SceneManager.LoadScene(firstSceneName);
    }

    // Mapeaza numarul camerei la numele scenei
    private static readonly Dictionary<int, string> RoomToScene = new Dictionary<int, string>
    {
        { 0, "mainhol" },
        { 1, "Room_1"  },
        { 2, "Room_2"  },
        { 3, "Room_3"  },
    };

    public void OnContinue()
    {
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.GetProgress((response) =>
            {
                if (response != null && response.success && response.data != null)
                {
                    int room = response.data.currentRoom; // camera curenta salvata
                    if (!RoomToScene.TryGetValue(room, out string sceneName))
                        sceneName = firstSceneName;
                    Debug.Log($"[Continue] Continui din camera {room}: {sceneName}");
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    Debug.Log("[Continue] Niciun save. Start de la inceput.");
                    SceneManager.LoadScene(firstSceneName);
                }
            });
        }
        else
        {
            SceneManager.LoadScene(firstSceneName);
        }
    }

    public void OnQuit()
    {
        // Logout si iesire din joc
        if (ApiManager.Instance != null) ApiManager.Instance.ClearAuthToken();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ─── OPTIONS ───────────────────────────────────────────────────

    public void OpenOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void SaveOptions()
    {
        if (masterVolumeSlider != null)
        {
            PlayerPrefs.SetFloat(KEY_MASTER, masterVolumeSlider.value);
            AudioListener.volume = masterVolumeSlider.value;
        }
        if (musicVolumeSlider != null)
            PlayerPrefs.SetFloat(KEY_MUSIC, musicVolumeSlider.value);
        if (sfxVolumeSlider != null)
            PlayerPrefs.SetFloat(KEY_SFX, sfxVolumeSlider.value);
        if (fullscreenToggle != null)
        {
            PlayerPrefs.SetInt(KEY_FS, fullscreenToggle.isOn ? 1 : 0);
            Screen.fullScreen = fullscreenToggle.isOn;
        }

        PlayerPrefs.Save();
        CloseOptions();
        Debug.Log("Setari salvate.");
    }

    private void LoadOptions()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
            AudioListener.volume     = masterVolumeSlider.value;
        }
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat(KEY_MUSIC, 0.7f);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value   = PlayerPrefs.GetFloat(KEY_SFX, 1f);
        if (fullscreenToggle != null)
            fullscreenToggle.isOn   = PlayerPrefs.GetInt(KEY_FS, 1) == 1;
    }

}
