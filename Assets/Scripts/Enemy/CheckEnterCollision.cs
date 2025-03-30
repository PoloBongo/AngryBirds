using System;
using UnityEngine;

public class CheckEnterCollision : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private float requiredVelocity = 5f;
    [SerializeField] private LitMotionScaleAnimation litMotionScaleAnimation;
    [SerializeField] private AudioSource audioSource;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        
        if (rb != null) 
        {
            float impactVelocity = rb.velocity.magnitude;
            
            if (impactVelocity >= requiredVelocity)
            {
                litMotionScaleAnimation.enabled = true;
                audioSource.Play();
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.GameManagerInstance.AddBirdKill();
    }
}
