using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventUIController : MonoBehaviour
{
    [Header("Event Text")]
    [SerializeField] private TMP_Text eventTitleText;
    [SerializeField] private TMP_Text eventDescriptionText;

    [Header("Choice Texts")]
    [SerializeField] private TMP_Text choiceAText;
    [SerializeField] private TMP_Text choiceBText;
    [SerializeField] private TMP_Text choiceCText;

    [Header("Free Text")]
    [SerializeField] private TMP_InputField freeTextInput;

    [Header("Panels")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private GameObject feedbackPanel;

    [Header("Feedback")]
    [SerializeField] private TMP_Text feedbackReasonText;
    [SerializeField] private TMP_Text feedbackStatsText;

    [Header("Fonts")]
    [SerializeField] private TMP_FontAsset titleFont;
    [SerializeField] private TMP_FontAsset bodyFont;
    [SerializeField] private TMP_FontAsset italicFont;

    [Header("Evaluators")]
    [SerializeField] private OllamaEvaluator ollamaEvaluator;

    [SerializeField] private AudienceSequenceController audienceSequenceController;

    private RuleBasedEvaluator ruleBasedEvaluator = new RuleBasedEvaluator();
    private EventData currentEvent;
    private ChoiceData lastResolvedChoice;

    private int pendingGoldEffect;
    private int pendingRespectEffect;
    private int pendingIntelligenceEffect;
    private string defaultFreeTextPlaceholder;
    private MedievalFeedbackPanel medievalFeedbackPanel;
    private Button continueButton;
    private TMP_Text continueButtonText;
    private string defaultContinueButtonText;
    private bool gameOverDisplayed;
  
    private void OnValidate()
    {
        ApplyFontPreview(eventTitleText, titleFont);
        ApplyFontPreview(choiceAText, titleFont);
        ApplyFontPreview(choiceBText, titleFont);
        ApplyFontPreview(choiceCText, titleFont);
        ApplyFontPreview(feedbackStatsText, titleFont);

        ApplyFontPreview(eventDescriptionText, bodyFont);
        ApplyFontPreview(feedbackReasonText, bodyFont);

        if (freeTextInput != null)
        {
            ApplyFontPreview(freeTextInput.textComponent, bodyFont);

            if (freeTextInput.placeholder is TMP_Text placeholderText)
                ApplyFontPreview(placeholderText, italicFont);
        }

        ApplyButtonFontPreview(choicesPanel);
        ApplyButtonFontPreview(feedbackPanel);
    }

    private void ApplyFontPreview(TMP_Text text, TMP_FontAsset font)
    {
        if (text == null || font == null)
            return;

        text.font = font;
        text.fontStyle = FontStyles.Normal;
    }

    private void ApplyButtonFontPreview(GameObject panel)
    {
        if (panel == null || titleFont == null)
            return;

        foreach (Button button in panel.GetComponentsInChildren<Button>(true))
        {
            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            ApplyFontPreview(label, titleFont);
        }
    }

    private void Start()
    {
        titleFont ??= PlayfairFontProvider.SemiBold;
        bodyFont ??= PlayfairFontProvider.Regular;
        italicFont ??= PlayfairFontProvider.Italic;
        CacheDefaultFreeTextPlaceholder();
        ApplyMedievalButtonStyle();
        ApplyMedievalTextStyle();
        ApplyMedievalFeedbackStyle();
        CacheContinueButton();
        HideEventUI();
//         RoyalChronicle.Instance.AddEntry(
//         "You became Village Leader."
// );
    }

    private void ShowChoicesPanel()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(true);

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    private void ShowFeedbackPanel()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(false);

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
            feedbackPanel.transform.SetAsLastSibling();
        }
    }

    private void ClearFeedback()
    {
        if (feedbackReasonText != null)
            feedbackReasonText.text = "";

        if (feedbackStatsText != null)
            feedbackStatsText.text = "";
    }

    private void CacheContinueButton()
    {
        if (feedbackPanel == null)
            return;

        Button[] buttons = feedbackPanel.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (button != null && button.gameObject.name == "ContinueButton")
            {
                continueButton = button;
                continueButtonText = button.GetComponentInChildren<TMP_Text>(true);

                if (continueButtonText != null)
                    defaultContinueButtonText = continueButtonText.text;

                return;
            }
        }
    }

    private void CacheDefaultFreeTextPlaceholder()
    {
        if (freeTextInput != null && freeTextInput.placeholder is TMP_Text placeholderText)
            defaultFreeTextPlaceholder = placeholderText.text;
    }

    private void SetFreeTextPlaceholder(string text)
    {
        if (text != null && freeTextInput != null && freeTextInput.placeholder is TMP_Text placeholderText)
            placeholderText.text = text;
    }

    private void ApplyMedievalButtonStyle()
    {
        ConfigureChoicesLayout();
        AddMedievalStyleToButtons(choicesPanel);
        AddMedievalStyleToButtons(feedbackPanel);
    }

    private void ConfigureChoicesLayout()
    {
        if (choicesPanel == null)
            return;

        RectTransform panelRect = choicesPanel.GetComponent<RectTransform>();
        if (panelRect != null)
            panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, 690f);

        VerticalLayoutGroup layout = choicesPanel.GetComponent<VerticalLayoutGroup>();
        if (layout != null)
        {
            layout.padding = new RectOffset(24, 24, 12, 12);
            layout.spacing = 14f;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = false;
        }
    }

    private void AddMedievalStyleToButtons(GameObject panel)
    {
        if (panel == null)
            return;

        Button[] buttons = panel.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (button != null && button.GetComponent<MedievalButtonAnimator>() == null)
                button.gameObject.AddComponent<MedievalButtonAnimator>();

            TMP_Text buttonText = button != null
                ? button.GetComponentInChildren<TMP_Text>(true)
                : null;

            if (buttonText != null && titleFont != null)
            {
                buttonText.font = titleFont;
                buttonText.fontStyle = FontStyles.Normal;
                buttonText.characterSpacing = 0f;
            }

            if (button != null && button.gameObject.name.StartsWith("Choice"))
                ConfigureChoiceButton(button);
        }
    }

    private void ConfigureChoiceButton(Button button)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        if (buttonRect != null)
            buttonRect.sizeDelta = new Vector2(900f, 118f);

        LayoutElement layoutElement = button.GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = button.gameObject.AddComponent<LayoutElement>();

        layoutElement.minHeight = 112f;
        layoutElement.preferredHeight = 118f;
        layoutElement.flexibleHeight = 0f;
    }

    private void ApplyMedievalFeedbackStyle()
    {
        if (feedbackPanel == null)
            return;

        medievalFeedbackPanel = feedbackPanel.GetComponent<MedievalFeedbackPanel>();

        if (medievalFeedbackPanel == null)
            medievalFeedbackPanel = feedbackPanel.AddComponent<MedievalFeedbackPanel>();

        medievalFeedbackPanel.Initialize(
            feedbackReasonText,
            feedbackStatsText,
            bodyFont,
            titleFont
        );
    }

    private void ApplyMedievalTextStyle()
    {
        StyleMedievalText(eventTitleText, titleFont, 46f, new Color(1f, 0.88f, 0.46f, 1f), 0.14f);
        StyleMedievalText(eventDescriptionText, bodyFont, 34f, new Color(1f, 0.985f, 0.91f, 1f), 0.09f);
        StyleChoiceText(choiceAText);
        StyleChoiceText(choiceBText);
        StyleChoiceText(choiceCText);

        if (freeTextInput != null)
        {
            StyleMedievalText(freeTextInput.textComponent, bodyFont, 29f, new Color(1f, 0.97f, 0.86f, 1f), 0.04f);

            if (freeTextInput.placeholder is TMP_Text placeholderText)
                StyleMedievalText(placeholderText, italicFont, 25f, new Color(1f, 0.86f, 0.54f, 0.82f), 0.03f);

            Image inputImage = freeTextInput.GetComponent<Image>();
            if (inputImage != null)
                inputImage.color = new Color(0.11f, 0.06f, 0.03f, 0.58f);
        }
    }

    private void StyleMedievalText(
        TMP_Text text,
        TMP_FontAsset font,
        float maxSize,
        Color color,
        float outlineWidth)
    {
        if (text == null)
            return;

        if (font != null)
            text.font = font;

        text.color = color;
        text.fontStyle = FontStyles.Normal;
        text.characterSpacing = 0f;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Max(18f, maxSize * 0.62f);
        text.fontSizeMax = maxSize;
        text.outlineColor = new Color32(18, 8, 2, 255);
        text.outlineWidth = outlineWidth;

        Outline outline = text.GetComponent<Outline>();
        if (outline == null)
            outline = text.gameObject.AddComponent<Outline>();

        outline.effectColor = new Color(0.02f, 0.01f, 0.004f, 0.82f);
        outline.effectDistance = new Vector2(1f, -1f);

        Shadow shadow = GetExactShadow(text.gameObject);
        if (shadow == null)
            shadow = text.gameObject.AddComponent<Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, 0.62f);
        shadow.effectDistance = new Vector2(1.5f, -1.5f);
    }

    private void StyleChoiceText(TMP_Text text)
    {
        StyleMedievalText(
            text,
            titleFont,
            38f,
            new Color(1f, 0.94f, 0.74f, 1f),
            0.07f
        );

        if (text == null)
            return;

        text.fontSizeMin = 25f;
        text.fontSizeMax = 38f;
        text.alignment = TextAlignmentOptions.Center;
        text.lineSpacing = 5f;
        text.margin = new Vector4(12f, 6f, 12f, 6f);
    }

    private Shadow GetExactShadow(GameObject target)
    {
        Shadow[] shadows = target.GetComponents<Shadow>();

        foreach (Shadow shadow in shadows)
        {
            if (shadow != null && shadow.GetType() == typeof(Shadow))
                return shadow;
        }

        return null;
    }

    public void ChooseA()
    {
        if (currentEvent != null)
            ResolveChoice(currentEvent.choiceA);
    }

    public void ChooseB()
    {
        if (currentEvent != null)
            ResolveChoice(currentEvent.choiceB);
    }

    public void ChooseC()
    {
        if (currentEvent != null)
            ResolveChoice(currentEvent.choiceC);
    }

    private void ResolveChoice(ChoiceData choice)
    {
        if (choice == null)
        {
            Debug.LogError("Choice is null.");
            return;
        }

        if (!ChoiceProcessor.CanApplyChoice(choice, out string failureReason))
        {
            if (feedbackReasonText != null)
                feedbackReasonText.text = failureReason;

            if (feedbackStatsText != null)
                feedbackStatsText.text = "";

            ShowFeedbackPanel();
            return;
        }

        lastResolvedChoice = choice;

        if (!ChoiceProcessor.ApplyChoice(choice))
            return;

        pendingGoldEffect = choice.effects.gold - choice.goldCost;
        pendingRespectEffect = choice.effects.respect;
        pendingIntelligenceEffect = choice.effects.intelligence;

        if (feedbackReasonText != null)
            feedbackReasonText.text = choice.consequenceText;

        if (feedbackStatsText != null)
            feedbackStatsText.text = FormatStatEffects(
                pendingGoldEffect,
                pendingRespectEffect,
                pendingIntelligenceEffect
            );

        if (medievalFeedbackPanel != null)
            medievalFeedbackPanel.ShowResult();

        if (TryShowGameOver())
            return;

        ShowFeedbackPanel();
        // RoyalChronicle.Instance.AddEntry(
        // choice.choiceText
        // );
    }

    public void SubmitFreeTextResponse()
    {
        if (freeTextInput == null)
        {
            Debug.LogError("freeTextInput nu este legat in Inspector.");
            return;
        }

        if (currentEvent == null)
        {
            Debug.LogError("currentEvent este null.");
            return;
        }

        if (IsExclusiveBuildingEvent())
        {
            ClearFeedback();
            SetFreeTextPlaceholder("Alege una dintre cele trei constructii.");
            ShowChoicesPanel();
            return;
        }

        string playerResponse = freeTextInput.text;

        if (string.IsNullOrWhiteSpace(playerResponse))
        {
            ClearFeedback();
            SetFreeTextPlaceholder("Scrie un raspuns mai intai.");
            ShowChoicesPanel();
            freeTextInput.ActivateInputField();
            return;
        }

        SetFreeTextPlaceholder(defaultFreeTextPlaceholder);

        if (medievalFeedbackPanel != null)
        {
            medievalFeedbackPanel.ShowLoading();
        }
        else
        {
            if (feedbackReasonText != null)
                feedbackReasonText.text = "Se analizeaza raspunsul...";

            if (feedbackStatsText != null)
                feedbackStatsText.text = "";
        }

        ShowFeedbackPanel();

        Debug.Log("SUBMIT APASAT");
        Debug.Log("Text: " + playerResponse);

        if (ollamaEvaluator == null)
        {
            Debug.LogWarning("OllamaEvaluator lipseste. Folosesc fallback local.");
            StatEvaluationResult fallbackResult = ruleBasedEvaluator.Evaluate(playerResponse);
            ApplyEvaluationResult(fallbackResult);
            return;
        }

        StartCoroutine(ollamaEvaluator.EvaluateResponse(
            currentEvent.eventTitle,
            currentEvent.description,
            playerResponse,
            onSuccess: result =>
            {
                ApplyEvaluationResult(result);
            },
            onError: error =>
            {
                Debug.LogWarning("Ollama failed: " + error);

                StatEvaluationResult fallbackResult = ruleBasedEvaluator.Evaluate(playerResponse);
                fallbackResult.reason = "AI-ul nu a raspuns. Folosesc evaluare locala.";

                ApplyEvaluationResult(fallbackResult);
            }
        ));
    }

    private void ApplyEvaluationResult(StatEvaluationResult result)
    {
        Debug.Log("APPLY RESULT A FOST CHEMAT");

        if (result == null)
        {
            Debug.LogError("Evaluation result is null.");
            return;
        }

        pendingGoldEffect = result.goldEffect;
        pendingRespectEffect = result.respectEffect;
        pendingIntelligenceEffect = result.intelligenceEffect;

        if (feedbackReasonText != null)
        {
            Debug.Log("===========================> " + result.reason);
            feedbackReasonText.text = result.reason;
        }
        else
            Debug.LogError("feedbackReasonText este null.");

        if (feedbackStatsText != null)
            feedbackStatsText.text = FormatStatEffects(
                pendingGoldEffect,
                pendingRespectEffect,
                pendingIntelligenceEffect
            );
        else
            Debug.LogError("feedbackStatsText este null.");

        if (GameState.Instance != null)
        {
            GameState.Instance.AddGold(result.goldEffect);
            GameState.Instance.AddRespect(result.respectEffect);
            GameState.Instance.AddIntelligence(result.intelligenceEffect);
        }
        else
        {
            Debug.LogError("GameState.Instance este null.");
        }

        if (freeTextInput != null)
            freeTextInput.text = "";

        SetFreeTextPlaceholder(defaultFreeTextPlaceholder);

        lastResolvedChoice = null;

        if (medievalFeedbackPanel != null)
            medievalFeedbackPanel.ShowResult();

        if (TryShowGameOver())
            return;

        ShowFeedbackPanel();
    }

    public void ContinueToNextEvent()
    {
        if (gameOverDisplayed || (GameState.Instance != null && GameState.Instance.IsGameOver))
        {
            RestartGame();
            return;
        }

        EventManager.Instance.AdvanceAfterResolvedEvent(lastResolvedChoice);

        HideEventUI();

        if (audienceSequenceController != null)
            audienceSequenceController.FinishCurrentAudienceAndWaitForKnock();
    }

    private bool TryShowGameOver()
    {
        if (GameState.Instance == null || !GameState.Instance.IsGameOver)
            return false;

        gameOverDisplayed = true;

        if (feedbackReasonText != null)
        {
            feedbackReasonText.text =
                "Domnia s-a incheiat: " + GameState.Instance.GetGameOverTitle() +
                "\n" + GameState.Instance.GetGameOverDescription();
        }

        if (feedbackStatsText != null)
        {
            feedbackStatsText.text =
                "Ziua " + GameState.Instance.Day +
                " | Rang: " + GameState.Instance.CurrentRank;
        }

        if (continueButtonText != null)
            continueButtonText.text = "Incepe o domnie noua";

        if (medievalFeedbackPanel != null)
            medievalFeedbackPanel.ShowResult();

        ShowFeedbackPanel();
        return true;
    }

    private void RestartGame()
    {
        GameFlags.ClearFlags();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string FormatStatEffects(int gold, int respect, int intelligence)
    {
        string goldText = gold >= 0 ? $"+{gold}" : gold.ToString();
        string respectText = respect >= 0 ? $"+{respect}" : respect.ToString();
        string intelligenceText = intelligence >= 0 ? $"+{intelligence}" : intelligence.ToString();

        return $"Gold: {goldText} | Respect: {respectText} | Intelligence: {intelligenceText}";
    }

    public void HideEventUI()
    {
        if (eventTitleText != null)
            eventTitleText.text = "";

        if (eventDescriptionText != null)
            eventDescriptionText.text = "";

        if (choiceAText != null)
            choiceAText.text = "";

        if (choiceBText != null)
            choiceBText.text = "";

        if (choiceCText != null)
            choiceCText.text = "";

        if (freeTextInput != null)
            freeTextInput.text = "";

        SetFreeTextPlaceholder(defaultFreeTextPlaceholder);

        ClearFeedback();
        if (medievalFeedbackPanel != null)
            medievalFeedbackPanel.ShowResult();

        if (choicesPanel != null)
            choicesPanel.SetActive(false);

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    public void ShowEvent(EventData eventData)
    {
        currentEvent = eventData;
        gameOverDisplayed = false;

        if (continueButtonText != null)
            continueButtonText.text = defaultContinueButtonText;

        if (currentEvent == null)
        {
            Debug.LogError("No current event found.");
            return;
        }

        if (eventTitleText != null)
            eventTitleText.text = currentEvent.eventTitle;

        if (eventDescriptionText != null)
            eventDescriptionText.text = currentEvent.description;

        if (choiceAText != null)
            SetChoicePresentation(choiceAText, currentEvent.choiceA);

        if (choiceBText != null)
            SetChoicePresentation(choiceBText, currentEvent.choiceB);

        if (choiceCText != null)
            SetChoicePresentation(choiceCText, currentEvent.choiceC);

        if (freeTextInput != null)
        {
            freeTextInput.text = "";
            freeTextInput.interactable = !IsExclusiveBuildingEvent();
        }

        lastResolvedChoice = null;
        SetFreeTextPlaceholder(
            IsExclusiveBuildingEvent()
                ? "Alege una dintre cele trei constructii."
                : defaultFreeTextPlaceholder
        );
        ClearFeedback();
        ShowChoicesPanel();
    }

    private void SetChoicePresentation(TMP_Text choiceText, ChoiceData choice)
    {
        if (choiceText == null)
            return;

        Button choiceButton = choiceText.GetComponentInParent<Button>();

        if (choice == null)
        {
            choiceText.text = "";

            if (choiceButton != null)
                choiceButton.interactable = false;

            return;
        }

        bool canApply = ChoiceProcessor.CanApplyChoice(choice, out string failureReason);
        string details = "";

        if (choice.goldCost > 0)
            details = "Cost: " + choice.goldCost + " aur";

        if (!canApply)
        {
            if (!string.IsNullOrEmpty(details))
                details += " | ";

            details += failureReason;
        }

        choiceText.text = string.IsNullOrEmpty(details)
            ? choice.choiceText
            : choice.choiceText + "\n<size=75%>" + details + "</size>";

        if (choiceButton != null)
            choiceButton.interactable = canApply;
    }

    private bool IsExclusiveBuildingEvent()
    {
        return currentEvent != null &&
               currentEvent.tags != null &&
               currentEvent.tags.Contains("building_choice");
    }
}
