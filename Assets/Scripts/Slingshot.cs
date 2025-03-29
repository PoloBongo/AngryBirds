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
    [SerializeField] private float powerMultiplication = 1.75f;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private ManageBirds manageBirds;
    [SerializeField] private GestionLaunchBird gestionLaunchBird;
    [SerializeField] private bool enableUnityGravity;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    private bool validateReleased;
    private bool alreadyHit;
    private Vector3 currentPosition { get; set; }
    public float angleShot { get; private set; }
    public float powerShot { get; private set; }
    public float frictionShot { get; set; } // coeff de frottement divisé par la masse
    public bool IsLaunch { get; set; }
    public bool CanResetCamera { get; set; }
    
    public bool GetEnableUnityGravity() => enableUnityGravity;
    
    private new Camera camera;
    
    private void Start()
    {
        camera = Camera.main;
        IsLaunch = false;
        CanResetCamera = false;
        validateReleased = false;
        alreadyHit = false;
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);
        frictionShot = 0.2f;

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
    
    private float GetSlingshotAngle()
    {
        Vector3 direction = center.position - currentPosition;
        float angle = Vector2.Angle(Vector2.right, direction);
        
        if (Vector2.Dot(Vector2.up, direction) < 0)
        {
            angle = -angle;
        }
        
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        return angle;
    }

    public void SwitchBird()
    {
        if (manageBirds.Index >= manageBirds.Birds.Count) return;
        
        manageBirds.Birds[manageBirds.Index - 1].gameObject.tag = "Untagged";
        manageBirds.Birds[manageBirds.Index].gameObject.tag = "Player";
        player = manageBirds.Birds[manageBirds.Index].gameObject;
        player.transform.position = idlePosition.position;
        cameraManager.SwitchFollowBird(player.transform);
        gestionLaunchBird.SwitchBirdTarget(player);
        DisableGravity();
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        validateReleased = false;
        alreadyHit = false;
    }
    
    private void Update()
    {
        if (manageBirds.Index >= manageBirds.Birds.Count) return;
        
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.inProgress)
            {
                Vector3 mousePosition = touch.screenPosition;
                mousePosition.z = 10;
                
                RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(mousePosition), Vector2.zero);
                if (hit.collider && hit.collider.CompareTag("Player") && !alreadyHit)
                {
                    alreadyHit = true;
                }
                else if (!alreadyHit)
                {
                    validateReleased = false;
                    return;
                }

                validateReleased = true;
                currentPosition = camera.ScreenToWorldPoint(mousePosition);
                currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLength);
                
                powerShot = GetLineRendererLength(lineRenderers[0]) * powerMultiplication;
                angleShot = GetSlingshotAngle();
                
                if (angleShot < minAngle || angleShot > maxAngle)
                {
                    float clampedX = center.position.x + maxLength * Mathf.Cos(angleShot * Mathf.Deg2Rad);
                    float clampedY = center.position.y + maxLength * Mathf.Sin(angleShot * Mathf.Deg2Rad);
                    currentPosition = new Vector3(clampedX, clampedY, 0);
                }
                
                manageBirds.Birds[manageBirds.Index].DrawTrajectory(angleShot, powerShot, manageBirds.Birds[manageBirds.Index].GetUseFriction());
                player.transform.position = currentPosition;
                
                SetStrips(currentPosition);
            }
            else
            {
                if (!validateReleased) return;

                if (angleShot >= 25f) manageBirds.Birds[manageBirds.Index].durationTpPoints = 0.02f;
                if (angleShot < 25f) manageBirds.Birds[manageBirds.Index].durationTpPoints = 0.015f;
                
                CanResetCamera = true;
                cameraManager.SwitchFollowToPlayer();
                IsLaunch = true;
                if (enableUnityGravity) EnableGravity();
                ResetStrips();
                gestionLaunchBird.ClearDrawTrajectory(manageBirds.Birds[manageBirds.Index]);
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
    
    private float GetLineRendererLength(LineRenderer lineRenderer)
    {
        if (lineRenderer.positionCount < 2)
            return 0f;

        Vector3 start = lineRenderer.GetPosition(0);
        Vector3 end = lineRenderer.GetPosition(1);

        return Vector3.Distance(start, end);
    }
}
