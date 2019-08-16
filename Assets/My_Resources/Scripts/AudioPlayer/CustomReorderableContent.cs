using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomReorderableContent : MonoBehaviour
{
    private List<Transform> _cachedElementsTransforms;
    private List<CustomReorderableElement> _cachedRdElements;
    private CustomReorderableElement _rdElement;
    private CustomReorderableList _rdList;
    private RectTransform _rect;

    public void InitializeReorderableContent(CustomReorderableList rdList)
    {
        _rdList = rdList;
        _rect = GetComponent<RectTransform>();
        _cachedRdElements = new List<CustomReorderableElement>();
        _cachedElementsTransforms = new List<Transform>();

        StartCoroutine(RefreshRdItems());
    }

    private IEnumerator RefreshRdItems()
    {

        for(int i = 0; i<_rect.childCount; i++)
        {
            if (_cachedElementsTransforms.Contains(_rect.GetChild(i)))
                continue;

            _rdElement = _rect.GetChild(i).gameObject.GetComponent<CustomReorderableElement>() ??
                       _rect.GetChild(i).gameObject.AddComponent<CustomReorderableElement>();
            _rdElement.InitializeReorderableItem(_rdList);

            _cachedElementsTransforms.Add(_rect.GetChild(i));
            _cachedRdElements.Add(_rdElement);
        }

        yield return 0;

        for(int i =  _cachedElementsTransforms.Count - 1; i >= 0; i--)
        {
            if(_cachedElementsTransforms[i] == null)
            {
                _cachedElementsTransforms.RemoveAt(i);
                _cachedElementsTransforms.RemoveAt(i);
            }
        }
    }
}
