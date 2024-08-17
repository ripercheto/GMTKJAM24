using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RobotInteractable : Interactable
{
    public PlayerRobot robot;

    protected virtual void OnValidate()
    {
        robot = FindComponentInParent<PlayerRobot>(transform);
    }

    public static T FindComponentInParent<T>(Transform child) where T : Component
    {
        Transform current = child;

        while (current != null)
        {
            T component = current.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            current = current.parent;
        }

        return null;
    }
}