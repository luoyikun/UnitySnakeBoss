using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsController : MonoBehaviour
{
    [Header("Spaceship Settings")]
    public SpaceshipData data;

    public Transform trailParent;
    private TrailRenderer[] trails;
    public Renderer spaceshipRenderer;

    private void Start()
    {
        trails = trailParent.GetComponentsInChildren<TrailRenderer>();
    }

    void Update()
    {
        ModifyTrail();
        ModifyBoostEmission();
    }

    private void ModifyBoostEmission()
    {
        //throw new NotImplementedException();
    }

    void ModifyTrail()
    {
        foreach (TrailRenderer tr in trails)
        {
            tr.time = (data.thrustInput) * .35f;
        }
    }

}
