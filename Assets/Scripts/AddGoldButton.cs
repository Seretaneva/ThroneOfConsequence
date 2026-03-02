using UnityEngine;

public class AddGoldButton : MonoBehaviour
{
    public void OnClickAddGold()
    {
        GameState.Instance.AddGold(10);
    }
}
