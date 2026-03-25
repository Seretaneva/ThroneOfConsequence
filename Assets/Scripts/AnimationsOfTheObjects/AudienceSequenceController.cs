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

    private GameObject currentNpc;

    private IEnumerator Start()
    {
        yield return null;

        while (EventManager.Instance == null || EventManager.Instance.GetCurrentEvent() == null)
            yield return null;

        StartCoroutine(PlayCurrentEventSequence());
    }

    public void StartNextEventSequence()
    {
        StartCoroutine(PlayCurrentEventSequence());
    }

    private IEnumerator PlayCurrentEventSequence()
    {
        EventData currentEvent = EventManager.Instance.GetCurrentEvent();

        if (currentEvent == null)
        {
            Debug.LogError("No current event in sequence.");
            yield break;
        }

        if (eventUIController != null)
            eventUIController.HideEventUI();

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        if (currentNpc != null)
            Destroy(currentNpc);

        // usa inchisa initial
        if (doorClosed != null) doorClosed.SetActive(true);
        if (doorOpen != null) doorOpen.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        // deschide usa
        if (doorClosed != null) doorClosed.SetActive(false);
        if (doorOpen != null) doorOpen.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        // spawn npc
        currentNpc = Instantiate(peasantPrefab, spawnPoint.position, Quaternion.identity);

        // merge spre stop point
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

        // apare portretul
        if (portraitImage != null && peasantPortrait != null)
        {
            portraitImage.sprite = peasantPortrait;
            portraitImage.gameObject.SetActive(true);
        }

        // apare textul si butoanele
        if (eventUIController != null)
            eventUIController.ShowEvent(currentEvent);
    }
}