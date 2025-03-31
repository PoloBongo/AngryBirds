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
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    private bool validateReleased;
    private bool alreadyHit;
    public bool isAim { get; private set; }
    private Vector3 currentPosition { get; set; }
    public float angleShot { get; private set; }
    public float powerShot { get; private set; }
    public float frictionShot { get; set; } // coeff de frottement divisé par la masse
    public bool IsLaunch { get; set; }
    public bool CanResetCamera { get; set; }
    
    private new Camera camera;
    
    private void Start()
    {
        camera = Camera.main;
        IsLaunch = false;
        CanResetCamera = false;
        validateReleased = false;
        isAim = false;
        GameManager.GameManagerInstance.canPlay = true;
        alreadyHit = false;
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);
        frictionShot = 0.2f;

        DisableGravity();
        ResetStrips();

        if (manageBirds.Index == 0) GameManager.GameManagerInstance.SetActualBirdsKill(0);
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
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
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
        isAim = false;
        GameManager.GameManagerInstance.canPlay = true;
    }
    
    private void Update()
    {
        if (manageBirds.Index >= manageBirds.Birds.Count || !GameManager.GameManagerInstance.canPlay) return;
        
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

                isAim = true;
                validateReleased = true;
                currentPosition = camera.ScreenToWorldPoint(mousePosition);
                currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLength);
                
                powerShot = GetLineRendererLength(lineRenderers[0]) * powerMultiplication;
                angleShot = GetSlingshotAngle();
                
                manageBirds.Birds[manageBirds.Index].DrawTrajectory(angleShot, powerShot, GameManager.GameManagerInstance.useFriction);
                player.transform.position = currentPosition;
                
                SetStrips(currentPosition);
            }
            else
            {
                if (!validateReleased) return;
                
                if (!GameManager.GameManagerInstance.debugDraw) gestionLaunchBird.ClearDrawTrajectory(manageBirds.Birds[manageBirds.Index]);
                CanResetCamera = true;
                cameraManager.SwitchFollowToPlayer();
                IsLaunch = true;
                isAim = false;
                if (GameManager.GameManagerInstance.enableGravity) EnableGravity();
                ResetStrips();
                GameManager.GameManagerInstance.canPlay = false;
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
