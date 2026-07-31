using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private List<EventData> events;
    private EventData currentEvent;

    private HashSet<string> playedEventIds = new HashSet<string>();
    private Queue<string> pendingEventIds = new Queue<string>();
    private int resolvedEventCount;

    public int ResolvedEventCount => resolvedEventCount;

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

            if (ev.priority > 0)
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

        if (eventData.requiredAnyFlags != null &&
            eventData.requiredAnyFlags.Count > 0 &&
            !GameFlags.HasAnyFlag(eventData.requiredAnyFlags))
        {
            return false;
        }

        if (GameFlags.HasAnyFlag(eventData.blockedFlags))
            return false;

        return true;
    }

    public void AdvanceAfterResolvedEvent(ChoiceData choice)
    {
        resolvedEventCount++;

        if (resolvedEventCount % 5 == 4 && TryPickSpecialEvent())
        {
            QueueNextEvents(choice);
            return;
        }

        if (TryPickNextEventFromChoice(choice))
            return;

        if (TryPickPendingEvent())
            return;

        PickRandomEvent();
    }

    private void QueueNextEvents(ChoiceData choice)
    {
        if (choice == null || choice.nextEventIds == null)
            return;

        foreach (string nextEventId in choice.nextEventIds)
        {
            if (!string.IsNullOrWhiteSpace(nextEventId))
                pendingEventIds.Enqueue(nextEventId);
        }
    }

    private bool TryPickNextEventFromChoice(ChoiceData choice)
    {
        if (events == null || events.Count == 0)
        {
            Debug.LogError("No events loaded.");
            currentEvent = null;
            return false;
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

                return true;
            }
        }

        return false;
    }

    private bool TryPickPendingEvent()
    {
        while (pendingEventIds.Count > 0)
        {
            string nextEventId = pendingEventIds.Dequeue();
            EventData nextEvent = events.Find(e => e.id == nextEventId);

            if (nextEvent == null || !IsEventValid(nextEvent))
                continue;

            if (nextEvent.unique &&
                !string.IsNullOrEmpty(nextEvent.id) &&
                playedEventIds.Contains(nextEvent.id))
            {
                continue;
            }

            currentEvent = nextEvent;

            if (currentEvent.unique && !string.IsNullOrEmpty(currentEvent.id))
                playedEventIds.Add(currentEvent.id);

            return true;
        }

        return false;
    }

    private bool TryPickSpecialEvent()
    {
        List<EventData> specialEvents = new List<EventData>();
        int highestPriority = int.MinValue;

        foreach (EventData ev in events)
        {
            if (ev == null || ev.priority <= 0 || !IsEventValid(ev))
                continue;

            if (ev.unique && !string.IsNullOrEmpty(ev.id) && playedEventIds.Contains(ev.id))
                continue;

            if (ev.priority > highestPriority)
            {
                highestPriority = ev.priority;
                specialEvents.Clear();
            }

            if (ev.priority == highestPriority)
                specialEvents.Add(ev);
        }

        if (specialEvents.Count == 0)
            return false;

        currentEvent = PickWeightedRandomEvent(specialEvents);

        if (currentEvent != null && currentEvent.unique && !string.IsNullOrEmpty(currentEvent.id))
            playedEventIds.Add(currentEvent.id);

        return currentEvent != null;
    }
}
