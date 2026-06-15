using TMPro;
using UnityEngine;
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
    private TMP_FontAsset medievalFont;
    private MedievalFeedbackPanel medievalFeedbackPanel;
  

    private void Start()
    {
        medievalFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Oswald Bold SDF");
        CacheDefaultFreeTextPlaceholder();
        ApplyMedievalButtonStyle();
        ApplyMedievalTextStyle();
        ApplyMedievalFeedbackStyle();
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
        AddMedievalStyleToButtons(choicesPanel);
        AddMedievalStyleToButtons(feedbackPanel);
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
        }
    }

    private void ApplyMedievalFeedbackStyle()
    {
        if (feedbackPanel == null)
            return;

        medievalFeedbackPanel = feedbackPanel.GetComponent<MedievalFeedbackPanel>();

        if (medievalFeedbackPanel == null)
            medievalFeedbackPanel = feedbackPanel.AddComponent<MedievalFeedbackPanel>();

        medievalFeedbackPanel.Initialize(feedbackReasonText, feedbackStatsText);
    }

    private void ApplyMedievalTextStyle()
    {
        StyleMedievalText(eventTitleText, 46f, new Color(1f, 0.80f, 0.28f, 1f), 0.24f);
        StyleMedievalText(eventDescriptionText, 32f, new Color(1f, 0.96f, 0.82f, 1f), 0.18f);
        StyleMedievalText(choiceAText, 30f, new Color(1f, 0.94f, 0.74f, 1f), 0.20f);
        StyleMedievalText(choiceBText, 30f, new Color(1f, 0.94f, 0.74f, 1f), 0.20f);
        StyleMedievalText(choiceCText, 30f, new Color(1f, 0.94f, 0.74f, 1f), 0.20f);

        if (freeTextInput != null)
        {
            StyleMedievalText(freeTextInput.textComponent, 28f, new Color(1f, 0.97f, 0.86f, 1f), 0.18f);

            if (freeTextInput.placeholder is TMP_Text placeholderText)
                StyleMedievalText(placeholderText, 24f, new Color(1f, 0.86f, 0.54f, 0.82f), 0.14f);

            Image inputImage = freeTextInput.GetComponent<Image>();
            if (inputImage != null)
                inputImage.color = new Color(0.11f, 0.06f, 0.03f, 0.58f);
        }
    }

    private void StyleMedievalText(TMP_Text text, float maxSize, Color color, float outlineWidth)
    {
        if (text == null)
            return;

        if (medievalFont != null)
            text.font = medievalFont;

        text.color = color;
        text.fontStyle |= FontStyles.Bold;
        text.characterSpacing = 0.6f;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Max(16f, maxSize * 0.55f);
        text.fontSizeMax = maxSize;
        text.outlineColor = new Color32(18, 8, 2, 255);
        text.outlineWidth = outlineWidth;

        Outline outline = text.GetComponent<Outline>();
        if (outline == null)
            outline = text.gameObject.AddComponent<Outline>();

        outline.effectColor = new Color(0.02f, 0.01f, 0.004f, 0.96f);
        outline.effectDistance = new Vector2(2.2f, -2.2f);

        Shadow shadow = GetExactShadow(text.gameObject);
        if (shadow == null)
            shadow = text.gameObject.AddComponent<Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, 0.78f);
        shadow.effectDistance = new Vector2(3.2f, -3.2f);
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

        lastResolvedChoice = choice;

        ChoiceProcessor.ApplyChoice(choice);

        pendingGoldEffect = choice.effects.gold;
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

        ShowFeedbackPanel();
    }

    public void ContinueToNextEvent()
    {
        if (lastResolvedChoice != null)
            EventManager.Instance.PickNextEventFromChoice(lastResolvedChoice);
        else
            EventManager.Instance.PickRandomEvent();

        HideEventUI();

        if (audienceSequenceController != null)
            audienceSequenceController.FinishCurrentAudienceAndWaitForKnock();
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
            choiceAText.text = currentEvent.choiceA != null ? currentEvent.choiceA.choiceText : "";

        if (choiceBText != null)
            choiceBText.text = currentEvent.choiceB != null ? currentEvent.choiceB.choiceText : "";

        if (choiceCText != null)
            choiceCText.text = currentEvent.choiceC != null ? currentEvent.choiceC.choiceText : "";

        if (freeTextInput != null)
            freeTextInput.text = "";

        lastResolvedChoice = null;
        SetFreeTextPlaceholder(defaultFreeTextPlaceholder);
        ClearFeedback();
        ShowChoicesPanel();
    }
}
