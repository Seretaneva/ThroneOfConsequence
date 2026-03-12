using System.Collections.Generic;
using UnityEngine;

public class RuleBasedEvaluator
{
    private readonly string[] negationWords =
    {
        "nu", "fara", "nici", "refuz", "refuz sa"
    };

    private readonly Dictionary<string, StatEvaluationResult> phraseRules = new Dictionary<string, StatEvaluationResult>()
    {
        { "ajut", new StatEvaluationResult { goldEffect = -2, respectEffect = 3, intelligenceEffect = 0, reason = "Decizie miloasa si bine vazuta." } },
        { "ofer ajutor", new StatEvaluationResult { goldEffect = -2, respectEffect = 3, intelligenceEffect = 0, reason = "Decizie miloasa si bine vazuta." } },
        { "sprijin", new StatEvaluationResult { goldEffect = -2, respectEffect = 2, intelligenceEffect = 0, reason = "Decizie generoasa si bine primita." } },
        { "donez", new StatEvaluationResult { goldEffect = -3, respectEffect = 3, intelligenceEffect = 0, reason = "Decizie generoasa pentru cei in nevoie." } },

        { "cresc taxele", new StatEvaluationResult { goldEffect = 3, respectEffect = -2, intelligenceEffect = 0, reason = "Decizie profitabila, dar nepopulara." } },
        { "iau taxe", new StatEvaluationResult { goldEffect = 2, respectEffect = -2, intelligenceEffect = 0, reason = "Decizie dura, orientata spre castig." } },
        { "profit", new StatEvaluationResult { goldEffect = 2, respectEffect = -1, intelligenceEffect = 0, reason = "Decizie orientata spre castig." } },

        { "pedepsesc", new StatEvaluationResult { goldEffect = 0, respectEffect = -1, intelligenceEffect = 1, reason = "Decizie severa, dar controlata." } },
        { "folosesc forta", new StatEvaluationResult { goldEffect = 0, respectEffect = -2, intelligenceEffect = 0, reason = "Decizie dura si riscanta." } },
        { "trimit garzi", new StatEvaluationResult { goldEffect = -1, respectEffect = 1, intelligenceEffect = 1, reason = "Decizie autoritara, dar ordonata." } },

        { "plan", new StatEvaluationResult { goldEffect = 0, respectEffect = 0, intelligenceEffect = 2, reason = "Decizie gandita cu atentie." } },
        { "strategie", new StatEvaluationResult { goldEffect = 0, respectEffect = 0, intelligenceEffect = 3, reason = "Decizie strategica si calculata." } },
        { "investesc", new StatEvaluationResult { goldEffect = -1, respectEffect = 0, intelligenceEffect = 2, reason = "Decizie practica pentru viitor." } },
        { "solutie", new StatEvaluationResult { goldEffect = 0, respectEffect = 0, intelligenceEffect = 2, reason = "Decizie cautata cu cap." } },

        { "corect", new StatEvaluationResult { goldEffect = 0, respectEffect = 2, intelligenceEffect = 1, reason = "Decizie echilibrata si corecta." } },
        { "drept", new StatEvaluationResult { goldEffect = 0, respectEffect = 2, intelligenceEffect = 1, reason = "Decizie dreapta si respectata." } },
        { "echitabil", new StatEvaluationResult { goldEffect = 0, respectEffect = 2, intelligenceEffect = 1, reason = "Decizie justa si cumpanita." } },

        { "ignor", new StatEvaluationResult { goldEffect = 0, respectEffect = -2, intelligenceEffect = -1, reason = "Decizie rece si prost privita." } },
        { "nu fac nimic", new StatEvaluationResult { goldEffect = 0, respectEffect = -2, intelligenceEffect = -1, reason = "Lipsa de actiune slabeste increderea." } }
    };

    public StatEvaluationResult Evaluate(string playerResponse)
    {
        StatEvaluationResult total = new StatEvaluationResult();

        if (string.IsNullOrWhiteSpace(playerResponse))
        {
            total.reason = "Raspuns neclar si lipsit de directie.";
            return total;
        }

        string text = Normalize(playerResponse);

        bool matchedAnything = false;

        foreach (var rule in phraseRules)
        {
            if (text.Contains(rule.Key))
            {
                matchedAnything = true;

                bool isNegated = IsPhraseNegated(text, rule.Key);

                if (isNegated)
                {
                    total.goldEffect -= rule.Value.goldEffect;
                    total.respectEffect -= rule.Value.respectEffect;
                    total.intelligenceEffect -= rule.Value.intelligenceEffect;
                }
                else
                {
                    total.goldEffect += rule.Value.goldEffect;
                    total.respectEffect += rule.Value.respectEffect;
                    total.intelligenceEffect += rule.Value.intelligenceEffect;
                }
            }
        }

        if (!matchedAnything)
        {
            total.reason = "Decizie neclara, cu efect modest.";
            return total;
        }

        total.goldEffect = Mathf.Clamp(total.goldEffect, -4, 4);
        total.respectEffect = Mathf.Clamp(total.respectEffect, -4, 4);
        total.intelligenceEffect = Mathf.Clamp(total.intelligenceEffect, -4, 4);

        total.reason = BuildReason(total);

        return total;
    }

    private string Normalize(string text)
    {
        text = text.ToLower();
        text = text.Replace(",", " ");
        text = text.Replace(".", " ");
        text = text.Replace("!", " ");
        text = text.Replace("?", " ");
        text = text.Replace("  ", " ");
        return text;
    }

    private bool IsPhraseNegated(string text, string phrase)
    {
        int index = text.IndexOf(phrase);
        if (index < 0)
            return false;

        int windowStart = Mathf.Max(0, index - 20);
        string contextBefore = text.Substring(windowStart, index - windowStart);

        foreach (string negation in negationWords)
        {
            if (contextBefore.Contains(negation))
                return true;
        }

        return false;
    }

    private string BuildReason(StatEvaluationResult result)
    {
        if (result.respectEffect > 0 && result.intelligenceEffect > 0)
            return "Decizie bine vazuta si destul de inteleapta.";

        if (result.goldEffect > 0 && result.respectEffect < 0)
            return "Decizie profitabila, dar nepopulara.";

        if (result.respectEffect > 0 && result.goldEffect < 0)
            return "Decizie generoasa si bine primita.";

        if (result.intelligenceEffect > 0)
            return "Decizie calculata si practica.";

        if (result.respectEffect < 0)
            return "Decizie dura si prost vazuta.";

        return "Decizie cu efect moderat.";
    }
}