using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudienceSequenceController : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private GameObject doorClosed;
    [SerializeField] private GameObject doorOpen;

    [Header("NPC")]
    [SerializeField] private GameObject peasantObject; 
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

    private bool canClickDoor = false;
    private bool sequenceRunning = false;
    private Animator peasantAnim;

    private IEnumerator Start()
    {
        if (peasantObject != null)
        {
            peasantObject.SetActive(false);
            peasantObject.transform.position = spawnPoint.position;
        }

        if (peasantObject != null)
        {
            peasantObject.SetActive(false);
            peasantAnim = peasantObject.GetComponent<Animator>();
        }

        if (eventUIController != null)
            eventUIController.HideEventUI();

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        if (doorClosed != null) doorClosed.SetActive(true);
        if (doorOpen != null) doorOpen.SetActive(false);

        yield return null;

        while (EventManager.Instance == null || EventManager.Instance.GetCurrentEvent() == null)
            yield return null;

        PrepareForVisitor();
    }

    private void PrepareForVisitor()
    {
        Debug.Log("PrepareForVisitor apelat");
        sequenceRunning = false;
        canClickDoor = true;

        if (eventUIController != null)
            eventUIController.HideEventUI();

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

    
        if (peasantObject != null)
        {
            peasantObject.SetActive(false);
            peasantObject.transform.position = spawnPoint.position;
        }

        if (doorClosed != null) doorClosed.SetActive(true);
        if (doorOpen != null) doorOpen.SetActive(false);

        if (audioSource != null && knockClip != null)
            audioSource.PlayOneShot(knockClip);
    }

    public void OnDoorClicked()
    {
        Debug.Log($"OnDoorClicked apelat | canClickDoor={canClickDoor} | sequenceRunning={sequenceRunning}");

        if (!canClickDoor || sequenceRunning)
        {
            Debug.LogWarning("Click blocat");
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
        if (currentEvent == null) { Debug.LogError("No current event."); yield break; }

        sequenceRunning = true;
        canClickDoor = false;

        if (eventUIController != null) eventUIController.HideEventUI();
        if (portraitImage != null) portraitImage.gameObject.SetActive(false);

        // ✅ Deschide usa
        if (doorClosed != null) doorClosed.SetActive(false);
        if (doorOpen != null) doorOpen.SetActive(true);

        yield return new WaitForSeconds(0.3f);

       
        if (peasantObject != null)
        {
            peasantObject.transform.position = spawnPoint.position;
            peasantObject.SetActive(true);

            if (peasantAnim != null)
                peasantAnim.SetBool("isWalking", true);
        }

      
        while (peasantObject != null &&
               Vector3.Distance(peasantObject.transform.position, stopPoint.position) > 0.05f)
        {
            peasantObject.transform.position = Vector3.MoveTowards(
                peasantObject.transform.position,
                stopPoint.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (peasantObject != null)
        {
            peasantObject.transform.position = stopPoint.position;
            if (peasantAnim != null)
                peasantAnim.SetBool("isWalking", false);
        }

        yield return new WaitForSeconds(0.2f);

        // ✅ Arată portretul și UI-ul
        if (portraitImage != null && peasantPortrait != null)
        {
            portraitImage.sprite = peasantPortrait;
            portraitImage.gameObject.SetActive(true);
        }

        if (eventUIController != null)
            eventUIController.ShowEvent(currentEvent);
    }
}