using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleShapeScaler : MonoBehaviour
{
    private ParticleSystem.ShapeModule shapeModule;
    private RectTransform rootRect;
    public Rect pixelRect;
    void Start()
    {
        shapeModule = GetComponent<ParticleSystem>().shape;
        rootRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        pixelRect = rootRect.rect;
        shapeModule.scale = new Vector3(pixelRect.width, pixelRect.height);
    }
}
