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
    [SerializeField] private TMP_Text feedbackText;

    private EventData currentEvent;

    private void Start()
    {
        LoadCurrentEvent();
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
        
        string playerResponse = freeTextInput.text;

        if (freeTextInput == null)
        {
            Debug.LogError("freeTextInput nu este legat in Inspector.");
            return;
        }

        if (string.IsNullOrWhiteSpace(playerResponse))
        {
            if (feedbackText != null)
                feedbackText.text = "Scrie un raspuns mai intai.";
            return;
        }

        if (feedbackText != null)
            feedbackText.text = "Se analizeaza raspunsul...";

        StartCoroutine(ollamaEvaluator.EvaluateResponse(
            currentEvent.eventTitle,
            currentEvent.description,
            playerResponse,
            onSuccess: result =>
            {
                GameState.Instance.AddGold(result.goldEffect);
                GameState.Instance.AddRespect(result.respectEffect);
                GameState.Instance.AddIntelligence(result.intelligenceEffect);

                if (feedbackText != null)
                    feedbackText.text = result.reason;

                freeTextInput.text = "";

                EventManager.Instance.PickRandomEvent();
                LoadCurrentEvent();
            },
            onError: error =>
            {
                if (feedbackText != null)
                    feedbackText.text = error;
            }
        ));
    }
}