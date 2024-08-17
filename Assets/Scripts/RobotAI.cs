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
        [BoxGroup("Player Power Check")]
        [LabelText("Type")]
        public Robot.PowerDirectionType playerPowerCheck;

        [BoxGroup("Enemy Power Check")]
        [LabelText("Target Type")]
        public Robot.PowerDirectionType enemyPowerTarget;
        [BoxGroup("Enemy Power Check")]
        public float chanceToCheckTarget;
        [BoxGroup("Enemy Power Check")]
        public float chanceToCheckOther;

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
            if (ai.OpponentRobot.powerDirectionType != playerPowerCheck)
            {
                yield break;
            }

            var enemyPowerDirectionToCheck = RandomWithWeights(chanceToCheckTarget, chanceToCheckOther) == 0 ? enemyPowerTarget : Robot.Cycle(enemyPowerTarget);
            if (ai.robot.powerDirectionType == enemyPowerDirectionToCheck)
            {
                var delay = enemyPowerOnTargetSideDelay.GetRandom();
                Debug.Log($"{playerPowerCheck} Flow rolled to check if enemy has power to the {enemyPowerTarget}. Waiting for {delay} seconds");
                yield return new WaitForSeconds(delay);
                var shouldLaser = RandomWithWeights(targetSideChanceToLaser, targetSideChanceToAction) == 0;
                if (shouldLaser)
                {
                    Debug.Log($"{playerPowerCheck} Flow rolled to prepare laser");
                    ai.robot.PrepareLaser();
                }
                else
                {
                    Debug.Log($"{playerPowerCheck} Flow rolled to prepare {enemyPowerTarget} action.");
                    action.StartPrepare();
                }
            }
            else
            {
                var delay = enemyPowerOnOtherSideDelay.GetRandom();
                Debug.Log($"{playerPowerCheck} Flow rolled to check if enemy has power to the {Robot.Cycle(enemyPowerTarget)}. Waiting for {delay} seconds");
                yield return new WaitForSeconds(delay);
                var choice = RandomWithWeights(otherSideChanceToPrepare, otherSideChanceToSwitchPower, otherSideChanceToLaser);
                switch (choice)
                {
                    case 0:
                    {
                        delay = otherSideChanceToPrepareSwitchPowerDelay.GetRandom();
                        Debug.Log($"{playerPowerCheck} Flow on {Robot.Cycle(enemyPowerTarget)} rolled to start preparing  action. Waiting for {delay} seconds to change power");
                        action.StartPrepare();
                        yield return new WaitForSeconds(delay);
                        Debug.Log($"{playerPowerCheck} Flow on {Robot.Cycle(enemyPowerTarget)} changing power");
                        ai.robot.CyclePowerDirection();
                    }
                        break;
                    case 1:
                    {
                        delay = otherSideChanceToSwitchPowerDelay.GetRandom();
                        ai.robot.CyclePowerDirection();
                        yield return new WaitForSeconds(delay);
                        Debug.Log($"{playerPowerCheck} Flow on {Robot.Cycle(enemyPowerTarget)} start preparing");
                        action.StartPrepare();
                    }
                        break;
                    case 2:
                        Debug.Log($"{playerPowerCheck} Flow on {Robot.Cycle(enemyPowerTarget)} rolled to shoot laser");
                        ai.robot.PrepareLaser();
                        break;
                }
            }
        }
    }

    public Robot robot;
    public AIFlow shieldFlow, punchFlow;

    private Robot OpponentRobot => robot.target;

    public IEnumerator Start()
    {
        shieldFlow.Initialize(this, robot.shieldAction);
        punchFlow.Initialize(this, robot.punchAction);

        while (true)
        {
            yield return shieldFlow.Check();
            yield return punchFlow.Check();
            yield return new WaitForSeconds(1f);
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