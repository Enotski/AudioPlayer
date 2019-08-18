using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class CustomButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Image btnImage;
    public Image btnChildImage;

    public Color btnNormalImgColor;
    public Color btnPressedImgColor;
    public Color btnChildImgNormalColor;
    public Color btnChildPressedImgColor;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(btnImage == null && btnChildImage == null)
        {
            Debug.LogWarning("Interactible image don`t assigned!");
            return;
        }

        btnImage.color = btnPressedImgColor;
        btnChildImage.color = btnChildPressedImgColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (btnImage == null && btnChildImage == null)
        {
            Debug.LogWarning("Interactible image don`t assigned!");
            return;
        }

        btnImage.color = btnNormalImgColor;
        btnChildImage.color = btnChildImgNormalColor;
    }
}
