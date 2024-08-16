using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInput
{
    public static Vector2 Directional
    {
        get
        {
            Vector2 playerInput;
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);
            return playerInput;
        }
    }

    public static bool PressedAction => Input.GetButtonDown("Fire1");
}