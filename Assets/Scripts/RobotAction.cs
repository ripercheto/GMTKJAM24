using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public enum RobotActionStateType
{
    Idle,
    Preparing,
    Prepared,
    Active
}

[Serializable]
public class RobotAction
{
    public int cost = 1;
    public float prepareTime = 2;
    public float activeTime = 2;
    public Robot.PowerDirectionType direction = Robot.PowerDirectionType.Left;

    [ShowInInspector, ReadOnly]
    public RobotActionStateType State { get; private set; }

    private Robot robot;
    private Action activeAction;
    private Action idleAction;
    private Action prepareAction;
    private Coroutine prepareCoroutine;
    private Coroutine activeCoroutine;

    public void Initialize(Robot parent, Action actionOnActive, Action actionOnIdle, Action actionOnPrepare)
    {
        robot = parent;
        activeAction = actionOnActive;
        idleAction = actionOnIdle;
        prepareAction = actionOnPrepare;
    }

    public void StartPrepare()
    {
        prepareAction?.Invoke();
        State = RobotActionStateType.Preparing;
        prepareCoroutine = robot.StartCoroutine(HandlePreparing());

        IEnumerator HandlePreparing()
        {
            yield return new WaitForSeconds(prepareTime);
            State = RobotActionStateType.Prepared;
            TryActivateState();
            prepareCoroutine = null;
        }
    }

    public void TryActivateState()
    {
        if (robot.charges < cost)
        {
            return;
        }
        if (State != RobotActionStateType.Prepared)
        {
            return;
        }
        if (robot.powerDirectionType != direction)
        {
            return;
        }

        State = RobotActionStateType.Active;
        robot.SetCharges(robot.charges - cost);
        activeAction?.Invoke();

        activeCoroutine = robot.StartCoroutine(HandleCooldown());

        IEnumerator HandleCooldown()
        {
            yield return new WaitForSeconds(activeTime);
            State = RobotActionStateType.Idle;
            idleAction?.Invoke();
            activeCoroutine = null;
        }
    }

    public void TryCancelActiveState()
    {
        if (robot.powerDirectionType == direction)
        {
            return;
        }

        if (State != RobotActionStateType.Active)
        {
            return;
        }

        State = RobotActionStateType.Idle;
        idleAction?.Invoke();
        if (activeCoroutine == null)
        {
            return;
        }
        robot.StopCoroutine(activeCoroutine);
    }

    public void ForceCancel()
    {
        if (prepareCoroutine != null)
        {
            robot.StopCoroutine(prepareCoroutine);
        }
        if (activeCoroutine != null)
        {
            robot.StopCoroutine(activeCoroutine);
        }
        State = RobotActionStateType.Idle;
        idleAction?.Invoke();
    }
}