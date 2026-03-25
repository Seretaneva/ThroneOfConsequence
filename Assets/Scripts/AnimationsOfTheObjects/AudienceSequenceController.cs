using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudienceSequenceController : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private GameObject doorClosed;
    [SerializeField] private GameObject doorOpen;

    [Header("NPC")]
    [SerializeField] private GameObject peasantPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform stopPoint;
    [SerializeField] private float moveSpeed = 2f;

    [Header("UI")]
    [SerializeField] private EventUIController eventUIController;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite peasantPortrait;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip knockClip;

    private GameObject currentNpc;
    private bool canClickDoor = false;
    private bool sequenceRunning = false;

    private IEnumerator Start()
    {
        yield return null;

        while (EventManager.Instance == null || EventManager.Instance.GetCurrentEvent() == null)
            yield return null;

        PrepareForVisitor();
    }

    private void PrepareForVisitor()
    {   
        Debug.Log("PrepareForVisitor a fost apelat");
        sequenceRunning = false;
        canClickDoor = true;
        
    
        if (eventUIController != null)
            eventUIController.HideEventUI();

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        if (currentNpc != null)
        {
            Destroy(currentNpc);
            currentNpc = null;
        }

        if (doorClosed != null) doorClosed.SetActive(true);
        if (doorOpen != null) doorOpen.SetActive(false);

        if (audioSource != null && knockClip != null)
            audioSource.PlayOneShot(knockClip);
    }

    public void OnDoorClicked()
    {
        Debug.Log("OnDoorClicked a fost apelat");

        if (!canClickDoor || sequenceRunning)
        {
            Debug.LogWarning($"Nu pot porni secventa. canClickDoor={canClickDoor}, sequenceRunning={sequenceRunning}");
            return;
        }

        StartCoroutine(PlayCurrentEventSequence());
    }

    public void StartNextEventSequence()
    {
        PrepareForVisitor();
    }

    private IEnumerator PlayCurrentEventSequence()
    {
        EventData currentEvent = EventManager.Instance.GetCurrentEvent();

        if (currentEvent == null)
        {
            Debug.LogError("No current event in sequence.");
            yield break;
        }

        if (peasantPrefab == null || spawnPoint == null || stopPoint == null)
        {
            Debug.LogError("PeasantPrefab / spawnPoint / stopPoint lipsesc din Inspector.");
            yield break;
        }

        sequenceRunning = true;
        canClickDoor = false;

        if (eventUIController != null)
            eventUIController.HideEventUI();

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        if (doorClosed != null) doorClosed.SetActive(false);
        if (doorOpen != null) doorOpen.SetActive(true);

        yield return new WaitForSeconds(0.15f);

        currentNpc = Instantiate(peasantPrefab, spawnPoint.position, Quaternion.identity);

        while (Vector3.Distance(currentNpc.transform.position, stopPoint.position) > 0.05f)
        {
            currentNpc.transform.position = Vector3.MoveTowards(
                currentNpc.transform.position,
                stopPoint.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        currentNpc.transform.position = stopPoint.position;

        yield return new WaitForSeconds(0.2f);

        if (portraitImage != null && peasantPortrait != null)
        {
            portraitImage.sprite = peasantPortrait;
            portraitImage.gameObject.SetActive(true);
        }

        if (eventUIController != null)
            eventUIController.ShowEvent(currentEvent);
    }
}