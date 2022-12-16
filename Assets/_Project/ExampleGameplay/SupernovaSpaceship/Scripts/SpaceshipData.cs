using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship Data", menuName = "Scriptable Objects/Spaceship Data", order = 1)]
public class SpaceshipData : ScriptableObject
{

    [Header("Movement")]
    public float thrustAmount;
    [HideInInspector]public float thrustInput;

    public float yawSpeed;
    public float pitchSpeed;
    [HideInInspector] public Vector3 steeringInput;

    public float leanAmount_X;
    public float leanAmount_Y;

    [Header("Shooting")]
    public float shootRate;
    [HideInInspector] public bool shootInput;

    [Header("Camera")]
    public float cameraTurnAmount;

    public void UpdateInputData(Vector3 newSteering, float newThrust, bool newShoot)
    {
        steeringInput = newSteering;
        thrustInput = newThrust;
        shootInput = newShoot;
    }

}
