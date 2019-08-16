using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CustomReorderableElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform _draggingElement;
    private LayoutElement _draggingElementLE;
    private Vector2 _draggingElementOriginalSize;
    private RectTransform _fakeElement;
    private LayoutElement _fakeElementLE;
    private int _fromIndex;
    private bool _isDragging;
    public RectTransform rect;
    private CustomReorderableList _reorderableList;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _draggingElement = rect;
        _fromIndex = rect.GetSiblingIndex();

        _draggingElementOriginalSize = gameObject.GetComponent<RectTransform>().rect.size;
        _draggingElementLE = _draggingElement.GetComponent<LayoutElement>();
        _draggingElement.SetParent(_reorderableList.DraggableArea, true);
        _draggingElement.SetAsLastSibling();

        _fakeElement = new GameObject("FakeElement").AddComponent<RectTransform>();
        _fakeElementLE = _fakeElement.gameObject.AddComponent<LayoutElement>();

        RefreshSizes();

        _reorderableList.OnElementGrabbed?.Invoke(new CustomReorderableList.ReorderableListEventStruct
        {
            DroppedObject = _draggingElement.gameObject,
            FromIndex = _fromIndex,
        });

        _isDragging = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging)
            return;

        var canvas = _draggingElement.GetComponentInParent<Canvas>();
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, canvas.worldCamera, out worldPoint);

        switch (_reorderableList.clampAxis)
        {
            case CustomReorderableList.ClampAxis.None:
                _draggingElement.position = worldPoint;
                break;
            case CustomReorderableList.ClampAxis.X_axis:
                _draggingElement.position = new Vector3(worldPoint.x, _draggingElement.position.y, _draggingElement.position.z);
                break;
            case CustomReorderableList.ClampAxis.Y_axis:
                _draggingElement.position = new Vector3(_draggingElement.position.x, worldPoint.y, _draggingElement.position.z);
                break;
        }

        if (_fakeElement.parent != _reorderableList)
            _fakeElement.SetParent(_reorderableList.Content, false);

        float minDistance = float.PositiveInfinity;
        int targetIndex = 0;
        float dist = 0;
        for (int j = 0; j < _reorderableList.Content.childCount; j++)
        {
            var c = _reorderableList.Content.GetChild(j).GetComponent<RectTransform>();

            if (_reorderableList.ContentLayout is VerticalLayoutGroup)
                dist = Mathf.Abs(c.position.y - worldPoint.y);
            else if (_reorderableList.ContentLayout is HorizontalLayoutGroup)
                dist = Mathf.Abs(c.position.x - worldPoint.x);
            else if (_reorderableList.ContentLayout is GridLayoutGroup)
                dist = (Mathf.Abs(c.position.x - worldPoint.x) + Mathf.Abs(c.position.y - worldPoint.y));

            if (dist < minDistance)
            {
                minDistance = dist;
                targetIndex = j;
            }
        }

        RefreshSizes();
        _fakeElement.SetSiblingIndex(targetIndex);
        _fakeElement.gameObject.SetActive(true);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;

        if (_draggingElement != null)
        {
            _reorderableList.OnElementDropped?.Invoke(new CustomReorderableList.ReorderableListEventStruct
            {
                DroppedObject = _draggingElement.gameObject,
                FromIndex = _fromIndex,
                ToIndex = _fakeElement.GetSiblingIndex()
            });

            RefreshSizes();
            _draggingElement.SetParent(_reorderableList.Content, false);
            _draggingElement.rotation = _reorderableList.transform.rotation;
            _draggingElement.SetSiblingIndex(_fakeElement.GetSiblingIndex());
        }

        if (_fakeElement != null)
            Destroy(_fakeElement.gameObject);
    }

    private void CancelDrag()
    {
        _isDragging = false;

        RefreshSizes();

        _draggingElement.SetParent(_reorderableList.Content, false);
        _draggingElement.rotation = _reorderableList.Content.transform.rotation;
        _draggingElement.SetSiblingIndex(_fromIndex);

        if (_fakeElement != null)
            Destroy(_fakeElement.gameObject);
    }

    private void RefreshSizes()
    {
        Vector2 size = _draggingElementOriginalSize;

        _draggingElement.sizeDelta = size;
        _fakeElementLE.preferredHeight = _draggingElementLE.preferredHeight = size.y;
        _fakeElementLE.preferredWidth = _draggingElementLE.preferredWidth = size.x;
    }

    public void InitializeReorderableItem(CustomReorderableList rdList)
    {
        _reorderableList = rdList;
        rect = GetComponent<RectTransform>();
    }
}
