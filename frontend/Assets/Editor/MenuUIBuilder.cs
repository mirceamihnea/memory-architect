using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class MenuUIBuilder
{
    static Color C_BG      = Hex("#050D1A");
    static Color C_CARD    = Hex("#0A1424DD"); // High-quality translucent dark glassmorphism
    static Color C_BORDER  = Hex("#1A3A5C");
    static Color C_CYAN    = Hex("#00E5FF");   // Bright neon cyan
    static Color C_PURPLE  = Hex("#7C3AED");   // Neon violet
    static Color C_INPUT   = Hex("#07111F");   // Deep futuristic input background
    static Color C_LABEL   = Hex("#8BA8C8");
    static Color C_WHITE   = Hex("#EEF4FF");
    static Color C_DANGER  = Hex("#FF4D6D");   // Cyber red
    static Color C_SUCCESS = Hex("#00E5B0");   // Neon green
    static Color C_BTNSEC  = Hex("#112240");   // Translucent secondary button base

    static Color Hex(string h) { ColorUtility.TryParseHtmlString(h, out Color c); return c; }

    // =========================================================
    [MenuItem("Tools/Build Login UI")]
    public static void BuildLoginUI()
    {
        var canvas = SetupCanvas();
        CreateEventSystem();

        // 1. Background Image with Sprite fallback
        var bg = Img(canvas.transform, "BG", Color.white);
        StretchFull(bg);
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Texture/Simple_Background.png");
        if (bgSprite != null)
        {
            bg.sprite = bgSprite;
            bg.color = Color.white;
        }
        else
        {
            bg.color = C_BG;
        }

        // 2. Corner Bracket Accent Line (left side decorative edge)
        var line = Img(canvas.transform, "AccentLine", C_CYAN);
        var lr = line.GetComponent<RectTransform>();
        lr.anchorMin = new Vector2(0,0); lr.anchorMax = new Vector2(0,1);
        lr.offsetMin = Vector2.zero; lr.offsetMax = new Vector2(4,0);

        // 3. Central Login Card
        var border = RectGO(canvas.transform, "CardBorder", new Vector2(464,664), Vector2.zero);
        AddRoundedImage(border.gameObject, C_BORDER);
        var borderOutline = border.gameObject.AddComponent<Outline>();
        borderOutline.effectColor = new Color(C_CYAN.r, C_CYAN.g, C_CYAN.b, 0.35f);
        borderOutline.effectDistance = new Vector2(2f, -2f);

        var card = RectGO(canvas.transform, "Card", new Vector2(460,660), Vector2.zero);
        AddRoundedImage(card.gameObject, C_CARD);
        
        // Add futuristic corner accents to the Card!
        AddCornerAccents(card, new Vector2(460, 660), C_CYAN, 2f, 18f);

        // 4. Logo & Decorative Text
        var logo = TMP(card, "Logo", "MEMORY\nARCHITECT", 48f, C_CYAN, FontStyles.Bold);
        Rect(logo, new Vector2(0,265), new Vector2(420,110));
        logo.alignment = TextAlignmentOptions.Center;
        logo.lineSpacing = -12f;

        var sep0 = Img(card, "Sep0", C_BORDER);
        Rect(sep0.GetComponent<RectTransform>(), new Vector2(0,195), new Vector2(400,1));

        var sub = TMP(card, "Tagline", "- ENTER THE MINDSCAPE -", 10f, C_LABEL, FontStyles.Normal);
        Rect(sub, new Vector2(0,179), new Vector2(400,22));
        sub.alignment = TextAlignmentOptions.Center;
        sub.characterSpacing = 6f;

        var authDecor = TMP(card, "AuthDecor", "[ SECURITY CORE CONNECTED ]", 8f, C_LABEL, FontStyles.Italic);
        Rect(authDecor, new Vector2(0,158), new Vector2(400,18));
        authDecor.alignment = TextAlignmentOptions.Center;
        authDecor.characterSpacing = 3f;

        // LOGIN PANEL
        var lp = RectGO(card, "LoginPanel", new Vector2(400,370), new Vector2(0,-45));

        var eL = TMP(lp, "ELabel", "ADRESA EMAIL", 9f, C_LABEL, FontStyles.Bold); eL.characterSpacing = 3f;
        Rect(eL, new Vector2(-135,148), new Vector2(130,18));
        var emailInput = MakeInput(lp, "EmailInput", "utilizator@email.com", new Vector2(0,120), new Vector2(400,48));

        var pL = TMP(lp, "PLabel", "PAROLA DE SECURE", 9f, C_LABEL, FontStyles.Bold); pL.characterSpacing = 3f;
        Rect(pL, new Vector2(-135,62), new Vector2(130,18));
        var passInput = MakeInput(lp, "PasswordInput", "parola", new Vector2(0,34), new Vector2(400,48));
        passInput.contentType = TMP_InputField.ContentType.Password;

        var loginBtn = MakeBtn(lp, "LoginButton", "INTRA IN JOC", new Vector2(0,-34), new Vector2(400,52), C_CYAN, Color.black, true);

        var divTxt = TMP(lp, "Div", "// - sau - //", 9f, new Color(0.4f,0.5f,0.6f,0.5f), FontStyles.Normal);
        Rect(divTxt, new Vector2(0,-95), new Vector2(400,20));
        divTxt.alignment = TextAlignmentOptions.Center;
        divTxt.characterSpacing = 2f;

        MakeBtn(lp, "GoToRegisterButton", "Nu ai cont? Creează unul >>", new Vector2(0,-136), new Vector2(400,44), C_BTNSEC, C_CYAN, false);

        // REGISTER PANEL
        var rp = RectGO(card, "RegisterPanel", new Vector2(400,420), new Vector2(0,-40));
        rp.gameObject.SetActive(false);

        var regTitle = TMP(rp, "RegTitle", "CREARE CONT NOU", 20f, C_WHITE, FontStyles.Bold);
        Rect(regTitle, new Vector2(0,185), new Vector2(400,36));
        regTitle.alignment = TextAlignmentOptions.Center;
        regTitle.characterSpacing = 1f;

        var reL = TMP(rp, "RELabel", "EMAIL", 9f, C_LABEL, FontStyles.Bold); reL.characterSpacing = 3f;
        Rect(reL, new Vector2(-160,135), new Vector2(80,18));
        var regEmail = MakeInput(rp, "RegisterEmailInput", "email@exemplu.com", new Vector2(0,110), new Vector2(400,48));

        var rpL = TMP(rp, "RPLabel", "PAROLA", 9f, C_LABEL, FontStyles.Bold); rpL.characterSpacing = 3f;
        Rect(rpL, new Vector2(-160,55), new Vector2(80,18));
        var regPass = MakeInput(rp, "RegisterPasswordInput", "minim 6 caractere", new Vector2(0,30), new Vector2(400,48));
        regPass.contentType = TMP_InputField.ContentType.Password;

        var rcL = TMP(rp, "RCLabel", "CONFIRMA PAROLA", 9f, C_LABEL, FontStyles.Bold); rcL.characterSpacing = 3f;
        Rect(rcL, new Vector2(-130,-25), new Vector2(140,18));
        var regConf = MakeInput(rp, "RegisterConfirmPasswordInput", "repeta parola", new Vector2(0,-50), new Vector2(400,48));
        regConf.contentType = TMP_InputField.ContentType.Password;

        MakeBtn(rp, "RegisterButton", "CREEAZA CONT", new Vector2(0,-125), new Vector2(400,52), C_PURPLE, C_WHITE, true);
        MakeBtn(rp, "GoToLoginButton", "<< Inapoi la login", new Vector2(0,-185), new Vector2(400,40), C_BTNSEC, C_LABEL, false);

        // Feedback
        var fb = TMP(card, "FeedbackText", "", 12f, C_LABEL, FontStyles.Italic);
        Rect(fb, new Vector2(0,-295), new Vector2(420,28));
        fb.alignment = TextAlignmentOptions.Center;

        // Version Info
        var ver = TMP(canvas.transform, "VersionText", "v1.0.0 // SYSTEM: SECURE", 10f, new Color(0.3f,0.5f,0.7f,0.8f), FontStyles.Normal);
        Rect(ver, new Vector2(-15,20), new Vector2(300,20));
        var verR = ver.GetComponent<RectTransform>();
        verR.anchorMin = new Vector2(1,0); verR.anchorMax = new Vector2(1,0); verR.pivot = new Vector2(1,0);
        ver.alignment = TextAlignmentOptions.Right;
        ver.characterSpacing = 1f;

        // Managers
        var mgrs = GameObject.Find("Managers") ?? new GameObject("Managers");
        
        var apiGO = GameObject.Find("ApiManager") ?? new GameObject("ApiManager");
        apiGO.transform.SetParent(mgrs.transform);
        if (!apiGO.GetComponent<ApiManager>()) apiGO.AddComponent<ApiManager>();
        
        var authGO = GameObject.Find("AuthManager") ?? new GameObject("AuthManager");
        authGO.transform.SetParent(mgrs.transform);
        if (!authGO.GetComponent<AuthManager>()) authGO.AddComponent<AuthManager>();
        
        var pmGO = GameObject.Find("ProgressManager") ?? new GameObject("ProgressManager");
        pmGO.transform.SetParent(mgrs.transform);
        if (!pmGO.GetComponent<ProgressManager>()) pmGO.AddComponent<ProgressManager>();
        
        var lmGO = GameObject.Find("LoginUIManager") ?? new GameObject("LoginUIManager");
        lmGO.transform.SetParent(mgrs.transform);
        var lm = lmGO.GetComponent<LoginUIManager>() ?? lmGO.AddComponent<LoginUIManager>();

        lm.loginPanel                    = lp.gameObject;
        lm.registerPanel                 = rp.gameObject;
        lm.loginEmailInput               = emailInput;
        lm.loginPasswordInput            = passInput;
        lm.loginButton                   = loginBtn;
        lm.goToRegisterButton            = lp.transform.Find("GoToRegisterButton").GetComponent<Button>();
        lm.registerEmailInput            = regEmail;
        lm.registerPasswordInput         = regPass;
        lm.registerConfirmPasswordInput  = regConf;
        lm.registerButton                = rp.transform.Find("RegisterButton").GetComponent<Button>();
        lm.goToLoginButton               = rp.transform.Find("GoToLoginButton").GetComponent<Button>();
        lm.feedbackText                  = fb;
        lm.mainMenuSceneName             = "MainMenu";

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[OK] Login UI modernizat generat! Salvează cu Ctrl+S.");
    }

    // =========================================================
    [MenuItem("Tools/Build MainMenu UI")]
    public static void BuildMainMenuUI()
    {
        var canvas = SetupCanvas();
        CreateEventSystem();

        // 1. Background Image with Sprite fallback
        var bg = Img(canvas.transform, "BG", Color.white);
        StretchFull(bg);
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Texture/Simple_Background.png");
        if (bgSprite != null)
        {
            bg.sprite = bgSprite;
            bg.color = Color.white;
        }
        else
        {
            bg.color = C_BG;
        }

        // 2. Corner Bracket Accent Line (left side decorative edge)
        var line = Img(canvas.transform, "AccentLine", C_CYAN);
        var lr = line.GetComponent<RectTransform>();
        lr.anchorMin = new Vector2(0,0); lr.anchorMax = new Vector2(0,1);
        lr.offsetMin = Vector2.zero; lr.offsetMax = new Vector2(4,0);

        // 3. Central Card Container
        var border = RectGO(canvas.transform, "CardBorder", new Vector2(464,624), Vector2.zero);
        AddRoundedImage(border.gameObject, C_BORDER);
        var borderOutline = border.gameObject.AddComponent<Outline>();
        borderOutline.effectColor = new Color(C_CYAN.r, C_CYAN.g, C_CYAN.b, 0.35f);
        borderOutline.effectDistance = new Vector2(2f, -2f);

        var card = RectGO(canvas.transform, "Card", new Vector2(460,620), Vector2.zero);
        AddRoundedImage(card.gameObject, C_CARD);
        
        // Add futuristic corner accents!
        AddCornerAccents(card, new Vector2(460, 620), C_CYAN, 2f, 18f);

        // 4. Logo, Subtitle and Decors
        var logo = TMP(card, "Logo", "MEMORY\nARCHITECT", 56f, C_CYAN, FontStyles.Bold);
        Rect(logo, new Vector2(0,230), new Vector2(420,130));
        logo.alignment = TextAlignmentOptions.Center;
        logo.lineSpacing = -14f;

        var tagline = TMP(card, "Tagline", "RECONSTRUCT  •  REMEMBER  •  ESCAPE", 9f, C_LABEL, FontStyles.Normal);
        Rect(tagline, new Vector2(0,148), new Vector2(420,22));
        tagline.alignment = TextAlignmentOptions.Center;
        tagline.characterSpacing = 3f;

        var sep = Img(card, "Sep", C_BORDER);
        Rect(sep.GetComponent<RectTransform>(), new Vector2(0,128), new Vector2(380,1));

        // 5. Menu Buttons with Custom Hovers
        var startBtn    = MakeMenuBtn(card, "StartGameButton", "[ > ]  START RECONSTRUCTION", new Vector2(0, 58),  C_CYAN,   Color.black, true);
        var continueBtn = MakeMenuBtn(card, "ContinueButton",  "[ + ]  RESUME SEQUENCE",       new Vector2(0, -6),  C_BTNSEC, C_WHITE,     false);
        var optBtn      = MakeMenuBtn(card, "OptionsButton",   "[ * ]  CONFIG PANEL",          new Vector2(0,-70),  C_BTNSEC, C_WHITE,     false);
        var quitBtn     = MakeMenuBtn(card, "QuitButton",      "[ X ]  TERMINATE LINK",        new Vector2(0,-134), C_BTNSEC, C_DANGER,    false);

        var sep2 = Img(card, "Sep2", C_BORDER);
        Rect(sep2.GetComponent<RectTransform>(), new Vector2(0,-190), new Vector2(380,1));

        // 6. Connected Player Info UI
        var pTxt = TMP(card, "PlayerNameText", "CONNECTION STATUS: SECURED", 9f, C_SUCCESS, FontStyles.Normal);
        Rect(pTxt, new Vector2(0,-245), new Vector2(420,22));
        pTxt.alignment = TextAlignmentOptions.Center;
        pTxt.characterSpacing = 2f;

        // Version Info
        var ver = TMP(canvas.transform, "Ver", "v1.0.0 // GRID: ONLINE", 10f, new Color(0.3f,0.5f,0.7f,0.8f), FontStyles.Normal);
        Rect(ver, new Vector2(-15,20), new Vector2(300,20));
        var verR = ver.GetComponent<RectTransform>();
        verR.anchorMin = new Vector2(1,0); verR.anchorMax = new Vector2(1,0); verR.pivot = new Vector2(1,0);
        ver.alignment = TextAlignmentOptions.Right;
        ver.characterSpacing = 1f;

        // 7. Modals: Options Panel
        var optPanel = RectGO(canvas.transform, "OptionsPanel", new Vector2(504,430), Vector2.zero);
        AddRoundedImage(optPanel.gameObject, C_BORDER);
        
        var optOutline = optPanel.gameObject.AddComponent<Outline>();
        optOutline.effectColor = new Color(C_PURPLE.r, C_PURPLE.g, C_PURPLE.b, 0.4f);
        optOutline.effectDistance = new Vector2(2f, -2f);
        
        optPanel.gameObject.SetActive(false);

        var optInner = RectGO(optPanel, "OptionsInner", new Vector2(500,426), Vector2.zero);
        AddRoundedImage(optInner.gameObject, Hex("#060C17FA"));
        
        // Add corner accents to options panel!
        AddCornerAccents(optInner, new Vector2(500, 426), C_PURPLE, 2f, 18f);

        var optTitle = TMP(optInner, "OptTitle", "[ SYSTEM PROTOCOLS ]", 22f, C_CYAN, FontStyles.Bold);
        Rect(optTitle, new Vector2(0,165), new Vector2(460,40));
        optTitle.alignment = TextAlignmentOptions.Center;
        optTitle.characterSpacing = 2f;

        var optSep = Img(optInner, "OptSep", C_BORDER);
        Rect(optSep.GetComponent<RectTransform>(), new Vector2(0,135), new Vector2(440,1));

        var masterSlider = MakeSliderRow(optInner, "MasterVolumeSlider", "MASTER GAIN",     new Vector2(0,90), 0.8f);
        var musicSlider  = MakeSliderRow(optInner, "MusicVolumeSlider",  "AUDIO STREAM",    new Vector2(0,35), 0.6f);
        var sfxSlider    = MakeSliderRow(optInner, "SfxVolumeSlider",    "EFFECT NODES",    new Vector2(0,-20), 0.8f);
        var fsToggle     = MakeToggleRow(optInner, "FullscreenToggle",   "FULLSCREEN LINK", new Vector2(0,-75));

        var optSep2 = Img(optInner, "OptSep2", C_BORDER);
        Rect(optSep2.GetComponent<RectTransform>(), new Vector2(0,-115), new Vector2(440,1));

        var saveBtn  = MakeBtn(optInner, "SaveOptionsButton",  "COMMIT SETTINGS", new Vector2(-95,-155), new Vector2(195,46), C_CYAN,   Color.black, true);
        var closeBtn = MakeBtn(optInner, "CloseOptionsButton", "ABORT OPTIONS",   new Vector2(105,-155), new Vector2(195,46), C_BTNSEC, C_LABEL,    false);

        // Managers
        var mgrs = GameObject.Find("Managers") ?? new GameObject("Managers");
        
        var mmGO = GameObject.Find("MainMenuManager") ?? new GameObject("MainMenuManager");
        mmGO.transform.SetParent(mgrs.transform);
        var mm = mmGO.GetComponent<MainMenuManager>() ?? mmGO.AddComponent<MainMenuManager>();
        
        mm.startGameButton    = startBtn;
        mm.continueButton     = continueBtn;
        mm.optionsButton      = optBtn;
        mm.quitButton         = quitBtn;
        mm.optionsPanel       = optPanel.gameObject;
        mm.masterVolumeSlider = masterSlider;
        mm.musicVolumeSlider  = musicSlider;
        mm.sfxVolumeSlider    = sfxSlider;
        mm.fullscreenToggle   = fsToggle;
        mm.saveOptionsButton  = saveBtn;
        mm.closeOptionsButton = closeBtn;
        mm.firstSceneName     = "mainhol";
        mm.loginSceneName     = "LoginScene";
        mm.playerNameText     = pTxt;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[OK] MainMenu UI modernizat generat! Salvează cu Ctrl+S.");
    }

    // =========================================================
    // HELPERS & STYLE METHODS
    // =========================================================

    static void AddCornerAccents(Transform parent, Vector2 size, Color color, float thickness = 2f, float length = 16f)
    {
        // Top-Left corner bracket
        CreateLine(parent, "TL_H", new Vector2(-size.x/2 + length/2,  size.y/2 - thickness/2), new Vector2(length, thickness), color);
        CreateLine(parent, "TL_V", new Vector2(-size.x/2 + thickness/2, size.y/2 - length/2),    new Vector2(thickness, length), color);

        // Top-Right corner bracket
        CreateLine(parent, "TR_H", new Vector2( size.x/2 - length/2,  size.y/2 - thickness/2), new Vector2(length, thickness), color);
        CreateLine(parent, "TR_V", new Vector2( size.x/2 - thickness/2, size.y/2 - length/2),    new Vector2(thickness, length), color);

        // Bottom-Left corner bracket
        CreateLine(parent, "BL_H", new Vector2(-size.x/2 + length/2, -size.y/2 + thickness/2), new Vector2(length, thickness), color);
        CreateLine(parent, "BL_V", new Vector2(-size.x/2 + thickness/2, -size.y/2 + length/2),    new Vector2(thickness, length), color);

        // Bottom-Right corner bracket
        CreateLine(parent, "BR_H", new Vector2( size.x/2 - length/2, -size.y/2 + thickness/2), new Vector2(length, thickness), color);
        CreateLine(parent, "BR_V", new Vector2( size.x/2 - thickness/2, -size.y/2 + length/2),    new Vector2(thickness, length), color);
    }

    static void CreateLine(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
    {
        var line = RectGO(parent, name, size, pos);
        line.gameObject.AddComponent<Image>().color = color;
    }

    static Canvas SetupCanvas()
    {
        var go = GameObject.Find("Canvas") ?? new GameObject("Canvas");
        var c  = go.GetComponent<Canvas>() ?? go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        if (!go.GetComponent<CanvasScaler>())     go.AddComponent<CanvasScaler>();
        if (!go.GetComponent<GraphicRaycaster>()) go.AddComponent<GraphicRaycaster>();
        var cs = go.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920,1080);
        return c;
    }

    static void CreateEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    static Image Img(Transform parent, string name, Color color)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var img = go.AddComponent<Image>(); img.color = color; return img;
    }

    static Image AddRoundedImage(GameObject go, Color color)
    {
        var img = go.AddComponent<Image>();
        img.color = color;
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Texture/RoundedRect.png");
        if (sprite != null)
        {
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
        }
        return img;
    }

    static void StretchFull(Image img)
    {
        var r = img.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
        r.offsetMin = r.offsetMax = Vector2.zero;
    }

    static RectTransform RectGO(Transform parent, string name, Vector2 size, Vector2 pos)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false);
        var r = go.AddComponent<RectTransform>(); r.sizeDelta = size; r.anchoredPosition = pos; return r;
    }

    static TextMeshProUGUI TMP(Transform parent, string name, string text, float size, Color color, FontStyles style)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = size; t.color = color; t.fontStyle = style; return t;
    }

    static void Rect(Component c, Vector2 pos, Vector2 size)
    {
        var r = c.GetComponent<RectTransform>() ?? c.gameObject.AddComponent<RectTransform>();
        r.anchoredPosition = pos; r.sizeDelta = size;
    }

    static TMP_InputField MakeInput(Transform parent, string name, string ph, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false);
        var r = go.AddComponent<RectTransform>(); r.anchoredPosition = pos; r.sizeDelta = size;
        
        // Futuristic panel background + thin glow outline
        var img = AddRoundedImage(go, C_INPUT); 
        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(C_BORDER.r, C_BORDER.g, C_BORDER.b, 0.8f);
        outline.effectDistance = new Vector2(1f, -1f);

        var field = go.AddComponent<TMP_InputField>();

        var ta = new GameObject("Text Area"); ta.transform.SetParent(go.transform, false);
        var taR = ta.AddComponent<RectTransform>();
        taR.anchorMin = Vector2.zero; taR.anchorMax = Vector2.one;
        taR.offsetMin = new Vector2(16,4); taR.offsetMax = new Vector2(-16,-4);
        ta.AddComponent<RectMask2D>();

        var phGO = new GameObject("Placeholder"); phGO.transform.SetParent(ta.transform, false);
        var phR = phGO.AddComponent<RectTransform>();
        phR.anchorMin = Vector2.zero; phR.anchorMax = Vector2.one; phR.offsetMin = phR.offsetMax = Vector2.zero;
        var phT = phGO.AddComponent<TextMeshProUGUI>();
        phT.text = ph; phT.fontSize = 14f; phT.color = new Color(0.35f,0.45f,0.6f,0.65f); phT.fontStyle = FontStyles.Italic;

        var txGO = new GameObject("Text"); txGO.transform.SetParent(ta.transform, false);
        var txR = txGO.AddComponent<RectTransform>();
        txR.anchorMin = Vector2.zero; txR.anchorMax = Vector2.one; txR.offsetMin = txR.offsetMax = Vector2.zero;
        var txT = txGO.AddComponent<TextMeshProUGUI>(); txT.fontSize = 14f; txT.color = C_WHITE;

        field.textViewport = taR; field.textComponent = txT; field.placeholder = phT; return field;
    }

    static Button MakeBtn(Transform parent, string name, string label, Vector2 pos, Vector2 size,
        Color bg, Color textColor, bool bold)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false);
        var r = go.AddComponent<RectTransform>(); r.anchoredPosition = pos; r.sizeDelta = size;
        
        // Outlined design style for buttons
        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(C_BORDER.r, C_BORDER.g, C_BORDER.b, 0.6f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        var img = AddRoundedImage(go, bg); 
        var btn = go.AddComponent<Button>();
        btn.transition = Selectable.Transition.None; // Handled dynamically via UIButtonHover component
        
        var tx = new GameObject("Text"); tx.transform.SetParent(go.transform, false);
        var txR = tx.AddComponent<RectTransform>();
        txR.anchorMin = Vector2.zero; txR.anchorMax = Vector2.one;
        txR.offsetMin = txR.offsetMax = Vector2.zero;
        var t = tx.AddComponent<TextMeshProUGUI>();
        t.text = label; t.fontSize = 14f; t.color = textColor;
        t.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        t.alignment = TextAlignmentOptions.Center; t.characterSpacing = 2.5f;

        // Attach modern UIButtonHover scripts for micro-interactions
        var hover = go.AddComponent<UIButtonHover>();
        hover.useCustomColors = true;
        
        if (bg == C_CYAN) // Primary Neon Cyan button
        {
            hover.hoverBgColor = Hex("#00B8D4");
            hover.hoverTextColor = Color.black;
        }
        else if (bg == C_PURPLE) // Primary Neon Purple button
        {
            hover.hoverBgColor = Hex("#6D28D9");
            hover.hoverTextColor = C_WHITE;
        }
        else if (textColor == C_DANGER) // Danger/Abort button
        {
            hover.hoverBgColor = C_DANGER;
            hover.hoverTextColor = C_WHITE;
        }
        else // Secondary menu option buttons
        {
            hover.hoverBgColor = C_CYAN;
            hover.hoverTextColor = Color.black;
        }

        return btn;
    }

    static Button MakeMenuBtn(Transform parent, string name, string label, Vector2 pos,
        Color bg, Color textColor, bool primary)
    {
        var btn = MakeBtn(parent, name, label, pos, new Vector2(380,54), bg, textColor, primary);
        var t = btn.GetComponentInChildren<TextMeshProUGUI>();
        t.fontSize = 15f; 
        t.alignment = TextAlignmentOptions.MidlineLeft;
        t.margin = new Vector4(24,0,0,0); 
        t.characterSpacing = 3.5f;
        return btn;
    }

    static Slider MakeSliderRow(Transform parent, string sliderName, string label, Vector2 pos, float val)
    {
        var lbl = TMP(parent, sliderName+"Lbl", label, 10f, C_LABEL, FontStyles.Bold);
        Rect(lbl, new Vector2(-120, pos.y), new Vector2(160,20)); lbl.characterSpacing = 2f;

        var go = new GameObject(sliderName); go.transform.SetParent(parent, false);
        var r = go.AddComponent<RectTransform>(); r.anchoredPosition = new Vector2(100,pos.y); r.sizeDelta = new Vector2(200,16);
        var s = go.AddComponent<Slider>();

        var bgT = new GameObject("Bg"); bgT.transform.SetParent(go.transform, false);
        var bgR = bgT.AddComponent<RectTransform>(); bgR.anchorMin = new Vector2(0,.38f); bgR.anchorMax = new Vector2(1,.62f); bgR.offsetMin = bgR.offsetMax = Vector2.zero;
        var bgImg = AddRoundedImage(bgT, C_BTNSEC); 
        var bgOutline = bgT.AddComponent<Outline>();
        bgOutline.effectColor = C_BORDER;
        bgOutline.effectDistance = new Vector2(1f, -1f);

        var fill = new GameObject("Fill"); fill.transform.SetParent(go.transform, false);
        var fR = fill.AddComponent<RectTransform>(); fR.anchorMin = new Vector2(0,.38f); fR.anchorMax = new Vector2(1,.62f); fR.offsetMin = fR.offsetMax = Vector2.zero;
        AddRoundedImage(fill, C_CYAN);

        var handle = new GameObject("Handle"); handle.transform.SetParent(go.transform, false);
        var hR = handle.AddComponent<RectTransform>(); hR.sizeDelta = new Vector2(16,16);
        var hImg = AddRoundedImage(handle, C_WHITE);
        var hOutline = handle.AddComponent<Outline>();
        hOutline.effectColor = C_CYAN;
        hOutline.effectDistance = new Vector2(1f, -1f);

        s.fillRect = fR; s.handleRect = hR; s.value = val; return s;
    }

    static Toggle MakeToggleRow(Transform parent, string toggleName, string label, Vector2 pos)
    {
        var lbl = TMP(parent, toggleName+"Lbl", label, 10f, C_LABEL, FontStyles.Bold);
        Rect(lbl, new Vector2(-120,pos.y), new Vector2(160,20)); lbl.characterSpacing = 2f;

        var go = new GameObject(toggleName); go.transform.SetParent(parent, false);
        var r = go.AddComponent<RectTransform>(); r.anchoredPosition = new Vector2(110,pos.y); r.sizeDelta = new Vector2(44,24);
        var tgl = go.AddComponent<Toggle>();

        var bg = new GameObject("Bg"); bg.transform.SetParent(go.transform, false);
        var bgR = bg.AddComponent<RectTransform>(); bgR.anchorMin = Vector2.zero; bgR.anchorMax = Vector2.one; bgR.offsetMin = bgR.offsetMax = Vector2.zero;
        var bgImg = AddRoundedImage(bg, C_BTNSEC); 
        var bgOutline = bg.AddComponent<Outline>();
        bgOutline.effectColor = C_BORDER;
        bgOutline.effectDistance = new Vector2(1f, -1f);

        var ck = new GameObject("Check"); ck.transform.SetParent(bg.transform, false);
        var ckR = ck.AddComponent<RectTransform>(); ckR.anchorMin = Vector2.zero; ckR.anchorMax = Vector2.one; ckR.offsetMin = new Vector2(4,4); ckR.offsetMax = new Vector2(-4,-4);
        var ckImg = AddRoundedImage(ck, C_CYAN);

        tgl.targetGraphic = bgImg; tgl.graphic = ckImg; tgl.isOn = true; return tgl;
    }
}
