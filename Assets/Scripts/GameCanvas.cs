using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas instance;
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI batteryText;

    private List<Interactable> interactables = new();
    public string emptyBatteryText = "EMPTY BATTERY";
    public string insertBatteryText = "INSERT BATTERY";

    private void Awake()
    {
        instance = this;
    }

    public void ShowText(Interactable interactable)
    {
        interactables.Add(interactable);
        interactionText.gameObject.SetActive(true);
        interactionText.SetText(interactable.textToShow);
    }

    public void HideText(Interactable interactable)
    {
        interactables.Remove(interactable);
        if (interactables.Count == 0)
        {
            interactionText.gameObject.SetActive(false);
        }
        else
        {
            var first = interactables[0];
            interactables.RemoveAt(0);
            interactionText.SetText(first.textToShow);
        }
    }

    public void EmptyBattery()
    {
        batteryText.gameObject.SetActive(true);
        batteryText.SetText(emptyBatteryText);
    }

    public void InsertBattery()
    {
        batteryText.gameObject.SetActive(true);
        batteryText.SetText(insertBatteryText);
    }

    public void HideBattery()
    {
        batteryText.gameObject.SetActive(false);
    }
}