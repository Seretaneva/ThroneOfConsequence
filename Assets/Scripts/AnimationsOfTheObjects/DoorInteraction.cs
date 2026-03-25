using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Sprites")]
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Peasant")]
    public GameObject peasant;
    public Transform peasantTargetPoint;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip knockClip;

    private SpriteRenderer sr;
    private bool canOpen = false;
    private bool opened = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (closedSprite != null)
            sr.sprite = closedSprite;

        if (peasant != null)
            peasant.SetActive(false);
    }

    public void PlayKnock()
    {
        canOpen = true;

        if (audioSource != null && knockClip != null)
            audioSource.PlayOneShot(knockClip);
    }

    private void OnMouseDown()
    {
        if (!canOpen || opened)
            return;

        OpenDoor();
    }

    private void OpenDoor()
    {
        opened = true;

        if (openSprite != null)
            sr.sprite = openSprite;

        if (peasant != null)
        {
            peasant.SetActive(true);

            PeasantMover mover = peasant.GetComponent<PeasantMover>();
            if (mover != null && peasantTargetPoint != null)
            {
                mover.MoveTo(peasantTargetPoint.position);
            }
        }
    }
}