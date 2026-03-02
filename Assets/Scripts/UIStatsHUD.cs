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
        goldText.text = "Gold: " + GameState.Instance.Gold;
        respectText.text = "Respect: " + GameState.Instance.Respect;
        intelligenceText.text = "Intelect: " + GameState.Instance.Intelligence;
    }
}