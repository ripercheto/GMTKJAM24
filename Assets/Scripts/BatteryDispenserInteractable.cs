using UnityEngine;

public class BatteryDispenserInteractable : Interactable
{
    public int maxCharges;
    public float rechargeDuration;
    public GameObject batteryPrefab;

    private int currentCharges;
    private float nextRechargeTime;

    private void Awake()
    {
        currentCharges = maxCharges;
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
        var battery = Instantiate(batteryPrefab);
        player.GiveBattery(battery);
        AddCharge(-1);
    }

    private void AddCharge(int amount)
    {
        currentCharges += amount;
        nextRechargeTime = Time.time + rechargeDuration;
    }
}