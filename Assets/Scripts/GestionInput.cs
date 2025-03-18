using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestionInput : MonoBehaviour
{
    [Header("Input Actions")]
    private Camera _camera;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnEnable()
    {
        //TouchSimulation.Enable();
    }

    private void Start()
    {
        _camera = Camera.main;
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
    }

    public void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.isTap)
            {
                Vector2 worldPosition = _camera.ScreenToWorldPoint(touch.screenPosition);

                RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, ~0);

                
                if (hit.collider)
                {
                    Debug.Log("Objet touché : " + hit.collider.gameObject.name);
                }
            }
        }
    }
}