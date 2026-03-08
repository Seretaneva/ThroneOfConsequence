using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private List<EventData> events;
    private EventData currentEvent;
    private int currentEventIndex = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (EventDatabaseLoader.LoadedDatabase == null || EventDatabaseLoader.LoadedDatabase.events == null)
        {
            Debug.LogError("Event database was not loaded.");
            return;
        }

        events = EventDatabaseLoader.LoadedDatabase.events;
        PickRandomEvent();
    }

    public EventData GetCurrentEvent()
    {
        return currentEvent;
    }

    public void PickRandomEvent()
    {
        if (events == null || events.Count == 0)
        {
            Debug.LogError("No events loaded.");
            return;
        }

        List<int> validIndexes = new List<int>();

        for (int i = 0; i < events.Count; i++)
        {
            if (IsEventValid(events[i]) && i != currentEventIndex)
            {
                validIndexes.Add(i);
            }
        }

        if (validIndexes.Count == 0)
        {
            Debug.LogWarning("No valid events found. Keeping current event.");
            return;
        }

        int randomListIndex = Random.Range(0, validIndexes.Count);
        currentEventIndex = validIndexes[randomListIndex];
        currentEvent = events[currentEventIndex];

        Debug.Log("Selected valid event: " + currentEvent.eventTitle);
    }

    private bool IsEventValid(EventData eventData)
    {
        int gold = GameState.Instance.Gold;
        int respect = GameState.Instance.Respect;
        int intelligence = GameState.Instance.Intelligence;

        bool validGold = gold >= eventData.minGold && gold <= eventData.maxGold;
        bool validRespect = respect >= eventData.minRespect && respect <= eventData.maxRespect;
        bool validIntelligence = intelligence >= eventData.minIntelligence && intelligence <= eventData.maxIntelligence;

        return validGold && validRespect && validIntelligence;
    }
}