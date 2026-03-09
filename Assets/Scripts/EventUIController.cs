using TMPro;
using UnityEngine;

public class EventUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text eventDescriptionText;
    [SerializeField] private TMP_Text eventTitleText;

    [SerializeField] private TMP_Text choiceAText;
    [SerializeField] private TMP_Text choiceBText;
    [SerializeField] private TMP_Text choiceCText;

    [SerializeField] private TMP_InputField freeTextInput;
    [SerializeField] private OllamaEvaluator ollamaEvaluator;

    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private GameObject feedbackPanel;

    [SerializeField] private TMP_Text feedbackReasonText;
    [SerializeField] private TMP_Text feedbackStatsText;

    private EventData currentEvent;
    private ChoiceData pendingChoiceResult;
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

        eventDescriptionText.text = currentEvent.description;
        eventTitleText.text = currentEvent.eventTitle;

        choiceAText.text = currentEvent.choiceA.choiceText;
        choiceBText.text = currentEvent.choiceB.choiceText;
        choiceCText.text = currentEvent.choiceC.choiceText;

        if (freeTextInput != null)
            freeTextInput.text = "";

        if (feedbackReasonText != null)
            feedbackReasonText.text = "";

        if (feedbackStatsText != null)
            feedbackStatsText.text = "";
    }

    public void ChooseA()
    {
        ApplyChoice(currentEvent.choiceA);
    }

    public void ChooseB()
    {
        ApplyChoice(currentEvent.choiceB);
    }

    public void ChooseC()
    {
        ApplyChoice(currentEvent.choiceC);
    }

    private void ApplyChoice(ChoiceData choice)
    {
        GameState.Instance.AddGold(choice.goldEffect);
        GameState.Instance.AddIntelligence(choice.intelligenceEffect);
        GameState.Instance.AddRespect(choice.respectEffect);

        EventManager.Instance.PickRandomEvent();
        LoadCurrentEvent();
    }


    public void SubmitFreeTextResponse()
    {
        Debug.Log("1. Submit apasat");

        if (freeTextInput == null)
        {
            Debug.LogError("freeTextInput nu este legat in Inspector.");
            return;
        }

        if (ollamaEvaluator == null)
        {
            Debug.LogError("ollamaEvaluator nu este legat in Inspector.");
            return;
        }

        if (currentEvent == null)
        {
            Debug.LogError("currentEvent este null.");
            return;
        }

        string playerResponse = freeTextInput.text;
        Debug.Log("2. Text citit: " + playerResponse);

        if (string.IsNullOrWhiteSpace(playerResponse))
        {
            Debug.LogWarning("3. Textul este gol");
            if (feedbackReasonText != null)
                feedbackReasonText.text = "Scrie un raspuns mai intai.";

            if (feedbackStatsText != null)
                feedbackStatsText.text = "";

            return;
        }

        Debug.Log("4. Inainte de EvaluateResponse");

        if (feedbackReasonText != null)
            feedbackReasonText.text = "Se analizeaza raspunsul...";

        if (feedbackStatsText != null)
            feedbackStatsText.text = "";

        StartCoroutine(ollamaEvaluator.EvaluateResponse(
            currentEvent.eventTitle,
            currentEvent.description,
            playerResponse,
            onSuccess: result =>
            {
                Debug.Log("5. onSuccess a fost apelat");

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

                Debug.Log("6. Inainte de ShowFeedbackPanel");
                ShowFeedbackPanel();
                Debug.Log("7. Dupa ShowFeedbackPanel");
            },
            onError: error =>
            {
                Debug.LogError("8. onError: " + error);

                if (feedbackReasonText != null)
                    feedbackReasonText.text = error;

                if (feedbackStatsText != null)
                    feedbackStatsText.text = "";
            }
        ));

        Debug.Log("9. Coroutine pornita");
    }
    private string FormatStatEffects(int gold, int respect, int intelligence)
    {
        string goldText = gold >= 0 ? $"+{gold}" : gold.ToString();
        string respectText = respect >= 0 ? $"+{respect}" : respect.ToString();
        string intelligenceText = intelligence >= 0 ? $"+{intelligence}" : intelligence.ToString();

        return $"Gold {goldText} | Respect {respectText} | Intelligence {intelligenceText}";
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

    private void ResolveChoice(ChoiceData choice, string reason)
    {
        GameState.Instance.AddGold(choice.goldEffect);
        GameState.Instance.AddRespect(choice.respectEffect);
        GameState.Instance.AddIntelligence(choice.intelligenceEffect);

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
    
    public void ContinueToNextEvent()
    {
        EventManager.Instance.PickRandomEvent();
        LoadCurrentEvent();
        ShowChoicesPanel();
    }
}