using UnityEngine;
using UnityEngine.InputSystem;

public class OpenThroneOnClick : MonoBehaviour
{
    [SerializeField] private GameObject throneStats;
    [SerializeField] private AudienceSequenceController audienceSequenceController;

    private Camera mainCam;
    private Collider2D col;

    private void Awake()
    {
        mainCam = Camera.main;
        col = GetComponent<Collider2D>();

        if (audienceSequenceController == null)
            audienceSequenceController = FindFirstObjectByType<AudienceSequenceController>();
    }

    private void Update()
    {
        if (!CanInteractWithThrone())
        {
            if (throneStats != null && throneStats.activeSelf)
                throneStats.SetActive(false);

            return;
        }

        if (Mouse.current == null || mainCam == null || col == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = mainCam.ScreenToWorldPoint(mouseScreenPos);
            Vector2 mousePoint2D = new Vector2(mouseWorld.x, mouseWorld.y);

            if (col.OverlapPoint(mousePoint2D))
            {
                Debug.Log("Click pe " + gameObject.name);

                if (throneStats != null)
                    throneStats.SetActive(!throneStats.activeSelf);
            }
        }
    }

    private bool CanInteractWithThrone()
    {
        return audienceSequenceController == null ||
               audienceSequenceController.IsThroneInteractionAllowed;
    }
}
