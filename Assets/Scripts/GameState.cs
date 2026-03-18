using System;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    // Events pentru UI
    public event Action OnStatsChanged;
    public event Action OnDayChanged;
    public event Action OnRankChanged;

    [Header("Visible Stats")]
    [SerializeField] private int gold = 50;
    [SerializeField] private int intelligence = 10;
    [SerializeField] private int respect = 10;

    [Header("Progression")]
    [SerializeField] private int day = 1;
    [SerializeField] private string currentRank = "Village Leader";

    [Header("Personality")]
    [SerializeField] private int cruelty = 0;
    [SerializeField] private int justice = 0;
    [SerializeField] private int mercy = 0;
    [SerializeField] private int greed = 0;
    [SerializeField] private int authority = 0;
    [SerializeField] private int wisdom = 0;

    [Header("Factions")]
    [SerializeField] private int peasants = 0;
    [SerializeField] private int nobles = 0;
    [SerializeField] private int merchants = 0;
    [SerializeField] private int army = 0;
    [SerializeField] private int clergy = 0;
    [SerializeField] private int scholars = 0;

    // Optional: buildings unlocked
    private HashSet<string> unlockedBuildings = new HashSet<string>();

    // Public read-only access
    public int Gold => gold;
    public int Intelligence => intelligence;
    public int Respect => respect;

    public int Day => day;
    public string CurrentRank => currentRank;

    public int Cruelty => cruelty;
    public int Justice => justice;
    public int Mercy => mercy;
    public int Greed => greed;
    public int Authority => authority;
    public int Wisdom => wisdom;

    public int Peasants => peasants;
    public int Nobles => nobles;
    public int Merchants => merchants;
    public int Army => army;
    public int Clergy => clergy;
    public int Scholars => scholars;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #region Visible Stat Modifiers

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

    #region Personality Modifiers

    public void AddCruelty(int amount)
    {
        cruelty += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddJustice(int amount)
    {
        justice += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddMercy(int amount)
    {
        mercy += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddGreed(int amount)
    {
        greed += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddAuthority(int amount)
    {
        authority += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddWisdom(int amount)
    {
        wisdom += amount;
        OnStatsChanged?.Invoke();
    }

    #endregion

    #region Faction Modifiers

    public void AddPeasants(int amount)
    {
        peasants += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddNobles(int amount)
    {
        nobles += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddMerchants(int amount)
    {
        merchants += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddArmy(int amount)
    {
        army += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddClergy(int amount)
    {
        clergy += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddScholars(int amount)
    {
        scholars += amount;
        OnStatsChanged?.Invoke();
    }

    #endregion

    #region Buildings

    public void UnlockBuilding(string buildingId)
    {
        if (string.IsNullOrWhiteSpace(buildingId))
            return;

        unlockedBuildings.Add(buildingId);
        OnStatsChanged?.Invoke();
    }

    public bool HasBuilding(string buildingId)
    {
        if (string.IsNullOrWhiteSpace(buildingId))
            return false;

        return unlockedBuildings.Contains(buildingId);
    }

    public IEnumerable<string> GetUnlockedBuildings()
    {
        return unlockedBuildings;
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
        if (string.IsNullOrWhiteSpace(newRank))
            return;

        currentRank = newRank;
        OnRankChanged?.Invoke();
    }

    #endregion
}