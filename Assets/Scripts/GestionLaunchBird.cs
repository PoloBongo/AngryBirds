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
            birdTrajectory.DrawTrajectory(45f, 8f, false);
            trajectoryPoints = birdTrajectory.ComputeTrajectoryWithoutFriction(45f * Mathf.Deg2Rad, 8f);
                    
            birdTrajectory.LaunchBird(45f, 8f);
                    
            // reset pour le prochain lancer
            trajectoryPoints.Clear();
            slingshot.IsLaunch = false;
        }
    }
}
