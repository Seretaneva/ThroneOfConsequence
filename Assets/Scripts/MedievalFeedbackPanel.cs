using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MedievalFeedbackPanel : MonoBehaviour
{
    private static readonly Color PanelColor = new Color(0.12f, 0.065f, 0.035f, 0.68f);
    private static readonly Color GoldColor = new Color(0.88f, 0.60f, 0.20f, 0.92f);
    private static readonly Color TextColor = new Color(1f, 0.92f, 0.72f, 1f);
    private static readonly Color StatColor = new Color(0.98f, 0.78f, 0.38f, 1f);

    private TMP_Text reasonText;
    private TMP_Text statsText;
    private TMP_FontAsset medievalFont;
    private RectTransform sealTransform;
    private Image sealImage;
    private bool loading;
    private string loadingBaseText = "Se analizeaza raspunsul";

    public void Initialize(TMP_Text reason, TMP_Text stats)
    {
        reasonText = reason;
        statsText = stats;
        medievalFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Bangers SDF");

        ApplyPanelStyle();
        ApplyTextStyle(reasonText, TextColor, 38f);
        ApplyTextStyle(statsText, StatColor, 30f);
        CreateLoadingSeal();
        ShowResult();
    }

    private void Update()
    {
        if (!loading)
            return;

        float time = Time.unscaledTime;
        int dotCount = Mathf.FloorToInt(time * 2.8f) % 4;

        if (reasonText != null)
            reasonText.text = loadingBaseText + new string('.', dotCount);

        if (sealTransform != null)
        {
            sealTransform.Rotate(0f, 0f, -180f * Time.unscaledDeltaTime);
            float pulse = 1f + Mathf.Sin(time * 5f) * 0.12f;
            sealTransform.localScale = Vector3.one * pulse;
        }

        if (sealImage != null)
        {
            float glow = 0.72f + Mathf.Sin(time * 5f) * 0.2f;
            sealImage.color = new Color(1f, 0.72f, 0.24f, glow);
        }
    }

    public void ShowLoading(string text = "Se analizeaza raspunsul")
    {
        loadingBaseText = text;
        loading = true;

        if (sealTransform != null)
            sealTransform.gameObject.SetActive(true);

        if (statsText != null)
            statsText.text = "";
    }

    public void ShowResult()
    {
        loading = false;

        if (sealTransform != null)
        {
            sealTransform.gameObject.SetActive(false);
            sealTransform.localScale = Vector3.one;
        }
    }

    private void ApplyPanelStyle()
    {
        Image panelImage = GetComponent<Image>();
        if (panelImage == null)
            panelImage = gameObject.AddComponent<Image>();

        panelImage.color = PanelColor;
        panelImage.raycastTarget = true;

        Outline outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();

        outline.effectColor = GoldColor;
        outline.effectDistance = new Vector2(4f, -4f);

        UnityEngine.UI.Shadow shadow = GetExactShadow(gameObject);
        if (shadow == null)
            shadow = gameObject.AddComponent<UnityEngine.UI.Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
        shadow.effectDistance = new Vector2(8f, -8f);

        AddTrim("FeedbackTopGoldTrim", 0.955f, 1f, GoldColor);
        AddTrim("FeedbackBottomGoldTrim", 0f, 0.045f, new Color(0.68f, 0.38f, 0.09f, 0.86f));
        AddTrim("FeedbackInnerGlow", 0.84f, 0.92f, new Color(1f, 0.78f, 0.32f, 0.16f));
    }

    private void ApplyTextStyle(TMP_Text text, Color color, float minSize)
    {
        if (text == null)
            return;

        if (medievalFont != null)
            text.font = medievalFont;

        text.color = color;
        text.fontStyle |= FontStyles.Bold;
        text.characterSpacing = 1.5f;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Max(18f, minSize * 0.55f);
        text.fontSizeMax = minSize;
        text.alignment = TextAlignmentOptions.Center;

        Outline outline = text.GetComponent<Outline>();
        if (outline == null)
            outline = text.gameObject.AddComponent<Outline>();

        outline.effectColor = new Color(0.04f, 0.015f, 0.005f, 0.92f);
        outline.effectDistance = new Vector2(1.8f, -1.8f);

        UnityEngine.UI.Shadow shadow = GetExactShadow(text.gameObject);
        if (shadow == null)
            shadow = text.gameObject.AddComponent<UnityEngine.UI.Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, 0.68f);
        shadow.effectDistance = new Vector2(3f, -3f);
    }

    private void CreateLoadingSeal()
    {
        Transform existing = transform.Find("LoadingSeal");
        GameObject sealObject;

        if (existing == null)
        {
            sealObject = new GameObject("LoadingSeal", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            sealObject.transform.SetParent(transform, false);
        }
        else
        {
            sealObject = existing.gameObject;
        }

        sealTransform = sealObject.GetComponent<RectTransform>();
        sealImage = sealObject.GetComponent<Image>();

        sealTransform.anchorMin = new Vector2(0.5f, 0.5f);
        sealTransform.anchorMax = new Vector2(0.5f, 0.5f);
        sealTransform.anchoredPosition = new Vector2(0f, -105f);
        sealTransform.sizeDelta = new Vector2(42f, 42f);
        sealTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);

        sealImage.color = GoldColor;
        sealImage.raycastTarget = false;
        sealObject.SetActive(false);
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
}
