using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

/// <summary>
/// Adauga efecte vizuale moderne de hover (scalare fluida, tranzitii de culori)
/// pentru butoanele din interfata de login si main menu.
/// </summary>
public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Animation")]
    public Vector3 hoverScale = new Vector3(1.03f, 1.03f, 1.03f);
    public float transitionSpeed = 12f;

    [Header("Color Transitions")]
    public bool useCustomColors = true;
    public Color hoverBgColor = new Color(0f, 0.9f, 1f, 1f); // Neon Cyan
    public Color hoverTextColor = Color.black;

    private Vector3 originalScale;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;

    private Color originalBgColor;
    private Color originalTextColor;
    private bool initialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) return;
        originalScale = transform.localScale;
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonImage != null) originalBgColor = buttonImage.color;
        if (buttonText != null) originalTextColor = buttonText.color;
        initialized = true;
    }

    private void OnEnable()
    {
        Initialize();
        // Reset scale and colors when UI panel is toggled
        transform.localScale = originalScale;
        if (buttonImage != null) buttonImage.color = originalBgColor;
        if (buttonText != null) buttonText.color = originalTextColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(hoverScale));
        if (useCustomColors)
        {
            StartCoroutine(ColorTo(hoverBgColor, hoverTextColor));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale));
        if (useCustomColors)
        {
            StartCoroutine(ColorTo(originalBgColor, originalTextColor));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale * 0.97f)); // Small squish effect on click
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(hoverScale));
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        transform.localScale = targetScale;
    }

    private IEnumerator ColorTo(Color targetBg, Color targetText)
    {
        float elapsed = 0f;
        float duration = 0.12f;
        Color startBg = buttonImage != null ? buttonImage.color : Color.white;
        Color startText = buttonText != null ? buttonText.color : Color.white;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (buttonImage != null) buttonImage.color = Color.Lerp(startBg, targetBg, t);
            if (buttonText != null) buttonText.color = Color.Lerp(startText, targetText, t);
            yield return null;
        }

        if (buttonImage != null) buttonImage.color = targetBg;
        if (buttonText != null) buttonText.color = targetText;
    }
}
