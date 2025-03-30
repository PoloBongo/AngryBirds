using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckEnterCollision : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private float requiredVelocity = 5f;
    [SerializeField] private LitMotionScaleAnimation litMotionScaleAnimation;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<ParticleSystem> particleSystems;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        int random = Random.Range(0, 4);
        Rigidbody2D rb = collision.rigidbody;
        
        if (rb != null) 
        {
            float impactVelocity = rb.velocity.magnitude;
            
            if (impactVelocity >= requiredVelocity)
            {
                litMotionScaleAnimation.enabled = true;
                audioSource.Play();
                if (particleSystems[random])
                {
                    particleSystems[random].gameObject.transform.position = this.transform.position;
                    particleSystems[random].Play();
                }
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.GameManagerInstance.AddBirdKill();
    }
}
