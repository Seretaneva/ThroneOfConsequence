using System.Collections.Generic;
using UnityEngine;

public class RoyalChronicle : MonoBehaviour
{
    public static RoyalChronicle Instance;

    private List<string> entries = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddEntry(string text)
    {
        int day = GameState.Instance.Day;
        entries.Add($"Day {day} - {text}");
    }

    public string GetChronicleText()
    {
        return string.Join("\n", entries);
    }
}