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

        if (events.Count == 1)
        {
            currentEventIndex = 0;
            currentEvent = events[0];
            return;
        }

        int newIndex;

        do
        {
            newIndex = Random.Range(0, events.Count);
        }
        while (newIndex == currentEventIndex);

        currentEventIndex = newIndex;
        currentEvent = events[currentEventIndex];

        Debug.Log("Selected event: " + currentEvent.eventTitle);
    }
}