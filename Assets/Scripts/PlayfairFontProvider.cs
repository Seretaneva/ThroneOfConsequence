using TMPro;
using UnityEngine;

public static class PlayfairFontProvider
{
    private static TMP_FontAsset regular;
    private static TMP_FontAsset semiBold;
    private static TMP_FontAsset italic;
    private static TMP_FontAsset fallback;

    public static TMP_FontAsset Regular =>
        regular ??= CreateFontAsset("Fonts/PlayfairDisplay-Regular", "Playfair Display Regular");

    public static TMP_FontAsset SemiBold =>
        semiBold ??= CreateFontAsset("Fonts/PlayfairDisplay-SemiBold", "Playfair Display SemiBold");

    public static TMP_FontAsset Italic =>
        italic ??= CreateFontAsset("Fonts/PlayfairDisplay-Italic", "Playfair Display Italic");

    private static TMP_FontAsset CreateFontAsset(string resourcePath, string assetName)
    {
        Font sourceFont = Resources.Load<Font>(resourcePath);

        if (sourceFont == null)
        {
            Debug.LogWarning("Font source was not found at Resources/" + resourcePath);
            return GetFallback();
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont);

        if (fontAsset == null)
        {
            Debug.LogWarning("TMP font could not be created from " + resourcePath);
            return GetFallback();
        }

        fontAsset.name = assetName;
        return fontAsset;
    }

    private static TMP_FontAsset GetFallback()
    {
        if (fallback == null)
            fallback = Resources.Load<TMP_FontAsset>("Fonts & Materials/Oswald Bold SDF");

        return fallback;
    }
}
