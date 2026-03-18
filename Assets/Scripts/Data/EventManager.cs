using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private List<EventData> events;
    private EventData currentEvent;

    private HashSet<string> playedEventIds = new HashSet<string>();

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
            currentEvent = null;
            return;
        }

        List<EventData> validEvents = new List<EventData>();

        foreach (var ev in events)
        {
            if (!IsEventValid(ev))
                continue;

            if (ev.unique && !string.IsNullOrEmpty(ev.id) && playedEventIds.Contains(ev.id))
                continue;

            validEvents.Add(ev);
        }

        if (validEvents.Count == 0)
        {
            Debug.LogWarning("No valid events found.");
            currentEvent = null;
            return;
        }

        currentEvent = PickWeightedRandomEvent(validEvents);

        if (currentEvent != null && currentEvent.unique && !string.IsNullOrEmpty(currentEvent.id))
            playedEventIds.Add(currentEvent.id);
    }

    private EventData PickWeightedRandomEvent(List<EventData> validEvents)
    {
        int totalWeight = 0;

        foreach (var ev in validEvents)
            totalWeight += Mathf.Max(1, ev.weight);

        int roll = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var ev in validEvents)
        {
            current += Mathf.Max(1, ev.weight);

            if (roll < current)
                return ev;
        }

        return validEvents[0];
    }

    private bool IsEventValid(EventData eventData)
    {
        if (eventData == null)
            return false;

        if (GameState.Instance == null)
        {
            Debug.LogError("GameState.Instance is null.");
            return false;
        }

        int gold = GameState.Instance.Gold;
        int respect = GameState.Instance.Respect;
        int intelligence = GameState.Instance.Intelligence;

        if (gold < eventData.minGold || gold > eventData.maxGold)
            return false;

        if (respect < eventData.minRespect || respect > eventData.maxRespect)
            return false;

        if (intelligence < eventData.minIntelligence || intelligence > eventData.maxIntelligence)
            return false;

        if (!string.IsNullOrEmpty(eventData.requiredFlag) && !GameFlags.HasFlag(eventData.requiredFlag))
            return false;

        if (!GameFlags.HasAllFlags(eventData.requiredFlags))
            return false;

        if (GameFlags.HasAnyFlag(eventData.blockedFlags))
            return false;

        return true;
    }

    public void PickNextEventFromChoice(ChoiceData choice)
    {
        if (events == null || events.Count == 0)
        {
            Debug.LogError("No events loaded.");
            currentEvent = null;
            return;
        }

        if (choice != null && choice.nextEventIds != null && choice.nextEventIds.Count > 0)
        {
            foreach (var nextId in choice.nextEventIds)
            {
                EventData nextEvent = events.Find(e => e.id == nextId);

                if (nextEvent == null)
                    continue;

                if (!IsEventValid(nextEvent))
                    continue;

                if (nextEvent.unique && !string.IsNullOrEmpty(nextEvent.id) && playedEventIds.Contains(nextEvent.id))
                    continue;

                currentEvent = nextEvent;

                if (currentEvent.unique && !string.IsNullOrEmpty(currentEvent.id))
                    playedEventIds.Add(currentEvent.id);

                return;
            }
        }

        PickRandomEvent();
    }
}