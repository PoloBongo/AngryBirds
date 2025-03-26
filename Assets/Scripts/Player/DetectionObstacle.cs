using System;
using UnityEngine;

public class DetectionObstacle : MonoBehaviour
{
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private new Rigidbody2D rigidbody2D;
    [SerializeField] private BirdTrajectory birdTrajectory;

    public bool antiSpam { get; set; }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player") && slingshot.CanResetCamera && !antiSpam)
        {
            rigidbody2D.gravityScale = 1f;
            birdTrajectory.trajectoryFinish = true;
            rigidbody2D.velocity = birdTrajectory.GetStockVelocity();
            antiSpam = true;
        }
    }
}
