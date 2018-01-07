﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFieldAbility : PlayerAbility
{//2017-11-17: copied from ShieldBubbleAbility

    public GameObject electricFieldPrefab;//prefab
    public float maxRange = 2.5f;
    public float maxEnergy = 100;
    public float maxHoldTime = 1;//how long until the max range is reached after it begins charging
    public float maxSlowPercent = 0.10f;//the max percent of slowness applied to objects near the center of the field when the field has maxRange
    public float lastDisruptTime = 0;//the last time that something happened that disrupted the shield
    public float baseActivationDelay = 2.0f;//how long after the last disruption the field can start regenerating
    public float maxForceResistance = 500f;//if it gets this much force, it takes out the field, but it will come right back up

    private GameObject currentElectricField;
    private ElectricFieldController cEFController;//"current Electric Field Controller"
    
    private float activationDelay = 2.0f;//how long it will wait, usually set to the base delay

    public AudioClip shieldBubbleSound;
    private PlayerController playerController;//for if this script is on Merky

    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        if (playerController)
        {
            playerController.onTeleport += processTeleport;
        }
        lastDisruptTime = Time.time;
    }

    void Update()
    {
        if (Time.time > lastDisruptTime + activationDelay)
        {
            processWaitGesture(Time.time - (lastDisruptTime + activationDelay));
        }
    }    

    public void processWaitGesture(float waitTime)
    {
        if (currentElectricField == null)
        {
            currentElectricField = Instantiate(electricFieldPrefab);
            cEFController = currentElectricField.GetComponent<ElectricFieldController>();
            cEFController.energyToRangeRatio = maxRange / maxEnergy;
            cEFController.energyToSlowRatio = maxSlowPercent / maxEnergy;
            cEFController.maxForceResistance = maxForceResistance;
        }
        currentElectricField.transform.position = transform.position;
        float energyToAdd = Time.deltaTime * maxEnergy / maxHoldTime;
        cEFController.addEnergy(energyToAdd);
        if (cEFController.energy > maxEnergy)
        {
            cEFController.energy = maxEnergy;
        }

        if (playerController)
        {
            if (playerController.range < cEFController.range + 0.5f)
            {
                playerController.setRange(cEFController.range + 0.5f);
            }
        }
    }

    public void dropWaitGesture()
    {
        lastDisruptTime = Time.time;

        currentElectricField = null;
        cEFController = null;
    }

    public void processTeleport(Vector2 oldPos, Vector2 newPos)
    {
        dropWaitGesture();
        float distance = Vector3.Distance(oldPos, newPos);
        activationDelay = baseActivationDelay * distance / playerController.baseRange;
    }

    public void checkForce(float force)
    {
        float addedDelay = maxHoldTime * force / maxForceResistance;
        lastDisruptTime = Time.time + addedDelay - maxHoldTime;
        if (force >= maxForceResistance)
        {
            dropWaitGesture();
        }
        Debug.Log("Force: " + force + ", addedDelay: " + addedDelay);
    }
}