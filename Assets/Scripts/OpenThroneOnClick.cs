using UnityEngine;
using UnityEngine.InputSystem;

public class OpenThroneOnClick : MonoBehaviour
{
    [SerializeField] private GameObject throneStats;

    private Camera mainCam;
    private Collider2D col;

    private void Awake()
    {
        mainCam = Camera.main;
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = mainCam.ScreenToWorldPoint(mouseScreenPos);
            Vector2 mousePoint2D = new Vector2(mouseWorld.x, mouseWorld.y);

            if (col != null && col.OverlapPoint(mousePoint2D))
            {
                Debug.Log("Click pe " + gameObject.name);

                throneStats.SetActive(!throneStats.activeSelf);
            }
        }
        
    }
}