using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Property")] [SerializeField] private SpriteRenderer spriteRenderer;
    private void Start()
    {
        FitToScreen();
    }

    private void FitToScreen()
    {
        float width = spriteRenderer.bounds.size.x;
        float height = spriteRenderer.bounds.size.y;

        if (Camera.main != null)
        {
            float worldScreenHeight = Camera.main.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

            transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
        }
    }
}