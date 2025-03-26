using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestionLaunchBird : MonoBehaviour
{
    [Header("Input Actions")]
    private Camera _camera;
    
    [Header("Property")]
    [SerializeField] private BirdTrajectory birdTrajectory;
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private RenderTrajectory renderTrajectory;
    
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

    public void SwitchBirdTarget(GameObject _newBirdObj)
    {
        birdTrajectory = _newBirdObj.GetComponent<BirdTrajectory>();
        renderTrajectory.SwitchBirdTarget(_newBirdObj.transform);
    }

    public void Update()
    {
        if (slingshot.IsLaunch)
        {
            birdTrajectory.DrawTrajectory(slingshot.angleShot, slingshot.powerShot, birdTrajectory.GetUseFriction());
            birdTrajectory.LaunchBird(slingshot.angleShot, slingshot.powerShot);
                    
            // reset pour le prochain lancer
            renderTrajectory.ResetPoints();
            slingshot.IsLaunch = false;
        }
    }

    public void ClearDrawTrajectory(BirdTrajectory _birdTrajectory)
    {
        _birdTrajectory.GetLineRenderer().positionCount = 0;
    }
}
