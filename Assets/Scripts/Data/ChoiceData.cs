using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoiceData
{
    [TextArea]
    public string choiceText;

    public StatEffects effects = new();
    public PersonalityEffects personalityEffects = new();
    public FactionEffects factionEffects = new();

    public List<string> requiredFlags = new();
    public List<string> blockedFlags = new();
    public List<string> setFlags = new();
    public List<string> removeFlags = new();

    public List<string> nextEventIds = new();
    public List<string> unlockBuildings = new();
    public List<string> leadershipTags = new();

    [TextArea]
    public string consequenceText;
}