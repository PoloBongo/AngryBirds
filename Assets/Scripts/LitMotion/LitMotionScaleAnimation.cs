using LitMotion;
using UnityEngine;

public class LitMotionScaleAnimation : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private float from;
    [SerializeField] private float to;
    [SerializeField] private float duration;
    [SerializeField] private bool bird;
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;

        LMotion.Create(from, to, duration)
            .WithOnComplete(() => { if (bird) Destroy(gameObject); })
            .Bind(value => { if (transform) transform.localScale = Vector3.one * EaseOutBack(value); });
    }
    
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }
}
