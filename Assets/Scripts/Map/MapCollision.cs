using System;
using UnityEngine;

public class MapCollision : MonoBehaviour
{
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Transform idlePosition;
    [SerializeField] private ManageBirds manageBirds;
    [SerializeField] private GestionLaunchBird gestionLaunchBird;
    [SerializeField] private DuplicationBirds duplicationBirds;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && slingshot.CanResetCamera)
        {
            Debug.Log(other.gameObject.name);
            gestionLaunchBird.ClearDrawTrajectory(manageBirds.Birds[manageBirds.Index]);
            manageBirds.Index++;
            manageBirds.ManageBirdsVisibility();
            cameraManager.SwitchFollowToIdlePos();
            slingshot.CanResetCamera = false;
            slingshot.SwitchBird();

            foreach (BirdTrajectory bird in duplicationBirds.GetBirdsDuplication())
            {
                gestionLaunchBird.ClearDrawTrajectory(bird);
            }
        }
    }
}
