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
    private HashSet<string> permanentlyLockedBuildings = new HashSet<string>();
    private Dictionary<string, string> buildingChoices = new Dictionary<string, string>();

    // Public read-only access
    public int Gold => gold;
    public int Intelligence => intelligence;
    public int Respect => respect;

    public int Day => day;
    public string CurrentRank => currentRank;
    public bool IsGameOver => gold <= 0 || respect <= 0 || intelligence <= 0;

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
        {
            Instance = this;
            GameSaveData save = SaveManager.GetPendingLoad();

            if (save != null)
                RestoreSaveData(save);
        }
        else
            Destroy(gameObject);
    }

    public void WriteSaveData(GameSaveData data)
    {
        data.gold = gold;
        data.intelligence = intelligence;
        data.respect = respect;
        data.day = day;
        data.currentRank = currentRank;
        data.cruelty = cruelty;
        data.justice = justice;
        data.mercy = mercy;
        data.greed = greed;
        data.authority = authority;
        data.wisdom = wisdom;
        data.peasants = peasants;
        data.nobles = nobles;
        data.merchants = merchants;
        data.army = army;
        data.clergy = clergy;
        data.scholars = scholars;
        data.unlockedBuildings = new List<string>(unlockedBuildings);
        data.permanentlyLockedBuildings = new List<string>(permanentlyLockedBuildings);

        foreach (KeyValuePair<string, string> choice in buildingChoices)
        {
            data.buildingChoiceGroups.Add(choice.Key);
            data.buildingChoiceIds.Add(choice.Value);
        }
    }

    private void RestoreSaveData(GameSaveData data)
    {
        gold = data.gold;
        intelligence = data.intelligence;
        respect = data.respect;
        day = data.day;
        currentRank = string.IsNullOrWhiteSpace(data.currentRank) ? "Village Leader" : data.currentRank;
        cruelty = data.cruelty;
        justice = data.justice;
        mercy = data.mercy;
        greed = data.greed;
        authority = data.authority;
        wisdom = data.wisdom;
        peasants = data.peasants;
        nobles = data.nobles;
        merchants = data.merchants;
        army = data.army;
        clergy = data.clergy;
        scholars = data.scholars;
        unlockedBuildings = new HashSet<string>(data.unlockedBuildings ?? new List<string>());
        permanentlyLockedBuildings = new HashSet<string>(data.permanentlyLockedBuildings ?? new List<string>());
        buildingChoices.Clear();

        int choiceCount = Mathf.Min(
            data.buildingChoiceGroups?.Count ?? 0,
            data.buildingChoiceIds?.Count ?? 0);

        for (int i = 0; i < choiceCount; i++)
            buildingChoices[data.buildingChoiceGroups[i]] = data.buildingChoiceIds[i];

        GameFlags.RestoreFlags(data.flags);
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

    public bool UnlockBuilding(string buildingId)
    {
        if (string.IsNullOrWhiteSpace(buildingId))
            return false;

        if (permanentlyLockedBuildings.Contains(buildingId))
            return false;

        if (!unlockedBuildings.Add(buildingId))
            return false;

        OnStatsChanged?.Invoke();
        return true;
    }

    public bool ChooseExclusiveBuilding(
        string choiceGroup,
        string selectedBuildingId,
        IEnumerable<string> choiceOptions)
    {
        if (string.IsNullOrWhiteSpace(choiceGroup) ||
            string.IsNullOrWhiteSpace(selectedBuildingId) ||
            choiceOptions == null)
        {
            return false;
        }

        if (buildingChoices.ContainsKey(choiceGroup) ||
            permanentlyLockedBuildings.Contains(selectedBuildingId))
        {
            return false;
        }

        bool selectedBuildingExists = false;

        foreach (string buildingId in choiceOptions)
        {
            if (buildingId == selectedBuildingId)
            {
                selectedBuildingExists = true;
                break;
            }
        }

        if (!selectedBuildingExists)
            return false;

        buildingChoices.Add(choiceGroup, selectedBuildingId);
        unlockedBuildings.Add(selectedBuildingId);

        foreach (string buildingId in choiceOptions)
        {
            if (!string.IsNullOrWhiteSpace(buildingId) && buildingId != selectedBuildingId)
                permanentlyLockedBuildings.Add(buildingId);
        }

        OnStatsChanged?.Invoke();
        return true;
    }

    public bool HasBuilding(string buildingId)
    {
        if (string.IsNullOrWhiteSpace(buildingId))
            return false;

        return unlockedBuildings.Contains(buildingId);
    }

    public bool IsBuildingPermanentlyLocked(string buildingId)
    {
        if (string.IsNullOrWhiteSpace(buildingId))
            return false;

        return permanentlyLockedBuildings.Contains(buildingId);
    }

    public bool HasMadeBuildingChoice(string choiceGroup)
    {
        if (string.IsNullOrWhiteSpace(choiceGroup))
            return false;

        return buildingChoices.ContainsKey(choiceGroup);
    }

    public string GetChosenBuilding(string choiceGroup)
    {
        if (string.IsNullOrWhiteSpace(choiceGroup))
            return null;

        return buildingChoices.TryGetValue(choiceGroup, out string buildingId)
            ? buildingId
            : null;
    }

    public IEnumerable<string> GetUnlockedBuildings()
    {
        return unlockedBuildings;
    }

    public IEnumerable<string> GetPermanentlyLockedBuildings()
    {
        return permanentlyLockedBuildings;
    }

    #endregion

    public string GetGameOverTitle()
    {
        if (gold <= 0)
            return "Faliment";

        if (respect <= 0)
            return "Revolta";

        if (intelligence <= 0)
            return "Detronat de propria curte";

        return "";
    }

    public string GetGameOverDescription()
    {
        if (gold <= 0)
            return "Vistieria este goala. Soldatii isi parasesc posturile, iar creditorii revendica domeniul.";

        if (respect <= 0)
            return "Nimeni nu-ti mai recunoaste autoritatea. Multimea patrunde in sala tronului si domnia ta se incheie.";

        if (intelligence <= 0)
            return "Consilierii semneaza ordine in numele tau, iar tronul iti mai apartine doar in picturi.";

        return "";
    }

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
