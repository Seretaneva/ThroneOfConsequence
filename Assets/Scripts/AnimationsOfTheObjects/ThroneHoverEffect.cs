using UnityEngine;
using UnityEngine.InputSystem;

public class ThroneHoverEffect : MonoBehaviour
{
    [SerializeField] private AudienceSequenceController audienceSequenceController;

    private Camera mainCam;
    private Collider2D col;
    private Vector3 originalScale;
    private bool isHovering;

    private void Awake()
    {
        mainCam = Camera.main;
        col = GetComponent<Collider2D>();
        originalScale = transform.localScale;

        if (audienceSequenceController == null)
            audienceSequenceController = FindFirstObjectByType<AudienceSequenceController>();
    }

    private void Update()
    {
        if (audienceSequenceController != null &&
            !audienceSequenceController.IsThroneInteractionAllowed)
        {
            if (isHovering)
            {
                isHovering = false;
                transform.localScale = originalScale;
            }

            return;
        }

        if (Mouse.current == null || mainCam == null || col == null)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(mouseScreenPos);
        Vector2 mousePoint2D = new Vector2(mouseWorld.x, mouseWorld.y);

        bool mouseIsOver = col.OverlapPoint(mousePoint2D);

        if (mouseIsOver && !isHovering)
        {
            isHovering = true;
            transform.localScale = originalScale * 1.08f;
        }
        else if (!mouseIsOver && isHovering)
        {
            isHovering = false;
            transform.localScale = originalScale;
        }
    }
}
