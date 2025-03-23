using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class GestionLaunchBird : MonoBehaviour
{
    [Header("Input Actions")]
    private Camera _camera;
    
    [Header("Property")]
    [SerializeField] private BirdTrajectory birdTrajectory;
    [SerializeField] private GameObject birdObject;
    [SerializeField] private Slingshot slingshot;
    
    [Header("Bird Property")]
    private List<Vector3> trajectoryPoints;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnEnable()
    {
        TouchSimulation.Enable();
    }

    private void Start()
    {
        _camera = Camera.main;
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
    }

    public void Update()
    {
        if (slingshot.IsLaunch)
        {
            birdTrajectory.DrawTrajectory(slingshot.angleShot, slingshot.powerShot, birdTrajectory.GetUseFriction());
            trajectoryPoints = birdTrajectory.GetUseFriction() ?
                birdTrajectory.ComputeTrajectoryWithFriction(slingshot.angleShot * Mathf.Deg2Rad, slingshot.powerShot)
                : birdTrajectory.ComputeTrajectoryWithoutFriction(slingshot.angleShot * Mathf.Deg2Rad, slingshot.powerShot);
                    
            birdTrajectory.LaunchBird(slingshot.angleShot, slingshot.powerShot);
                    
            // reset pour le prochain lancer
            trajectoryPoints.Clear();
            slingshot.IsLaunch = false;
        }
    }
}
