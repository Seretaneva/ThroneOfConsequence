using UnityEngine;

[CreateAssetMenu(fileName = "NewEvent", menuName = "Game/Event Definition")]
public class EventDefinition : ScriptableObject
{
    [Header("Event Info")]
    public string eventTitle;

    [TextArea(3, 6)]
    public string description;

    [Header("Choices")]
    public ChoiceData choiceA;
    public ChoiceData choiceB;
    public ChoiceData choiceC;
}