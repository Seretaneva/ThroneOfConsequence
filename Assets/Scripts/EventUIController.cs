using TMPro;
using UnityEngine;

public class EventUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text eventText;

    // un mic "index" ca să schimbăm evenimentul după alegere
    private int eventIndex = 0;

    private void Start()
    {
        ShowEvent(eventIndex);
    }

    // A / B / C vor fi chemate de butoane
    public void ChooseA()
    {
        // Îl ajut: pierd bani, câștig respect
        GameState.Instance.AddGold(-10);
        GameState.Instance.AddRespect(+5);

        NextEvent();
    }

    public void ChooseB()
    {
        // Îl ignor: câștig bani (nu cheltui), pierd respect
        GameState.Instance.AddGold(+5);
        GameState.Instance.AddRespect(-5);

        NextEvent();
    }

    public void ChooseC()
    {
        // Pedepsesc hoțul: respect crește, inteligența crește puțin (ordine)
        GameState.Instance.AddRespect(+3);
        GameState.Instance.AddIntelligence(+2);

        NextEvent();
    }

    private void NextEvent()
    {
        GameState.Instance.NextDay();  // opțional, ca să simți „timpul”
        eventIndex++;
        ShowEvent(eventIndex);
    }

    private void ShowEvent(int index)
    {
        // Pentru început, doar 2-3 evenimente hardcodate
        if (index == 0)
            eventText.text = "Un țăran: Mi-au fost furate grânele! Ce faci?";
        else if (index == 1)
            eventText.text = "O văduvă: Nu am bani de taxe luna asta. Ce decizi?";
        else if (index == 2)
            eventText.text = "Un negustor: Vreau să deschid o piață, dar cer protecție. Accepti?";
        else
            eventText.text = "Nu mai sunt evenimente (deocamdată).";
    }
}