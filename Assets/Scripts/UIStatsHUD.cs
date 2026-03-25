using TMPro;
using UnityEngine;

public class UIStatsHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text respectText;
    [SerializeField] private TMP_Text intelligenceText;

    private void OnEnable()
    {
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
}