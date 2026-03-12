using System;
using System.Collections.Generic;

[Serializable]
public class EventData
{
    public string eventTitle;
    public string description;

    public ChoiceData choiceA;
    public ChoiceData choiceB;
    public ChoiceData choiceC;

    public int minGold;
    public int maxGold = 9999;

    public int minRespect;
    public int maxRespect = 9999;

    public int minIntelligence;
    public int maxIntelligence = 9999;

    public List<string> tags;
    
    public string requiredFlag;
}