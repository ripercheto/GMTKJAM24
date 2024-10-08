using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public enum InputType
    {
        Interactable,
        Axis
    }

    [BoxGroup("Interaction")]
    public AnimatorHandler animatorHandler;
    [BoxGroup("Interaction")]
    public float interactionDelay = 0.5f;
    [BoxGroup("Interaction")]
    public bool needsInput = true;
    [BoxGroup("Interaction")]
    [ShowIf(nameof(needsInput))]
    public string textToShow;
    [BoxGroup("Interaction")]
    [ShowIf(nameof(needsInput))]
    public InputType inputType;
    [BoxGroup("Interaction")]
    [ShowIf(nameof(inputType), InputType.Axis)]
    public Vector2 axisInputNeeded;
    private float interactionTime;

    private bool pressedInteract;
    protected abstract bool CanInteract { get; }

    protected virtual void Update()
    {
        switch (inputType)
        {
            case InputType.Interactable:
                pressedInteract = PlayerInput.PressingAction;
                break;
            case InputType.Axis:
                pressedInteract = Vector2.Dot(PlayerInput.Directional, axisInputNeeded) > 0.5f;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (needsInput && inputType == InputType.Interactable)
        {
            GameCanvas.instance.ShowText(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (needsInput && inputType == InputType.Interactable)
        {
            GameCanvas.instance.HideText(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (needsInput && !pressedInteract)
        {
            return;
        }
        if (Time.time < interactionTime)
        {
            return;
        }
        if (!CanInteract)
        {
            return;
        }
        interactionTime = Time.time + interactionDelay;
        var player = other.GetComponent<Player>();
        HandleInteraction(player);
        if (animatorHandler.HasAnimator)
        {
            animatorHandler.SetTrigger();
        }
    }

    protected abstract void HandleInteraction(Player player);
}