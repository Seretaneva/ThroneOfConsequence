using UnityEngine;

public class AddRespectButton : MonoBehaviour
{
    public void OnClickAddRespect()
    {
        GameState.Instance.AddRespect(1);
    }
}
