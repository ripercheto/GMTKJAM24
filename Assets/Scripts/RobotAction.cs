using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class RobotAction
{
    public int cost = 1;
    public float prepareTime = 2;
    public float activeTime = 2;
    public Robot.PowerDirectionType direction = Robot.PowerDirectionType.Left;

    [ShowInInspector, ReadOnly]
    public Robot.RobotActionStateType State { get; private set; }

    private Robot robot;
    private Action activeAction;
    private Coroutine activeCoroutine;

    public void Initialize(Robot parent, Action actionOnActive)
    {
        robot = parent;
        activeAction = actionOnActive;
    }

    public void StartPrepare()
    {
        State = Robot.RobotActionStateType.Preparing;
        robot.StartCoroutine(HandlePreparing());

        IEnumerator HandlePreparing()
        {
            yield return new WaitForSeconds(prepareTime);
            State = Robot.RobotActionStateType.Prepared;
            TryActivateState();
        }
    }

    public void TryActivateState()
    {
        if (robot.charges < cost)
        {
            return;
        }
        if (State != Robot.RobotActionStateType.Prepared)
        {
            return;
        }
        if (robot.powerDirectionType != direction)
        {
            return;
        }

        State = Robot.RobotActionStateType.Active;
        robot.charges -= cost;
        activeAction?.Invoke();

        activeCoroutine = robot.StartCoroutine(HandleCooldown());

        IEnumerator HandleCooldown()
        {
            yield return new WaitForSeconds(activeTime);
            State = Robot.RobotActionStateType.Idle;
            activeCoroutine = null;
        }
    }

    public void TryCancelActiveState()
    {
        if (robot.powerDirectionType == direction)
        {
            return;
        }

        if (State != Robot.RobotActionStateType.Active)
        {
            return;
        }

        State = Robot.RobotActionStateType.Idle;
        if (activeCoroutine == null)
        {
            return;
        }
        robot.StopCoroutine(activeCoroutine);
    }
}