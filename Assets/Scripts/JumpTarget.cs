using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTarget : MonoBehaviour
{
    public WalkablePolygon area;

    private void OnValidate()
    {
        var parentArea = GetComponentInParent<WalkablePolygon>();
        area = parentArea;
    }
}