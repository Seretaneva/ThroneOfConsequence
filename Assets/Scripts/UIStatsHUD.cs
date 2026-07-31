using TMPro;
using UnityEngine;

public class UIStatsHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text respectText;
    [SerializeField] private TMP_Text intelligenceText;
    [SerializeField] private TMP_FontAsset statsFont;

    private void OnValidate()
    {
        if (statsFont == null)
            return;

        if (goldText != null)
            goldText.font = statsFont;

        if (respectText != null)
            respectText.font = statsFont;

        if (intelligenceText != null)
            intelligenceText.font = statsFont;
    }

    private void OnEnable()
    {
        ApplyFont();
        GameState.Instance.OnStatsChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        if (GameState.Instance != null)
            GameState.Instance.OnStatsChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        goldText.text = ": " + GameState.Instance.Gold;
        respectText.text = ": " + GameState.Instance.Respect;
        intelligenceText.text = ": " + GameState.Instance.Intelligence;
    }

    private void ApplyFont()
    {
        TMP_FontAsset font = statsFont != null
            ? statsFont
            : PlayfairFontProvider.SemiBold;

        if (font == null)
            return;

        goldText.font = font;
        respectText.font = font;
        intelligenceText.font = font;
    }
}
