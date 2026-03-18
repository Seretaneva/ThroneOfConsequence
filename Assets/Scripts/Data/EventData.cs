using System;
using System.Collections.Generic;

[Serializable]
public class EventData
{
    // ID unic pentru referinte in nextEventIds
    public string id;

    public string eventTitle;
    public string description;

    public ChoiceData choiceA;
    public ChoiceData choiceB;
    public ChoiceData choiceC;

    // Conditii pe stats vizibile
    public int minGold;
    public int maxGold = 9999;

    public int minRespect;
    public int maxRespect = 9999;

    public int minIntelligence;
    public int maxIntelligence = 9999;

    // Taguri pentru filtrare / categorii
    public List<string> tags = new();

    // Compatibilitate cu vechiul sistem
    public string requiredFlag;

    // Sistem nou
    public List<string> requiredFlags = new();
    public List<string> blockedFlags = new();

    // Storyline / chain
    public string storyline;
    public int storyStage;

    // Control selectie
    public int minRank = 0;
    public int maxRank = 9999;
    public int weight = 1;
    public int priority = 0;

    // Optional: sa nu reapara de prea multe ori
    public bool unique;
}