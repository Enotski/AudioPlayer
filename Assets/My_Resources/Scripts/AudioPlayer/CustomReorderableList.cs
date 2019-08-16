using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
public class CustomReorderableList : MonoBehaviour
{
    public ClampAxis clampAxis = ClampAxis.None;

    public LayoutGroup ContentLayout;
    public RectTransform DraggableArea;
    public RectTransform Content;
    public static CustomReorderableContent listContent;

    [Serializable]
    public class ReorderableListHandler : UnityEvent<ReorderableListEventStruct>
    {
    }

    [Header("UI Re-orderable Events")]
    public ReorderableListHandler OnElementDropped = new ReorderableListHandler();
    public ReorderableListHandler OnElementGrabbed = new ReorderableListHandler();
    public ReorderableListHandler OnElementRemoved = new ReorderableListHandler();
    public ReorderableListHandler OnElementAdded = new ReorderableListHandler();

    [Serializable]
    public struct ReorderableListEventStruct
    {
        public GameObject DroppedObject;
        public int FromIndex;
        public int ToIndex;
    }

    public enum ClampAxis
    {
        None,
        Y_axis,
        X_axis
    }

    private void Awake()
    {
        Content = ContentLayout.GetComponent<RectTransform>();
        listContent = ContentLayout.gameObject.AddComponent<CustomReorderableContent>();
        listContent.InitializeReorderableContent(this);
    }
}
