using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceshipController : MonoBehaviour
{
    [Header("Spaceship Settings")]
    public SpaceshipData data;

    [Header("Physics")]
    public Rigidbody spaceshipRigidbody;

    [Header("Shooting")]
    public ObjectPoolBehaviour projectileObjectPool;
    public Transform projectileSpawnTransform;
    private float nextShot = 0.0f;
    public Transform shipModel;

    void FixedUpdate()
    {
        MoveSpaceship();
        TurnSpaceship();
        CalculateShootingLogic();
    }

     void MoveSpaceship()
    {
        spaceshipRigidbody.velocity = transform.forward * data.thrustAmount * (Mathf.Max(data.thrustInput,.2f));
    }

    void TurnSpaceship()
    {
        Vector3 newTorque = new Vector3(data.steeringInput.x * data.pitchSpeed, -data.steeringInput.z * data.yawSpeed, 0);
        spaceshipRigidbody.AddRelativeTorque(newTorque);

        spaceshipRigidbody.rotation =
            Quaternion.Slerp(spaceshipRigidbody.rotation, Quaternion.Euler(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0)), .5f);

        VisualSpaceshipTurn();
    }

    void VisualSpaceshipTurn()
    {
        shipModel.localEulerAngles = new Vector3(data.steeringInput.x * data.leanAmount_Y
            , shipModel.localEulerAngles.y, data.steeringInput.z * data.leanAmount_X);
    }

    void CalculateShootingLogic()
    {
        if(data.shootInput == true)
        {
            if(Time.time > nextShot)
            {
                ShootProjectile();
                nextShot = Time.time + data.shootRate;
            }
        }
    }

    void ShootProjectile()
    {
        
        GameObject newProjectile = projectileObjectPool.GetPooledObject();
        newProjectile.transform.position = projectileSpawnTransform.position;
        newProjectile.transform.rotation = projectileSpawnTransform.rotation;
        newProjectile.SetActive(true);
        
    }


}
