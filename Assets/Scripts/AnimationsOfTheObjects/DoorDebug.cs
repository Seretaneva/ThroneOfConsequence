using UnityEngine;

public class DoorDebug : MonoBehaviour
{
    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        Debug.Log($"Collider exista: {col != null}, enabled: {col?.enabled}");
        Debug.Log($"Layer obiect usa: {gameObject.layer}");
    }

    void OnMouseDown()
    {
        Debug.Log("✅ OnMouseDown FUNCTIONEAZA pe usa!");
    }

    void OnMouseOver()
    {
        Debug.Log("Mouse PESTE usa");
    }
}