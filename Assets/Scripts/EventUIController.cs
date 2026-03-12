using TMPro;
using UnityEngine;

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

    private RuleBasedEvaluator ruleBasedEvaluator = new RuleBasedEvaluator();
    private EventData currentEvent;

    private int pendingGoldEffect;
    private int pendingRespectEffect;
    private int pendingIntelligenceEffect;

    private void Start()
    {
        LoadCurrentEvent();
        ShowChoicesPanel();
    }

    private void LoadCurrentEvent()
    {
        currentEvent = EventManager.Instance.GetCurrentEvent();

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
            choiceAText.text = currentEvent.choiceA.choiceText;

        if (choiceBText != null)
            choiceBText.text = currentEvent.choiceB.choiceText;

        if (choiceCText != null)
            choiceCText.text = currentEvent.choiceC.choiceText;

        if (freeTextInput != null)
            freeTextInput.text = "";

        ClearFeedback();
    }

    private void ClearFeedback()
    {
        if (feedbackReasonText != null)
            feedbackReasonText.text = "";

        if (feedbackStatsText != null)
            feedbackStatsText.text = "";
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
            feedbackPanel.SetActive(true);
    }

    public void ChooseA()
    {
        ResolveChoice(currentEvent.choiceA, "Ai ales prima optiune.");
    }

    public void ChooseB()
    {
        ResolveChoice(currentEvent.choiceB, "Ai ales a doua optiune.");
    }

    public void ChooseC()
    {
        ResolveChoice(currentEvent.choiceC, "Ai ales a treia optiune.");
    }

    private void ResolveChoice(ChoiceData choice, string reason)
    {
        GameState.Instance.AddGold(choice.goldEffect);
        GameState.Instance.AddRespect(choice.respectEffect);
        GameState.Instance.AddIntelligence(choice.intelligenceEffect);

        if (!string.IsNullOrEmpty(choice.setFlag))
        {
            GameFlags.SetFlag(choice.setFlag);
        }

        pendingGoldEffect = choice.goldEffect;
        pendingRespectEffect = choice.respectEffect;
        pendingIntelligenceEffect = choice.intelligenceEffect;

        if (feedbackReasonText != null)
            feedbackReasonText.text = reason;

        if (feedbackStatsText != null)
            feedbackStatsText.text = FormatStatEffects(
                pendingGoldEffect,
                pendingRespectEffect,
                pendingIntelligenceEffect
            );

        ShowFeedbackPanel();
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
            if (feedbackReasonText != null)
                feedbackReasonText.text = "Scrie un raspuns mai intai.";

            if (feedbackStatsText != null)
                feedbackStatsText.text = "";

            return;
        }

        if (feedbackReasonText != null)
            feedbackReasonText.text = "Se analizeaza raspunsul...";

        if (feedbackStatsText != null)
            feedbackStatsText.text = "";

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
                Debug.LogWarning("Ollama failed, using fallback evaluator. Error: " + error);
                StatEvaluationResult fallbackResult = ruleBasedEvaluator.Evaluate(playerResponse);
                ApplyEvaluationResult(fallbackResult);
            }
        ));
    }

    private void ApplyEvaluationResult(StatEvaluationResult result)
    {
        if (result == null)
        {
            Debug.LogError("Evaluation result is null.");
            return;
        }

        GameState.Instance.AddGold(result.goldEffect);
        GameState.Instance.AddRespect(result.respectEffect);
        GameState.Instance.AddIntelligence(result.intelligenceEffect);

        pendingGoldEffect = result.goldEffect;
        pendingRespectEffect = result.respectEffect;
        pendingIntelligenceEffect = result.intelligenceEffect;

        if (feedbackReasonText != null)
            feedbackReasonText.text = result.reason;

        if (feedbackStatsText != null)
            feedbackStatsText.text = FormatStatEffects(
                pendingGoldEffect,
                pendingRespectEffect,
                pendingIntelligenceEffect
            );

        if (freeTextInput != null)
            freeTextInput.text = "";

        ShowFeedbackPanel();
    }

    public void ContinueToNextEvent()
    {
        EventManager.Instance.PickRandomEvent();
        LoadCurrentEvent();
        ShowChoicesPanel();
    }

    private string FormatStatEffects(int gold, int respect, int intelligence)
    {
        string goldText = gold >= 0 ? $"+{gold}" : gold.ToString();
        string respectText = respect >= 0 ? $"+{respect}" : respect.ToString();
        string intelligenceText = intelligence >= 0 ? $"+{intelligence}" : intelligence.ToString();

        return $"Gold {goldText} | Respect {respectText} | Intelligence {intelligenceText}";
    }
}