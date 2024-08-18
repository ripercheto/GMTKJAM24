using System;
using System.Collections;
using Sirenix.OdinInspector;
using SmallHedge.SoundManager;
using UnityEngine;
using UnityEngine.Rendering;

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

    [BoxGroup("Animation")]
    public AnimatorHandler damageTrigger;
    [BoxGroup("Animation")]
    public AnimatorHandler punchInt;
    [BoxGroup("Animation")]
    public AnimatorHandler shieldInt;
    
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

    [BoxGroup("Shield")]
    public AnimatorHandler shieldAnimatorHandler;
    [BoxGroup("Shield")]
    public RobotAction shieldAction;

    [BoxGroup("Punch")]
    public RobotAction punchAction;

    public Material activePowerMaterial, noPowerMaterial;
    public MeshRenderer[] chargesIndicator;

    [BoxGroup("Power")]
    public Material playerMaterial;
    [BoxGroup("Power")]
    public Material leftMaterial, rightMaterial, middlePowerMaterial;
    [BoxGroup("Power"), ColorUsage(false, true)]
    public Color powerActiveColor, powerInactiveColor, powerWrongSideColor;

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
    private static readonly int Color1 = Shader.PropertyToID("_EmissionColor");
    private LocalKeyword playerEmissionKeyword;

    private void Awake()
    {
        playerEmissionKeyword = new LocalKeyword(playerMaterial.shader, "_EMISSION");
        shieldAction.Initialize(this, HandleShieldActive, HandleShieldIdle);
        punchAction.Initialize(this, HandleOnActivatePunch, HandlePunchIdle);
        SetCharges(0);
    }

    private void HandlePunchIdle()
    {
        punchInt.SetInt(0);
    }

    private void HandleShieldActive()
    {
        SoundManager.PlaySound(SoundType.ShieldActivating);
        shieldAnimatorHandler.SetBool(true);
        shieldInt.SetInt(2);
    }

    private void HandleShieldIdle()
    {
        SoundManager.PlaySound(SoundType.ShieldDeactivating);
        shieldAnimatorHandler.SetBool(false);
        shieldInt.SetInt(0);
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
            SoundManager.PlaySound(SoundType.PunchHittingRobot);
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
        UpdatePowerMaterial();
        shieldAction.TryActivateState();
        shieldAction.TryCancelActiveState();
        punchAction.TryActivateState();
    }

    public void PreparePunch()
    {
        SoundManager.PlaySound(SoundType.PunchStance);
        punchAction.StartPrepare();
        punchInt.SetInt(1);
    }

    public void PrepareShield()
    {
        SoundManager.PlaySound(SoundType.ShieldStance);
        shieldAction.StartPrepare();
        shieldInt.SetInt(1);
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
        SoundManager.PlaySound(SoundType.FiringLaser);

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
        punchInt.SetInt(2);
        SoundManager.PlaySound(SoundType.FistBeingLaunched);
        target.ReceivePunch();
    }

    protected virtual void HandleOnBlockPunch()
    {
        SoundManager.PlaySound(SoundType.PunchHittingShield);
        shieldAction.ForceCancel();
    }

    public virtual void SetCharges(int newCharges)
    {
        charges = newCharges;
        for (var i = 0; i < chargesIndicator.Length; i++)
        {
            var active = i < charges;
            chargesIndicator[i].material = active ? activePowerMaterial : noPowerMaterial;
        }
        UpdatePowerMaterial();
    }

    [Button]
    private void TakeDamage()
    {
        health--;
        shieldAction.ForceCancel();
        punchAction.ForceCancel();
        Debug.Log($"{name} took damage. Health: {health}");

        damageTrigger.SetTrigger();
        StartCoroutine(Blink());

        IEnumerator Blink()
        {
            playerMaterial.SetKeyword(playerEmissionKeyword, true);
            yield return new WaitForSeconds(0.1f);
            playerMaterial.SetKeyword(playerEmissionKeyword, false);
        }
    }

    private void UpdatePowerMaterial()
    {
        if (middlePowerMaterial == null)
        {
            return;
        }
        if (leftMaterial == null)
        {
            return;
        }
        if (rightMaterial == null)
        {
            return;
        }
        middlePowerMaterial.SetColor(Color1, charges < laserCost ? powerInactiveColor : powerActiveColor);
        switch (powerDirectionType)
        {
            case PowerDirectionType.Left:
                leftMaterial.SetColor(Color1, charges < shieldAction.cost ? powerInactiveColor : powerActiveColor);
                rightMaterial.SetColor(Color1, powerWrongSideColor);
                break;
            case PowerDirectionType.Right:
                rightMaterial.SetColor(Color1, charges < punchAction.cost ? powerInactiveColor : powerActiveColor);
                leftMaterial.SetColor(Color1, powerWrongSideColor);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static PowerDirectionType Cycle(PowerDirectionType type)
    {
        return (PowerDirectionType)(((int)type + 1) % Enum.GetValues(typeof(PowerDirectionType)).Length);
    }
}