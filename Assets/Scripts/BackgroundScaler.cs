using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScaler : MonoBehaviour
{
  void Update()
{
    ScaleBackground();
}
    void ScaleBackground()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        Vector2 spriteSize = sr.sprite.bounds.size;

        float scaleX = worldScreenWidth / spriteSize.x;
        float scaleY = worldScreenHeight / spriteSize.y;

        // Alegem valoarea mai mare ca sa acopere tot ecranul
        float scale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
