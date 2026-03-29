using UnityEngine;
using UnityEngine.InputSystem;

public class DoorClickSimple : MonoBehaviour
{
    [SerializeField] private AudienceSequenceController audienceController;

    private Camera mainCam;
    private Collider2D col;

    private void Awake()
    {
        mainCam = Camera.main;
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (mainCam == null)
            {
                Debug.LogError("Nu exista Main Camera cu tag MainCamera");
                return;
            }

            if (col == null)
            {
                Debug.LogError("Obiectul usii nu are Collider2D");
                return;
            }

            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = mainCam.ScreenToWorldPoint(mouseScreenPos);
            Vector2 mousePoint2D = new Vector2(mouseWorld.x, mouseWorld.y);

            if (col.OverlapPoint(mousePoint2D))
            {
                Debug.Log("Click pe usa detectat");

                if (audienceController == null)
                {
                    Debug.LogError("audienceController este null");
                    return;
                }

                audienceController.OnDoorClicked();
            }
        }
    }
}