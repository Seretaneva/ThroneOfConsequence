using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int version = 1;
    public int gold;
    public int intelligence;
    public int respect;
    public int day;
    public string currentRank;
    public int cruelty;
    public int justice;
    public int mercy;
    public int greed;
    public int authority;
    public int wisdom;
    public int peasants;
    public int nobles;
    public int merchants;
    public int army;
    public int clergy;
    public int scholars;
    public List<string> flags = new();
    public List<string> unlockedBuildings = new();
    public List<string> permanentlyLockedBuildings = new();
    public List<string> buildingChoiceGroups = new();
    public List<string> buildingChoiceIds = new();
    public int resolvedEventCount;
    public string currentEventId;
    public List<string> playedEventIds = new();
    public List<string> pendingEventIds = new();
}

public static class SaveManager
{
    private const string SaveKey = "throne_of_consequence_save_v1";
    private static GameSaveData pendingLoad;

    public static bool HasSave => PlayerPrefs.HasKey(SaveKey);
    public static bool HasPendingLoad => pendingLoad != null;

    public static void SaveGame()
    {
        if (GameState.Instance == null || EventManager.Instance == null)
            return;

        GameSaveData data = new GameSaveData();
        GameState.Instance.WriteSaveData(data);
        EventManager.Instance.WriteSaveData(data);
        data.flags = GameFlags.GetAllFlags();

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
        Debug.Log("Game saved. Next event: " + data.currentEventId);
    }

    public static bool PrepareContinue()
    {
        if (!HasSave)
            return false;

        try
        {
            pendingLoad = JsonUtility.FromJson<GameSaveData>(PlayerPrefs.GetString(SaveKey));
            return pendingLoad != null;
        }
        catch (Exception exception)
        {
            Debug.LogError("Save could not be loaded: " + exception.Message);
            pendingLoad = null;
            return false;
        }
    }

    public static GameSaveData GetPendingLoad() => pendingLoad;

    public static void FinishPendingLoad()
    {
        pendingLoad = null;
    }

    public static void DeleteSave()
    {
        pendingLoad = null;
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}
