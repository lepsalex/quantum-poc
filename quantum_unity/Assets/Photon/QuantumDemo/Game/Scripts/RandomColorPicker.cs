using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorPicker : MonoBehaviour
{
    void Start()
    {
        Renderer cubeRenderer = GetComponent<Renderer>();
        cubeRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 0.7f, 0.9f));
    }
}
