using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    // Events pentru UI
    public event Action OnStatsChanged;
    public event Action OnDayChanged;
    public event Action OnRankChanged;

    [Header("Stats")]
    [SerializeField] private int gold = 50;
    [SerializeField] private int intelligence = 10;
    [SerializeField] private int respect = 10;

    [Header("Progression")]
    [SerializeField] private int day = 1;
    [SerializeField] private string currentRank = "Village Leader";

    // Public read-only access
    public int Gold => gold;
    public int Intelligence => intelligence;
    public int Respect => respect;
    public int Day => day;
    public string CurrentRank => currentRank;

    private void Awake()
    {
        // Singleton simplu
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #region Stat Modifiers

    public void AddGold(int amount)
    {
        gold += amount;
        gold = Mathf.Max(0, gold);
        OnStatsChanged?.Invoke();
    }

    public void AddIntelligence(int amount)
    {
        intelligence += amount;
        intelligence = Mathf.Max(0, intelligence);
        OnStatsChanged?.Invoke();
    }

    public void AddRespect(int amount)
    {
        respect += amount;
        respect = Mathf.Max(0, respect);
        OnStatsChanged?.Invoke();
    }

    #endregion

    #region Time

    public void NextDay()
    {
        day++;
        OnDayChanged?.Invoke();
    }

    #endregion

    #region Rank

    public void SetRank(string newRank)
    {
        currentRank = newRank;
        OnRankChanged?.Invoke();
    }

    #endregion
}