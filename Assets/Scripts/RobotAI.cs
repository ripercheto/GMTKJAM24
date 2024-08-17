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
            if (ai.playerRobot.powerDirectionType != playerPowerCheck)
            {
                yield break;
            }

            var enemyPowerDirectionToCheck = RandomWithWeights(chanceToCheckTarget, chanceToCheckOther) == 0 ? enemyPowerTarget : Robot.Cycle(enemyPowerTarget);
            if (ai.robot.powerDirectionType == enemyPowerDirectionToCheck)
            {
                Debug.Log("");
                yield return new WaitForSeconds(enemyPowerOnTargetSideDelay.GetRandom());
                var shouldLaser = RandomWithWeights(targetSideChanceToLaser, targetSideChanceToAction) == 0;
                if (shouldLaser)
                {
                    ai.robot.PrepareLaser();
                }
                else
                {
                    action.StartPrepare();
                }
            }
            else
            {
                var delay = enemyPowerOnOtherSideDelay.GetRandom();
                yield return new WaitForSeconds(delay);
                var choice = RandomWithWeights(otherSideChanceToPrepare, otherSideChanceToSwitchPower, otherSideChanceToLaser);
                switch (choice)
                {
                    case 0:
                        action.StartPrepare();
                        yield return new WaitForSeconds(otherSideChanceToPrepareSwitchPowerDelay.GetRandom());
                        ai.robot.CyclePowerDirection();
                        break;
                    case 1:
                        ai.robot.CyclePowerDirection();
                        yield return new WaitForSeconds(otherSideChanceToSwitchPowerDelay.GetRandom());
                        action.StartPrepare();
                        break;
                    case 2:
                        ai.robot.PrepareLaser();
                        break;
                }
            }
        }
    }

    public AIFlow shieldFlow, punchFlow;

    public Robot playerRobot;
    public Robot robot;

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