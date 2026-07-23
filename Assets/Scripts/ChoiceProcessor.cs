using UnityEngine;

public static class ChoiceProcessor
{
    public static bool CanApplyChoice(ChoiceData choice, out string failureReason)
    {
        failureReason = "";

        if (choice == null)
        {
            failureReason = "Alegerea nu este disponibila.";
            return false;
        }

        if (GameState.Instance == null)
        {
            failureReason = "Starea jocului nu este disponibila.";
            return false;
        }

        int minimumGold = Mathf.Max(choice.goldCost, choice.requiredGold);

        if (GameState.Instance.Gold < minimumGold)
        {
            int missingGold = minimumGold - GameState.Instance.Gold;
            failureReason = "Iti lipsesc " + missingGold + " aur.";
            return false;
        }

        if (GameState.Instance.Respect < choice.requiredRespect)
        {
            failureReason = "Necesita " + choice.requiredRespect + " Respect.";
            return false;
        }

        if (GameState.Instance.Intelligence < choice.requiredIntelligence)
        {
            failureReason = "Necesita " + choice.requiredIntelligence + " Intelect.";
            return false;
        }

        return true;
    }

    public static bool ApplyChoice(ChoiceData choice)
    {
        if (!CanApplyChoice(choice, out string failureReason))
        {
            Debug.LogWarning("ChoiceProcessor: " + failureReason);
            return false;
        }

        ApplyCost(choice);
        ApplyVisibleStats(choice);
        ApplyFlags(choice);
        ApplyPersonality(choice);
        ApplyFactions(choice);
        ApplyBuildings(choice);
        return true;
    }

    private static void ApplyCost(ChoiceData choice)
    {
        if (choice.goldCost > 0)
            GameState.Instance.AddGold(-choice.goldCost);
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

        if (!string.IsNullOrWhiteSpace(choice.buildingChoiceGroup))
        {
            ApplyExclusiveBuildingChoice(choice);
            return;
        }

        foreach (var buildingId in choice.unlockBuildings)
        {
            if (string.IsNullOrWhiteSpace(buildingId))
                continue;

            if (GameState.Instance.UnlockBuilding(buildingId))
                Debug.Log("Building unlocked: " + buildingId);
            else
                Debug.Log("Building was already unlocked: " + buildingId);
        }
    }

    private static void ApplyExclusiveBuildingChoice(ChoiceData choice)
    {
        if (choice.unlockBuildings.Count != 1)
        {
            Debug.LogError(
                "Exclusive building choice must contain exactly one selected building."
            );
            return;
        }

        string selectedBuildingId = choice.unlockBuildings[0];

        if (GameState.Instance.ChooseExclusiveBuilding(
            choice.buildingChoiceGroup,
            selectedBuildingId,
            choice.buildingChoiceOptions))
        {
            Debug.Log(
                "Exclusive building chosen: " + selectedBuildingId +
                " | Group: " + choice.buildingChoiceGroup
            );
        }
        else
        {
            Debug.LogWarning(
                "Exclusive building choice was rejected: " + selectedBuildingId +
                " | Group: " + choice.buildingChoiceGroup
            );
        }
    }
}
