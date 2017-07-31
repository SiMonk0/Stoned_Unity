﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

    //Effects
    [Header("Collision Effect")]
    public GameObject collisionEffectPrefab;//the object that holds the special effect for collision
    public float particleStartSpeed = 7.0f;
    public float particleAmount = 50.0f;
    [Header("Tap Target Highlighting")]
    public ParticleSystem tapTargetHighlight;
    //Supporting Lists
    private List<ParticleSystem> collisionEffectList = new List<ParticleSystem>();

    private static EffectManager instance;

	// Use this for initialization
	void Start () {
		if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
	}

    /// <summary>
    /// Shows sparks coming from the point of collision
    /// </summary>
    /// <param name="position">Position of collision</param>
    /// <param name="damagePercent">How much percent of total HP of damage was inflicted, between 0 and 100</param>
    public static void collisionEffect(Vector2 position, float damagePercent = 100.0f)
    {
        ParticleSystem chosenPS = null;
        //Find existing particle system
        foreach (ParticleSystem ps in instance.collisionEffectList)
        {
            if (!ps.isPlaying)
            {
                chosenPS = ps;
            }
        }
        //Else make a new one
        if (chosenPS == null)
        {
            GameObject ce = GameObject.Instantiate(instance.collisionEffectPrefab);
            ParticleSystem ceps = ce.GetComponent<ParticleSystem>();
            instance.collisionEffectList.Add(ceps);
            chosenPS = ceps;
        }
        //Start Speed
        {
            ParticleSystem.MainModule psmm = chosenPS.main;
            ParticleSystem.MinMaxCurve psmmc = psmm.startSpeed;
            float speed = (damagePercent * instance.particleStartSpeed) / 100;
            speed = Mathf.Max(speed, 0.5f);//make speed at least 1.0f
            psmmc.constant = speed;
            psmm.startSpeed = psmmc;
        }
        //Particle Amount
        {
            int amountOfParticles = (int)((damagePercent * instance.particleAmount) / 100);
            amountOfParticles = Mathf.Max(amountOfParticles, 3);//make speed at least 1.0f
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1]
            {
                new ParticleSystem.Burst(0, (short)amountOfParticles)
            };
            chosenPS.emission.SetBursts(bursts);
        }
        //
        chosenPS.gameObject.transform.position = position;
        chosenPS.Play();
    }

    public static void highlightTapArea(Vector2 pos, bool play = true)
    {
        if (play)
        {
            instance.tapTargetHighlight.transform.position = pos;
            instance.tapTargetHighlight.Play();
        }
        else
        {
            instance.tapTargetHighlight.Stop();
        }
    }
}
