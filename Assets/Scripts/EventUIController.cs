using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUIController : MonoBehaviour
{
    [Header("Event Data")]
    [SerializeField] private EventDefinition currentEvent;

    [Header("UI References")]
    [SerializeField] private TMP_Text eventTitleText;
    [SerializeField] private TMP_Text eventDescriptionText;

    [SerializeField] private Button choiceAButton;
    [SerializeField] private Button choiceBButton;
    [SerializeField] private Button choiceCButton;

    [SerializeField] private TMP_Text choiceAText;
    [SerializeField] private TMP_Text choiceBText;
    [SerializeField] private TMP_Text choiceCText;

    private void Start()
    {
        DisplayEvent();
    }

    private void DisplayEvent()
    {
        if (currentEvent == null)
        {
            Debug.LogError("Current Event is not assigned in EventUIController.");
            return;
        }

        eventTitleText.text = currentEvent.eventTitle;
        eventDescriptionText.text = currentEvent.description;

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

        Debug.Log("Choice selected: " + choice.choiceText);
    }
}