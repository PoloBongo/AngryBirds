using LitMotion;
using UnityEngine;

public class LitMotionScaleAnimation : MonoBehaviour
{
    private void OnEnable()
    {
        this.transform.localScale = Vector3.zero;

        LMotion.Create(0f, 1f, 0.5f)
            .Bind(value => this.transform.localScale = Vector3.one * EaseOutBack(value));
    }
    
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }
}
