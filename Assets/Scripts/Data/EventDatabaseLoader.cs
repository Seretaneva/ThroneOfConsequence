using UnityEngine;

public class EventDatabaseLoader : MonoBehaviour
{
    public static EventDatabase LoadedDatabase { get; private set; }

    private void Awake()
    {
        LoadEvents();
    }

    private void LoadEvents()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("events");

        if (jsonFile == null)
        {
            Debug.LogError("events.json was not found in Assets/Resources.");
            return;
        }

        LoadedDatabase = JsonUtility.FromJson<EventDatabase>(jsonFile.text);

        if (LoadedDatabase == null || LoadedDatabase.events == null || LoadedDatabase.events.Count == 0)
        {
            Debug.LogError("Failed to load events from JSON.");
            return;
        }

        Debug.Log("Loaded " + LoadedDatabase.events.Count + " events from JSON.");
    }
}