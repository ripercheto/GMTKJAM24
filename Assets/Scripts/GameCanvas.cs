using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas instance;
    public TextMeshProUGUI interactionText;

    private Interactable interactableTriggered;

    private void Awake()
    {
        instance = this;
    }

    public void ShowText(Interactable interactable)
    {
        interactableTriggered = interactable;
        interactionText.gameObject.SetActive(true);
        interactionText.SetText(interactableTriggered.textToShow);
    }

    public void HideText(Interactable interactable)
    {
        if (interactableTriggered != interactable)
        {
            return;
        }
        interactionText.gameObject.SetActive(false);
    }
}