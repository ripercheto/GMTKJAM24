using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    [BoxGroup("Interaction")]
    public float interactionDelay = 0.5f;
    [BoxGroup("Interaction")]
    public bool needsInput = true;
    protected abstract bool CanInteract { get; }

    private bool pressedInteract;
    private float interactionTime;

    private void Update()
    {
        pressedInteract = PlayerInput.PressingAction;
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
    }

    protected abstract void HandleInteraction(Player player);
}