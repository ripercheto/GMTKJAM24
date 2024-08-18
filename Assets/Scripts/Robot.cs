using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public enum PowerDirectionType
    {
        Left,
        Right
    }

    public int health = 4;
    public int chargePerBattery = 4;
    public Robot target;
    [BoxGroup("Laser")]
    public int laserCost = 4;
    [BoxGroup("Laser")]
    public float laserCooldown = 5;
    [BoxGroup("Laser")]
    public Transform laserEffectSocket;
    [BoxGroup("Laser")]
    public GameObject laserEffectPrefab;
    [BoxGroup("Laser")]
    public float laserEffectDestroyDelay = 2;
    [BoxGroup("Laser")]
    public ParticleSystem[] laserCooldownEffect;
    public AnimatorHandler shieldAnimatorHandler;
    public RobotAction shieldAction;
    public RobotAction punchAction;

    [ReadOnly]
    public int charges;
    [ReadOnly]
    public PowerDirectionType powerDirectionType;

    public bool CanUseLaser
    {
        get
        {
            if (charges < laserCost)
            {
                return false;
            }

            if (Time.time < laserUseTime)
            {
                return false;
            }

            return true;
        }
    }

    private float laserUseTime;

    private void Awake()
    {
        shieldAction.Initialize(this, HandleShieldActive, HandleShieldIdle);
        punchAction.Initialize(this, HandleOnActivatePunch);
    }

    private void HandleShieldActive()
    {
        shieldAnimatorHandler.SetBool(true);
    }

    private void HandleShieldIdle()
    {
        shieldAnimatorHandler.SetBool(false);
    }

    public void ReceiveBattery()
    {
        SetCharges(chargePerBattery);
        shieldAction.TryActivateState();
        punchAction.TryActivateState();
    }

    public virtual void ReceivePunch()
    {
        if (shieldAction.State == RobotActionStateType.Active)
        {
            HandleOnBlockPunch();
            shieldAction.TryCancelActiveState();
        }
        else
        {
            TakeDamage();
        }
    }

    public virtual void ReceiveLaser()
    {
        TakeDamage();
    }

    public void CyclePowerDirection()
    {
        powerDirectionType = Cycle(powerDirectionType);
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

    public void TryUseLaser()
    {
        if (!CanUseLaser)
        {
            return;
        }

        SetCharges(charges - laserCost);
        laserUseTime = Time.time + laserCooldown;
        target.ReceiveLaser();

        StartCoroutine(CooldownEffect());
        StartCoroutine(DestroyLaser());

        IEnumerator CooldownEffect()
        {
            foreach (var ps in laserCooldownEffect)
            {
                ps.Play();
            }
            yield return new WaitForSeconds(laserCooldown);
            foreach (var ps in laserCooldownEffect)
            {
                ps.Stop();
            }
        }

        IEnumerator DestroyLaser()
        {
            var rotation = Quaternion.LookRotation(target.laserEffectSocket.position - laserEffectSocket.position);
            var laser = Instantiate(laserEffectPrefab, laserEffectSocket.position, rotation);
            yield return new WaitForSeconds(laserEffectDestroyDelay);
            Destroy(laser.gameObject);
        }
    }

    protected virtual void HandleOnActivatePunch()
    {
        target.ReceivePunch();
    }

    protected virtual void HandleOnBlockPunch()
    {
        //TODO BREAK SHIELD
    }

    public virtual void SetCharges(int newCharges)
    {
        charges = newCharges;
    }

    private void TakeDamage()
    {
        health--;
        shieldAction.ForceCancel();
        punchAction.ForceCancel();
        Debug.Log($"{name} took damage. Health: {health}");
    }

    public static PowerDirectionType Cycle(PowerDirectionType type)
    {
        return (PowerDirectionType)(((int)type + 1) % Enum.GetValues(typeof(PowerDirectionType)).Length);
    }
}