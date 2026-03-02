using UnityEngine;

public class AddIntelligenceButton : MonoBehaviour
{
   public void OnClickAddIntelligence(){
    GameState.Instance.AddIntelligence(1);
   }
}
