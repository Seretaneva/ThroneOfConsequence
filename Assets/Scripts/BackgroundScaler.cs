using UnityEngine;

[ExecuteAlways]
public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        ScaleBackground();
    }

    void Update()
    {
        ScaleBackground();
    }

    void ScaleBackground()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
            return;

        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;

        Vector2 spriteSize = sr.sprite.bounds.size;

        float scaleX = screenWidth / spriteSize.x;
        float scaleY = screenHeight / spriteSize.y;

        float scale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(scale, scale, 1f);

        transform.position = new Vector3(0, 0, 0);
    }
}