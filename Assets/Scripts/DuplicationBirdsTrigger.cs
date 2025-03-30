using UnityEngine;

public class DuplicationBirdsTrigger : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private CircleCollider2D circleCollider2D;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("BirdCapacity"))
        {
            circleCollider2D.isTrigger = false;
        }
    }
}
