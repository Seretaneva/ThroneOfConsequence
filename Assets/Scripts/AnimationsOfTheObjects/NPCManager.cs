using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCManager : MonoBehaviour
{
    public NPCData[] npcs;

    public SpriteRenderer npcBodyRenderer;
    public Image portraitImage;

    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Animator npcAnimator;
    public void LoadRandomNPC()
    {
        int index = Random.Range(0, npcs.Length);
        NPCData npc = npcs[index];

        npcBodyRenderer.sprite = npc.bodySprite;
        portraitImage.sprite = npc.portraitSprite;

        titleText.text = npc.title;
        descriptionText.text = npc.description;
        npcBodyRenderer.sprite = npc.bodySprite;
        portraitImage.sprite = npc.portraitSprite;

        if (npcAnimator != null && npc.animatorController != null)
        {
            npcAnimator.runtimeAnimatorController = npc.animatorController;
        }
            }
}