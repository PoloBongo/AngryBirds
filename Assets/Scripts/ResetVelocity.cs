using UnityEngine;

public class ResetVelocity : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private void Start()
    {
        rb.velocity = Vector3.zero;
    }
    
}
