using UnityEngine;
using Cinemachine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class DeplacementMap : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.1f;
    [SerializeField] private float friction = 0.95f;
    [SerializeField] private float minX = -5f, maxX = 5f;

    private Vector2 startTouchPosition;
    private float velocity = 0f;
    [SerializeField] private Collider2D blockZoneCollider;

    [SerializeField] private GameObject cameraObject;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private CameraManager cameraManager;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (slingshot.isAim)
        {
            cameraManager.SwitchFollowToIdlePos();
            return;
        }
        
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                startTouchPosition = touch.screenPosition;

                if (IsTouchInBlockZone(startTouchPosition))
                {
                    return;
                }

                transform.position = cameraObject.transform.position;
                vcam.Follow = targetTransform;
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float swipeDistance = touch.screenPosition.x - startTouchPosition.x;
                velocity = swipeDistance * speedMultiplier;
            }
        }

        transform.position += new Vector3(velocity * Time.deltaTime, 0, 0);

        velocity *= friction;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            transform.position.y,
            transform.position.z
        );
    }
    
    private bool IsTouchInBlockZone(Vector2 screenPosition)
    {
        Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 10;

        return blockZoneCollider.OverlapPoint(worldPosition);
    }
}
