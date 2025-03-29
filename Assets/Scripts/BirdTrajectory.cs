using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BirdTrajectory : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private float mass;
    [SerializeField] private float gravity;
    [SerializeField] private float spring = 10f; // constante de raideur du ressort (N/m)
    
    [Header("Components")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private DetectionObstacle detectionObstacle;

    private const int numPoints = 100;
    [SerializeField] private bool useFriction = false;

    private List<Vector3> trajectoryPoints;
    private float elapsedTime = 0f;
    private Vector2 stockVelocity;
    private Vector2 velocityLastPoint;
    
    [Header("Jump")]
    private bool jumpDeclenched;
    private bool canUpdated = false;
    
    [Header("Capacity")]
    [SerializeField] private bool debugDrawCapacity;
    [SerializeField] private bool isBirdDuplication;

    public float durationTpPoints { get; set; }
    public bool trajectoryFinish { get; set; }

    public bool GetUseFriction() => useFriction;
    public Vector2 GetStockVelocity() => stockVelocity;
    public LineRenderer GetLineRenderer() => lineRenderer;
    private void Start()
    {
        if (slingshot == null) slingshot = FindSlingshot();
        
        jumpDeclenched = false;
        gravity = Mathf.Abs(Physics2D.gravity.y); // on utilise la gravité d'unity plutôt qu'en brute (c'est la même valeur soit 9.81f)
        mass = rigidBody2D.mass; // masse de l'oiseau (kg)
        slingshot.frictionShot /= mass;
    }

    private Slingshot FindSlingshot()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Slingshot");
        return obj.GetComponent<Slingshot>();
    }

    public void DrawTrajectory(float angleDegrees, float l1, bool withFriction)
    {
       float angle = Mathf.Abs(angleDegrees) * Mathf.Deg2Rad;
       float velocity = SpeedInitial(angle, l1);

        trajectoryPoints = withFriction ? 
            ComputeTrajectoryWithFriction(angle, velocity) : 
            ComputeTrajectoryWithoutFriction(angle, velocity);

        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }
    
    public void DrawTrajectoryRecurrence()
    {
        jumpDeclenched = true;
        durationTpPoints = 0.015f;
        
        float angle = slingshot.angleShot * Mathf.Deg2Rad;
        float velocity = SpeedInitial(angle, slingshot.powerShot);
        float friction = slingshot.frictionShot;

        trajectoryPoints = useFriction ? 
            ComputeJumpTrajectoryWithFriction(angle, velocity, friction)
            : ComputeJumpTrajectoryWithoutFriction(angle, velocity);
        
        lineRenderer.positionCount = trajectoryPoints.Count;

        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
        
        LaunchBird(slingshot.angleShot, slingshot.powerShot);
    }
    
    public void DrawTrajectoryWithLineRenderer(float angleDegrees, float l1, bool withFriction)
    {
        if (slingshot == null) slingshot = FindSlingshot();
        
        float angle = angleDegrees * Mathf.Deg2Rad;
        float velocity = SpeedInitial(angle, l1);
        gravity = Mathf.Abs(Physics2D.gravity.y);

        trajectoryPoints = withFriction ? 
            ComputeTrajectoryWithFriction(angle, velocity, true) : 
            ComputeTrajectoryWithoutFriction(angle, velocity, true);

        if (debugDrawCapacity)
        {
            lineRenderer.positionCount = trajectoryPoints.Count;
            lineRenderer.SetPositions(trajectoryPoints.ToArray());
        }

        canUpdated = true;
        
        LaunchBird(slingshot.angleShot, slingshot.powerShot);
    }

    private float SpeedInitial(float angle, float l1)
    {
        var v_eject = l1 * Mathf.Sqrt(spring / mass) *
                      Mathf.Sqrt(1 - Mathf.Pow(mass * Mathf.Abs(Physics2D.gravity.y) * Mathf.Sin(angle) / (spring * l1), 2));
        return v_eject;
    }

    private List<Vector3> ComputeTrajectoryWithoutFriction(float angle, float velocity, bool isForCapacities = false)
    {
        List<Vector3> points = new List<Vector3>(); // stock le x,y
        Vector3 startPosition = transform.position;
        
        float timeMax = (2 * velocity * Mathf.Sin(angle)) / gravity; // temps d'impact avec le sol
        float timeStep = timeMax / numPoints; // divise le temps entre chaque point de la trajectoire

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep; // actuel temps
            float x = velocity * Mathf.Cos(angle) * t; // pos horizontal
            float y = velocity * Mathf.Sin(angle) * t - 0.5f * gravity * t * t; // pos vertical
            points.Add(new Vector3(isForCapacities ? startPosition.x + x : x, isForCapacities ? startPosition.y + y : y, 0)); // stock les points x,y pour les draw ensuite
        }

        return points;
    }

    private List<Vector3> ComputeTrajectoryWithFriction(float angle, float velocity, bool isForCapacities = false)
    {
        List<Vector3> points = new List<Vector3>(); // stock le x,y
        Vector3 startPosition = transform.position;
        
        float lambdaX = velocity * Mathf.Cos(angle); // vitesse horizontal
        float lambdaY = velocity * Mathf.Sin(angle) + gravity / slingshot.frictionShot; // vitesse vertical en prenant compte de la résistance de l'air
        float timeMax = 2 * velocity * Mathf.Sin(angle) / gravity; // temps d'impact avec le sol
        float timeStep = timeMax / numPoints; // divise le temps entre chaque point de la trajectoire
        
        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep;
            float x = (lambdaX / slingshot.frictionShot) * (1 - Mathf.Exp(-slingshot.frictionShot * t)); // pos horizontal
            float y = (lambdaY / slingshot.frictionShot) * (1 - Mathf.Exp(-slingshot.frictionShot * t)) - (gravity / slingshot.frictionShot) * t; // pos vertical
            
            points.Add(new Vector3(isForCapacities ? startPosition.x + x : x, isForCapacities ? startPosition.y + y : y, 0)); // stock les points x,y pour les draw ensuite
        }

        return points;
    }
    
    private List<Vector3> ComputeJumpTrajectoryWithoutFriction(float angle, float velocity)
    {
        List<Vector3> points = new List<Vector3>();
        float dt = 0.01f;
        
        float x = transform.position.x;
        float y = transform.position.y;
        
        float velocityX = velocity * Mathf.Cos(angle);
        float velocityY = velocity * Mathf.Sin(angle);

        while (y >= 0)
        {
            x += velocityX * dt;
            y += velocityY * dt;

            if (y < 0) break;

            points.Add(new Vector3(x, y, 0));

            velocityY -= gravity * dt;
        }

        return points;
    }
    
    private List<Vector3> ComputeJumpTrajectoryWithFriction(float angle, float velocity, float friction)
    {
        List<Vector3> points = new List<Vector3>();
        float dt = 0.01f;

        float x = transform.position.x;
        float y = transform.position.y;

        float velocityX = velocity * Mathf.Cos(angle);
        float velocityY = velocity * Mathf.Sin(angle);

        points.Add(new Vector3(x, y, 0));

        while (y > 0)
        {
            x += velocityX * dt;
            y += velocityY * dt;

            points.Add(new Vector3(x, y, 0));

            velocityX -= friction * velocityX * dt;
            velocityY -= (gravity + friction * velocityY) * dt;

            if (points.Count > 500) break; 
        }

        return points;
    }

    
    public void LaunchBird(float angleDegrees, float l1)
    {
        if (!slingshot) slingshot = FindSlingshot();
        
        float angle = angleDegrees * Mathf.Deg2Rad;
        float velocity = SpeedInitial(angle, l1);
        float adjustedVelocity = velocity * (1 - slingshot.frictionShot + 0.1f);
        
        float velocityX = useFriction ? adjustedVelocity * Mathf.Cos(angle) : velocity * Mathf.Cos(angle);
        float velocityY = useFriction ? adjustedVelocity * Mathf.Sin(angle) : velocity * Mathf.Sin(angle);
        
        // applique la vitesse au rigidbody pour simuler la physique sur la trajectoire
        // - cela permet de laisser unity gérer la collision avec les autres objects
        if (slingshot.GetEnableUnityGravity())
        {
            rigidBody2D.velocity = new Vector2(velocityX, velocityY);
        }
        else
        {
            stockVelocity = new Vector2(velocityX, velocityY);
            trajectoryFinish = false;
        }
    }

    private void Update()
    {
        if (isBirdDuplication)
        {
            if (trajectoryPoints.Count <= 0 || trajectoryFinish ||
                slingshot.GetEnableUnityGravity() || !canUpdated) return;
        }
        else
        {
            if (!slingshot.CanResetCamera || trajectoryPoints.Count <= 0 || trajectoryFinish ||
                slingshot.GetEnableUnityGravity()) return;
        }
        
        if (jumpDeclenched)
        {
            elapsedTime = 0f;
            jumpDeclenched = false;
        }
        elapsedTime += Time.deltaTime;
            
        int index = Mathf.Clamp((int)(elapsedTime / durationTpPoints), 0, trajectoryPoints.Count - 1);
        transform.position = trajectoryPoints[index];
            
        if (index >= trajectoryPoints.Count - 1)
        {
            rigidBody2D.gravityScale = 1f;
            trajectoryFinish = true;
            detectionObstacle.antiSpam = true;

            rigidBody2D.velocity = stockVelocity;
        }
    }
}