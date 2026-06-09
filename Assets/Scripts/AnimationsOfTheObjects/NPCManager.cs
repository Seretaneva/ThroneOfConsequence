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
    public NPCData CurrentNPC { get; private set; }

    private Vector3 originalBodyScale;
    private bool originalBodyScaleCached;

    public NPCData LoadRandomNPC()
    {
        if (npcs == null || npcs.Length == 0)
        {
            Debug.LogError("NPCManager nu are NPC-uri configurate.");
            return null;
        }

        int index = Random.Range(0, npcs.Length);
        NPCData npc = npcs[index];

        ApplyNPC(npc);
        return npc;
    }

    public void ApplyNPC(NPCData npc)
    {
        if (npc == null)
        {
            Debug.LogError("NPC-ul primit este null.");
            return;
        }

        CurrentNPC = npc;

        if (npcAnimator == null && npcBodyRenderer != null)
            npcAnimator = npcBodyRenderer.GetComponentInParent<Animator>();

        if (npcBodyRenderer != null)
        {
            CacheOriginalBodyScale();
            npcBodyRenderer.sprite = npc.bodySprite;
            npcBodyRenderer.transform.localScale = originalBodyScale * Mathf.Max(0.1f, npc.bodyScale);
        }

        if (portraitImage != null)
            portraitImage.sprite = npc.portraitSprite;

        if (titleText != null)
            titleText.text = npc.title;

        if (descriptionText != null)
            descriptionText.text = npc.description;

        if (npcAnimator != null && npc.animatorController != null)
        {
            npcAnimator.runtimeAnimatorController = npc.animatorController;
            npcAnimator.Rebind();
            npcAnimator.Update(0f);
        }
    }

    private void CacheOriginalBodyScale()
    {
        if (originalBodyScaleCached || npcBodyRenderer == null)
            return;

        originalBodyScale = npcBodyRenderer.transform.localScale;
        originalBodyScaleCached = true;
    }
}
