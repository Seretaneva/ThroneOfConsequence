using UnityEngine;
using UnityEngine.EventSystems;

public class DoorClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudienceSequenceController controller;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK PE USA");

        if (controller == null)
        {
            Debug.LogError("Controller este NULL!");
            return;
        }

        controller.OnDoorClicked();
    }
}