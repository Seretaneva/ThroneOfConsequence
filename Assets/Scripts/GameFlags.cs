using System.Collections.Generic;

public static class GameFlags
{
    private static HashSet<string> flags = new HashSet<string>();

    public static void SetFlag(string flag)
    {
        flags.Add(flag);
    }

    public static bool HasFlag(string flag)
    {
        return flags.Contains(flag);
    }

    public static void RemoveFlag(string flag)
    {
        flags.Remove(flag);
    }

    public static void ClearFlags()
    {
        flags.Clear();
    }
}