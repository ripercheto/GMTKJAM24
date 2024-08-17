using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [Serializable]
    public class RobotAction
    {
        public int cost = 1;
        public float prepareTime = 2;
        public float activeTime = 2;
        public PowerDirectionType direction = PowerDirectionType.Left;

        [ShowInInspector, ReadOnly]
        public RobotActionStateType State { get; private set; }

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
            State = RobotActionStateType.Preparing;
            robot.StartCoroutine(HandlePreparing());

            IEnumerator HandlePreparing()
            {
                yield return new WaitForSeconds(prepareTime);
                State = RobotActionStateType.Prepared;
                TryActivateState();
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
            robot.charges -= cost;
            activeAction?.Invoke();

            activeCoroutine = robot.StartCoroutine(HandleCooldown());

            IEnumerator HandleCooldown()
            {
                yield return new WaitForSeconds(activeTime);
                State = RobotActionStateType.Idle;
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
            if (activeCoroutine == null)
            {
                return;
            }
            robot.StopCoroutine(activeCoroutine);
        }
    }

    public enum PowerDirectionType
    {
        Left,
        Right,
    }

    public enum RobotActionStateType
    {
        Idle,
        Preparing,
        Prepared,
        Active,
    }

    public int chargePerBattery = 4;

    public Player player;
    public WalkablePolygon leftArmArea, rightArmArea, leftRespawnArea, rightRespawnArea;

    public RobotAction shieldAction;
    public RobotAction punchAction;

    [ShowInInspector, ReadOnly]
    private int charges;
    [ShowInInspector, ReadOnly]
    private PowerDirectionType powerDirectionType;

    private void Awake()
    {
        shieldAction.Initialize(this, null);
        punchAction.Initialize(this, HandleOnPlayerPunch);
    }

    public void ReceiveBattery()
    {
        charges = chargePerBattery;
        shieldAction.TryActivateState();
        punchAction.TryActivateState();
    }

    [Button]
    public void ReceivePunch()
    {
        if (shieldAction.State != RobotActionStateType.Active)
        {
            return;
        }
        HandleOnBlockPunch();
        shieldAction.TryCancelActiveState();
    }

    public void CyclePowerDirection()
    {
        powerDirectionType = (PowerDirectionType)(((int)powerDirectionType + 1) % Enum.GetValues(typeof(PowerDirectionType)).Length);
        shieldAction.TryActivateState();
        shieldAction.TryCancelActiveState();
        punchAction.TryActivateState();
    }

    public void PreparePunch()
    {
        punchAction.StartPrepare();
    }

    public void PrepareShield()
    {
        shieldAction.StartPrepare();
    }

    private void HandleOnPlayerPunch()
    {
        if (player.isKinematic)
        {
            return;
        }
        if (player.walkableArea != rightArmArea)
        {
            return;
        }
        player.MakeFallOff(rightRespawnArea);
    }

    private void HandleOnBlockPunch()
    {
        if (player.isKinematic)
        {
            return;
        }
        if (player.walkableArea != leftArmArea)
        {
            return;
        }
        player.MakeFallOff(leftRespawnArea);
    }
}