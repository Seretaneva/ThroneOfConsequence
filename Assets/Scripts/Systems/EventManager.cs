using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [SerializeField] private EventDefinition[] events;

    private int currentEventIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public EventDefinition GetCurrentEvent()
    {
        if (events == null || events.Length == 0)
        {
            Debug.LogError("EventManager has no events assigned.");
            return null;
        }

        return events[currentEventIndex];
    }

    public void GoToNextEvent()
    {
        if (events == null || events.Length == 0)
            return;

        currentEventIndex++;

        if (currentEventIndex >= events.Length)
        {
            currentEventIndex = 0;
        }
    }
}