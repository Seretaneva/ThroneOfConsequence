using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MedievalButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    private static readonly Color NormalColor = new Color(0.30f, 0.16f, 0.07f, 1f);
    private static readonly Color HoverColor = new Color(0.43f, 0.24f, 0.09f, 1f);
    private static readonly Color PressedColor = new Color(0.18f, 0.09f, 0.04f, 1f);
    private static readonly Color DisabledColor = new Color(0.18f, 0.15f, 0.12f, 0.72f);
    private static readonly Color GoldColor = new Color(0.86f, 0.58f, 0.20f, 1f);
    private static readonly Color BrightGoldColor = new Color(1f, 0.76f, 0.32f, 1f);
    private static readonly Color TextColor = new Color(1f, 0.91f, 0.70f, 1f);

    private const float HoverScale = 1.045f;
    private const float PressedScale = 0.965f;
    private const float AnimationSpeed = 14f;

    private RectTransform rectTransform;
    private Button button;
    private Image image;
    private TMP_Text[] labels;
    private Vector3 baseScale;
    private Vector3 targetScale;
    private Color targetColor;
    private bool pointerInside;
    private bool pointerDown;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        labels = GetComponentsInChildren<TMP_Text>(true);
        baseScale = rectTransform.localScale;
        targetScale = baseScale;

        ApplyMedievalStyle();
        RefreshTargetState();
    }

    private void OnEnable()
    {
        RefreshTargetState();
    }

    private void Update()
    {
        RefreshTargetState();

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.unscaledDeltaTime * AnimationSpeed
        );

        if (image != null)
            image.color = Color.Lerp(image.color, targetColor, Time.unscaledDeltaTime * AnimationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        RefreshTargetState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        pointerDown = false;
        RefreshTargetState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        RefreshTargetState();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        RefreshTargetState();
    }

    public void OnSelect(BaseEventData eventData)
    {
        pointerInside = true;
        RefreshTargetState();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        pointerInside = false;
        pointerDown = false;
        RefreshTargetState();
    }

    private void ApplyMedievalStyle()
    {
        if (button != null)
            button.transition = Selectable.Transition.None;

        if (image != null)
        {
            image.color = NormalColor;
            image.raycastTarget = true;
        }

        AddShadow();
        AddOutline();
        AddTrim("TopGoldTrim", 0.93f, 1f, GoldColor);
        AddTrim("BottomDarkTrim", 0f, 0.08f, new Color(0.08f, 0.035f, 0.015f, 0.95f));
        AddTrim("InnerHighlight", 0.70f, 0.82f, new Color(1f, 0.83f, 0.45f, 0.18f));
        StyleLabels();
    }

    private void AddOutline()
    {
        Outline outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();

        outline.effectColor = GoldColor;
        outline.effectDistance = new Vector2(3f, -3f);
    }

    private void AddShadow()
    {
        UnityEngine.UI.Shadow shadow = GetExactShadow();
        if (shadow == null)
            shadow = gameObject.AddComponent<UnityEngine.UI.Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, 0.62f);
        shadow.effectDistance = new Vector2(5f, -5f);
    }

    private void AddTrim(string trimName, float anchorMinY, float anchorMaxY, Color color)
    {
        Transform existing = transform.Find(trimName);
        Image trimImage;
        RectTransform trimRect;

        if (existing == null)
        {
            GameObject trim = new GameObject(trimName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            trim.transform.SetParent(transform, false);
            trim.transform.SetAsFirstSibling();
            trimImage = trim.GetComponent<Image>();
            trimRect = trim.GetComponent<RectTransform>();
        }
        else
        {
            trimImage = existing.GetComponent<Image>();
            trimRect = existing.GetComponent<RectTransform>();
        }

        trimImage.color = color;
        trimImage.raycastTarget = false;

        trimRect.anchorMin = new Vector2(0.035f, anchorMinY);
        trimRect.anchorMax = new Vector2(0.965f, anchorMaxY);
        trimRect.offsetMin = Vector2.zero;
        trimRect.offsetMax = Vector2.zero;
    }

    private void StyleLabels()
    {
        foreach (TMP_Text label in labels)
        {
            if (label == null)
                continue;

            label.color = TextColor;
            label.fontStyle |= FontStyles.Bold;

            UnityEngine.UI.Shadow shadow = GetExactShadow(label.gameObject);
            if (shadow == null)
                shadow = label.gameObject.AddComponent<UnityEngine.UI.Shadow>();

            shadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
            shadow.effectDistance = new Vector2(2f, -2f);

            Outline outline = label.GetComponent<Outline>();
            if (outline == null)
                outline = label.gameObject.AddComponent<Outline>();

            outline.effectColor = new Color(0.05f, 0.02f, 0.01f, 0.9f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }
    }

    private UnityEngine.UI.Shadow GetExactShadow()
    {
        return GetExactShadow(gameObject);
    }

    private UnityEngine.UI.Shadow GetExactShadow(GameObject target)
    {
        UnityEngine.UI.Shadow[] shadows = target.GetComponents<UnityEngine.UI.Shadow>();

        foreach (UnityEngine.UI.Shadow shadow in shadows)
        {
            if (shadow != null && shadow.GetType() == typeof(UnityEngine.UI.Shadow))
                return shadow;
        }

        return null;
    }

    private void RefreshTargetState()
    {
        bool isInteractable = button == null || button.interactable;

        if (!isInteractable)
        {
            targetScale = baseScale;
            targetColor = DisabledColor;
            return;
        }

        if (pointerDown)
        {
            targetScale = baseScale * PressedScale;
            targetColor = PressedColor;
            return;
        }

        if (pointerInside)
        {
            float pulse = Mathf.Sin(Time.unscaledTime * 7f) * 0.008f;
            targetScale = baseScale * (HoverScale + pulse);
            targetColor = Color.Lerp(HoverColor, BrightGoldColor, 0.18f);
            return;
        }

        targetScale = baseScale;
        targetColor = NormalColor;
    }
}
