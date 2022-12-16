using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    private CinemachineFreeLook playerCamera;

    [Header("Spaceship Settings")]
    public SpaceshipData data;

    void Start()
    {
        playerCamera = Camera.main.transform.parent.GetComponentInChildren<CinemachineFreeLook>();
    }

    void Update()
    {
        MoveCamera();
        ModifyFOV();
    }

    void MoveCamera()
    {
        playerCamera.m_XAxis.Value = data.cameraTurnAmount * data.steeringInput.z;
    }

    void ModifyFOV()
    {
        playerCamera.m_Lens.FieldOfView = 40 + (data.thrustInput * 10);
    }
}
