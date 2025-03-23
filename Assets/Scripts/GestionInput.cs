using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestionInput : MonoBehaviour
{
    [Header("Input Actions")]
    private Camera _camera;
    
    [Header("Property")]
    [SerializeField] private BirdTrajectory birdTrajectory;
    [SerializeField] private GameObject birdObject;
    
    [Header("Bird Property")]
    private bool friction = false;
    private List<Vector3> trajectoryPoints;
    private int currentPointIndex = 0;
    private float movementSpeed = 1f;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnEnable()
    {
        TouchSimulation.Enable();
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
                    if (!hit.collider.gameObject.CompareTag("Player")) return;
                    Debug.Log("Objet touché : " + hit.collider.gameObject.name + " friction : " + friction);
                    birdTrajectory.DrawTrajectory(45f, 8f, friction);
                    trajectoryPoints = birdTrajectory.ComputeTrajectoryWithoutFriction(45f * Mathf.Deg2Rad, 8f);
                    
                    birdTrajectory.LaunchBird(45f, 8f);
                    
                    // reset pour le prochain lancer
                    trajectoryPoints.Clear();
                    currentPointIndex = 0;
                }
            }
        }

        // déplacement de l'oiseau
        // check si "trajectoryPoints" n'est pas null est sup à 0
        if (trajectoryPoints is not { Count: > 0 } || currentPointIndex >= trajectoryPoints.Count) return;
        Vector3 targetPosition = trajectoryPoints[currentPointIndex];
        birdObject.transform.position = Vector3.MoveTowards(birdObject.transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // déplace l'oiseau au point suivant
        if (birdObject.transform.position == targetPosition)
        {
            currentPointIndex++;
        }
    }
}
