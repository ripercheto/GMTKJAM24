using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class RobotAI : MonoBehaviour
{
    [Serializable, InlineProperty]
    public class MinMaxFloat
    {
        [HorizontalGroup]
        public float min, max;
        public float GetRandom() => Random.Range(min, max);
    }

    [Serializable]
    public class AIFlow
    {
        [BoxGroup("Enemy Power Check")]
        [LabelText("Target Type")]
        public Robot.PowerDirectionType enemyPowerTarget;

        [BoxGroup("Chose Target")]
        [LabelText("Delay")]
        public MinMaxFloat enemyPowerOnTargetSideDelay;
        [BoxGroup("Chose Target")]
        [LabelText("Chance To Laser")]
        public float targetSideChanceToLaser;
        [BoxGroup("Chose Target")]
        [LabelText("Chance To Action")]
        public float targetSideChanceToAction;

        [BoxGroup("Chose Other")]
        [LabelText("Delay")]
        public MinMaxFloat enemyPowerOnOtherSideDelay;
        [BoxGroup("Chose Other/Stance")]
        [LabelText("Chance")]
        public float otherSideChanceToPrepare;
        [BoxGroup("Chose Other/Stance")]
        [LabelText("Delay")]
        public MinMaxFloat otherSideChanceToPrepareSwitchPowerDelay;
        [BoxGroup("Chose Other/Switch Power")]
        [LabelText("Chance")]
        public float otherSideChanceToSwitchPower;
        [BoxGroup("Chose Other/Switch Power")]
        [LabelText("Delay")]
        public MinMaxFloat otherSideChanceToSwitchPowerDelay;
        [BoxGroup("Chose Other/Laser")]
        [LabelText("Chance")]
        public float otherSideChanceToLaser;

        private RobotAI ai;
        private RobotAction action;

        public void Initialize(RobotAI ai, RobotAction action)
        {
            this.ai = ai;
            this.action = action;
        }

        public IEnumerator Check()
        {
            ai.robot.ReceiveBattery();

            Debug.Log($"Rolled to use {enemyPowerTarget}. Check if AI has power");
            if (ai.robot.powerDirectionType == enemyPowerTarget)
            {
                var delay = enemyPowerOnTargetSideDelay.GetRandom();
                Debug.Log($"{enemyPowerTarget} Flow has power. Waiting for {delay} seconds");
                yield return new WaitForSeconds(delay);
                var shouldLaser = RandomWithWeights(targetSideChanceToLaser, targetSideChanceToAction) == 0;
                if (shouldLaser)
                {
                    Debug.Log($"{enemyPowerTarget} Flow rolled to use laser");
                    ai.robot.UseLaser();
                }
                else
                {
                    Debug.Log($"{enemyPowerTarget} Flow rolled to prepare {action.direction} action.");
                    action.StartPrepare();
                }
            }
            else
            {
                var delay = enemyPowerOnOtherSideDelay.GetRandom();
                Debug.Log($"{enemyPowerTarget} Flow does not have power. Waiting for {delay} seconds");
                yield return new WaitForSeconds(delay);
                var choice = RandomWithWeights(otherSideChanceToPrepare, otherSideChanceToSwitchPower, otherSideChanceToLaser);
                switch (choice)
                {
                    case 0:
                    {
                        delay = otherSideChanceToPrepareSwitchPowerDelay.GetRandom();
                        Debug.Log($"{enemyPowerTarget} Flow rolled to start preparing  action. Waiting for {delay} seconds to change power");
                        action.StartPrepare();
                        yield return new WaitForSeconds(delay);
                        Debug.Log($"{enemyPowerTarget} Flow changing power");
                        ai.robot.CyclePowerDirection();
                    }
                        break;
                    case 1:
                    {
                        delay = otherSideChanceToSwitchPowerDelay.GetRandom();
                        ai.robot.CyclePowerDirection();
                        yield return new WaitForSeconds(delay);
                        Debug.Log($"{enemyPowerTarget} Flow rolled to start preparing {action.direction}");
                        action.StartPrepare();
                    }
                        break;
                    case 2:
                        Debug.Log($"{enemyPowerTarget} Flow rolled to shoot laser");
                        ai.robot.UseLaser();
                        break;
                }
            }
        }
    }

    public Robot robot;
    public AIFlow shieldFlow, punchFlow;

    public float playerLeftChanceToLeft, playerLeftChanceToRight, playerRightChanceToLeft, playerRightChanceToRight;

    private Robot OpponentRobot => robot.target;

    public IEnumerator Start()
    {
        shieldFlow.Initialize(this, robot.shieldAction);
        punchFlow.Initialize(this, robot.punchAction);

        while (true)
        {
            Debug.Log($"Player has power to {OpponentRobot.powerDirectionType}, proceeding");
            if (OpponentRobot.powerDirectionType == Robot.PowerDirectionType.Left)
            {
                var flowToUse = RandomWithWeights(playerLeftChanceToLeft, playerLeftChanceToRight) == 0 ? shieldFlow : punchFlow;
                yield return flowToUse.Check();
            }
            else if (OpponentRobot.powerDirectionType == Robot.PowerDirectionType.Right)
            {
                var flowToUse = RandomWithWeights(playerRightChanceToLeft, playerRightChanceToRight) == 0 ? shieldFlow : punchFlow;
                yield return flowToUse.Check();
            }
            Debug.Log($"AI LOOP END");
        }
    }

    public static int RandomWithWeights(params float[] weights)
    {
        var totalWeight = 0f;
        foreach (var weight in weights)
        {
            totalWeight += weight;
        }

        var randomValue = Random.Range(0, totalWeight);

        for (var i = 0; i < weights.Length; i++)
        {
            if (randomValue < weights[i])
                return i;

            randomValue -= weights[i];
        }

        return -1; // This line should never be reached if weights are properly set
    }
}