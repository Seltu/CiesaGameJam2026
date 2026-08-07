using UnityEngine;
using UnityEngine.EventSystems;

public class MoveIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private BattleMove _move;
    [SerializeField] private RectTransform _moveSprite;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    private Vector2 _originalPosition;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = _moveSprite.GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
            _canvasGroup = _moveSprite.gameObject.AddComponent<CanvasGroup>();
        }

        _originalPosition = _moveSprite.anchoredPosition;
    }

    private void OnEnable()
    {
        _moveSprite.anchoredPosition = _originalPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _moveSprite.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        GameObject objectUnderPointer = eventData.pointerCurrentRaycast.gameObject;

        if (objectUnderPointer != null)
        {
            Transform hoveredTransform = objectUnderPointer.transform;

            while (hoveredTransform != null)
            {
                if (hoveredTransform.CompareTag("MoveSlot"))
                {
                    OnDroppedOnMoveSlot(hoveredTransform);
                    return;
                }

                hoveredTransform = hoveredTransform.parent;
            }
        }

        _moveSprite.anchoredPosition = _originalPosition;
    }

    public BattleMove GetMove()
    {
        return _move;
    }

    private void OnDroppedOnMoveSlot(Transform slotTransform)
    {
        _moveSprite.position = slotTransform.position;
        EventManager.TriggerEvent("MoveDrop", this, slotTransform);
    }
}