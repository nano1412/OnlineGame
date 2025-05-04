using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isHolding;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
    }
}
