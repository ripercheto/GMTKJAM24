using SmallHedge.SoundManager;
using UnityEngine;

public class BatteryDispenserInteractable : Interactable
{
    public Transform socket;
    public int maxCharges;
    public float rechargeDuration;
    public GameObject batteryPrefab;

    private int currentCharges;
    private float nextRechargeTime;

    private GameObject battery;
    
    private void Awake()
    {
        currentCharges = maxCharges;
        battery = Instantiate(batteryPrefab, socket);
    }

    protected override bool CanInteract
    {
        get
        {
            if (currentCharges == 0)
            {
                return false;
            }

            return true;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (currentCharges >= maxCharges)
        {
            return;
        }
        if (Time.time < nextRechargeTime)
        {
            return;
        }
        AddCharge(1);
        battery = Instantiate(batteryPrefab, socket);
        SoundManager.PlaySound(SoundType.BatteryAppearing);
    }

    protected override void HandleInteraction(Player player)
    {
        SpawnBatteryOnPlayer(player);
    }

    private void SpawnBatteryOnPlayer(Player player)
    {
        if (player.HasBattery)
        {
            return;
        }
        SoundManager.PlaySound(SoundType.GrabbingBattery);
        player.GiveBattery(battery);
        battery = null;
        AddCharge(-1);
    }

    private void AddCharge(int amount)
    {
        currentCharges += amount;
        nextRechargeTime = Time.time + rechargeDuration;
    }
}