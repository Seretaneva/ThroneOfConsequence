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

    private EventDefinition currentEvent;

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

    GameState.Instance.NextDay();

    EventManager.Instance.PickRandomEvent();
    LoadCurrentEvent();
}
}