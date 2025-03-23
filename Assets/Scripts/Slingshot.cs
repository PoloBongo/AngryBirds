using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Slingshot : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private LineRenderer[] lineRenderers;
    [SerializeField] private Transform[] stripPositions;
    [SerializeField] private Transform center;
    [SerializeField] private Transform idlePosition;
    [SerializeField] private float maxLength;
    [SerializeField] private GameObject player;

    private Vector3 currentPosition { get; set; }
    public bool IsLaunch { get; set; }
    
    private new Camera camera;
    
    private void Start()
    {
        camera = Camera.main;
        IsLaunch = false;
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        DisableGravity();
        ResetStrips();
    }

    private void DisableGravity()
    {
        player.GetComponent<Rigidbody2D>().gravityScale = 0f;
    }
    
    private void EnableGravity()
    {
        player.GetComponent<Rigidbody2D>().gravityScale = 1f;
    }

    private void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.inProgress)
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = 10;
            
                currentPosition = camera.ScreenToWorldPoint(mousePosition);
                currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLength);
                player.transform.position = currentPosition;
                
                SetStrips(currentPosition);
            }
            else
            {
                IsLaunch = true;
                EnableGravity();
                ResetStrips();
            }
        }
    }

    private void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
        player.transform.position = currentPosition;
    }

    private void SetStrips(Vector3 _position)
    {
        lineRenderers[0].SetPosition(1, _position);
        lineRenderers[1].SetPosition(1, _position);
    }
}
