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

    private bool canResetVelocity = false;

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
            canResetVelocity = true;

            foreach (BirdTrajectory bird in duplicationBirds.GetBirdsDuplication())
            {
                gestionLaunchBird.ClearDrawTrajectory(bird);
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (!canResetVelocity) return;
        Rigidbody2D rb = manageBirds.Birds[manageBirds.Index - 1].GetComponent<Rigidbody2D>();
        rb.velocity *= 0.95f;
        
        if (rb.velocity.sqrMagnitude < 0.01f)
        {
            rb.velocity = Vector2.zero;
            canResetVelocity = false;
        }
    }

}
