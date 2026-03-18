
using System.Collections.Generic;

public static class GameFlags
{
    private static HashSet<string> flags = new HashSet<string>();

    private static string Normalize(string flag)
    {
        return flag.Trim().ToLower();
    }

    public static void SetFlag(string flag)
    {
        flags.Add(Normalize(flag));
    }

    public static bool HasFlag(string flag)
    {
        return flags.Contains(Normalize(flag));
    }

    public static void RemoveFlag(string flag)
    {
        flags.Remove(flag);
    }

    public static void ClearFlags()
    {
        flags.Clear();
    }

    public static bool HasAllFlags(List<string> requiredFlags)
    {
        if (requiredFlags == null) return true;

        foreach (var flag in requiredFlags)
        {
            if (!string.IsNullOrEmpty(flag) && !flags.Contains(flag))
                return false;
        }

        return true;
    }

    public static bool HasAnyFlag(List<string> checkFlags)
    {
        if (checkFlags == null) return false;

        foreach (var flag in checkFlags)
        {
            if (!string.IsNullOrEmpty(flag) && flags.Contains(flag))
                return true;
        }

        return false;
    }
}