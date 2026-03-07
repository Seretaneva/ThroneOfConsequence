using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [SerializeField] private EventDefinition[] events;

    private EventDefinition currentEvent;
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
        PickRandomEvent();
    }

    public EventDefinition GetCurrentEvent()
    {
        if (currentEvent == null)
        {
            Debug.LogError("No current event selected.");
        }

        return currentEvent;
    }

    public void PickRandomEvent()
    {
        if (events == null || events.Length == 0)
        {
            Debug.LogError("EventManager has no events assigned.");
            return;
        }

        if (events.Length == 1)
        {
            currentEventIndex = 0;
            currentEvent = events[0];
            return;
        }

        int newIndex;

        do
        {
            newIndex = Random.Range(0, events.Length);
        }
        while (newIndex == currentEventIndex);

        currentEventIndex = newIndex;
        currentEvent = events[currentEventIndex];

        Debug.Log("Selected event: " + currentEvent.eventTitle);
    }
}