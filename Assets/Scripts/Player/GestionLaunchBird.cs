using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestionLaunchBird : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private BirdTrajectory birdTrajectory;
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private RenderTrajectory renderTrajectory;

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
