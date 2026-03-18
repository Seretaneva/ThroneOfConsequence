using UnityEngine;

public static class ChoiceProcessor
{
    public static void ApplyChoice(ChoiceData choice)
    {
        if (choice == null)
        {
            Debug.LogError("ChoiceProcessor: choice is null.");
            return;
        }

        if (GameState.Instance == null)
        {
            Debug.LogError("ChoiceProcessor: GameState.Instance is null.");
            return;
        }

        ApplyVisibleStats(choice);
        ApplyFlags(choice);
        ApplyPersonality(choice);
        ApplyFactions(choice);
        ApplyBuildings(choice);
    }

    private static void ApplyVisibleStats(ChoiceData choice)
    {
        GameState.Instance.AddGold(choice.effects.gold);
        GameState.Instance.AddRespect(choice.effects.respect);
        GameState.Instance.AddIntelligence(choice.effects.intelligence);
    }

    private static void ApplyFlags(ChoiceData choice)
    {
        if (choice.setFlags != null)
        {
            foreach (var flag in choice.setFlags)
            {
                if (!string.IsNullOrWhiteSpace(flag))
                    GameFlags.SetFlag(flag);
            }
        }

        if (choice.removeFlags != null)
        {
            foreach (var flag in choice.removeFlags)
            {
                if (!string.IsNullOrWhiteSpace(flag))
                    GameFlags.RemoveFlag(flag);
            }
        }
    }

    private static void ApplyPersonality(ChoiceData choice)
    {
        if (choice.personalityEffects == null)
            return;

        GameState.Instance.AddCruelty(choice.personalityEffects.cruelty);
        GameState.Instance.AddJustice(choice.personalityEffects.justice);
        GameState.Instance.AddMercy(choice.personalityEffects.mercy);
        GameState.Instance.AddGreed(choice.personalityEffects.greed);
        GameState.Instance.AddAuthority(choice.personalityEffects.authority);
        GameState.Instance.AddWisdom(choice.personalityEffects.wisdom);
    }

    private static void ApplyFactions(ChoiceData choice)
    {
        if (choice.factionEffects == null)
            return;

        GameState.Instance.AddPeasants(choice.factionEffects.peasants);
        GameState.Instance.AddNobles(choice.factionEffects.nobles);
        GameState.Instance.AddMerchants(choice.factionEffects.merchants);
        GameState.Instance.AddArmy(choice.factionEffects.army);
        GameState.Instance.AddClergy(choice.factionEffects.clergy);
        GameState.Instance.AddScholars(choice.factionEffects.scholars);
    }

    private static void ApplyBuildings(ChoiceData choice)
    {
        if (choice.unlockBuildings == null)
            return;

        foreach (var buildingId in choice.unlockBuildings)
        {
            if (!string.IsNullOrWhiteSpace(buildingId))
            {
                // aici vei chema mai tarziu BuildingManager / UnlockSystem
                Debug.Log("Unlocked building: " + buildingId);
            }
        }
    }
}