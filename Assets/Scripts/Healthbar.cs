using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image image;
    private int max;

    public void Initialize(int max)
    {
        this.max = max;
    }

    public void SetHealth(int current)
    {
        image.fillAmount = (float)current / max;
    }
}