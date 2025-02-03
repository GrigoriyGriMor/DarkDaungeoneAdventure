using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderController : MonoBehaviour, IDragHandler,
        IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool useVerticalAxies = true;
    [SerializeField] private bool useHorizontalAxis = true;

    [SerializeField] private Image tr_plane;
    private Vector2 inputVector;

    public Vector2 _stickPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.LogError("pointDown");

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.LogError("drag");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}
