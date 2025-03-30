using UnityEngine;

public class CheckEnterCollision : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private float requiredVelocity = 5f;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        
        if (rb != null) 
        {
            float impactVelocity = rb.velocity.magnitude;
            
            if (impactVelocity >= requiredVelocity)
            {
                GameManager.GameManagerInstance.AddBirdKill();
                Destroy(gameObject);
            }
        }
    }
}
