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

    private const int numPoints = 200;

    private List<Vector3> trajectoryPoints;
    private float elapsedTime = 0f;
    private Vector2 stockVelocity;
    private Vector2 velocityLastPoint;
    
    float baseSpeed = 7000f;
    float maxSpeed = 20000f;
    
    [Header("Jump")]
    private bool jumpDeclenched;
    private bool canUpdated = false;
    
    [Header("Information")]
    [SerializeField] private bool isBirdDuplication;

    public float durationTpPoints { get; set; }
    public bool trajectoryFinish { get; set; }
    public Vector2 GetStockVelocity() => stockVelocity;
    public LineRenderer GetLineRenderer() => lineRenderer;
    private void Start()
    {
        if (slingshot == null) slingshot = FindSlingshot();
        
        jumpDeclenched = false;
        gravity = Mathf.Abs(Physics2D.gravity.y); // on utilise la gravité d'unity plutôt qu'en brute (c'est la même valeur soit 9.81f)
        mass = rigidBody2D.mass; // masse de l'oiseau (kg)
        slingshot.frictionShot /= mass;

        durationTpPoints = 0.1f;
        
        baseSpeed = 7000f;
        maxSpeed = 20000f;
    }

    private Slingshot FindSlingshot()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Slingshot");
        return obj.GetComponent<Slingshot>();
    }

    public void DrawTrajectory(float angleDegrees, float l1, bool withFriction)
    {
        baseSpeed = withFriction ? 1000f : 7000f;
        maxSpeed = withFriction ? 10000f : 20000f;
        
        float angle = angleDegrees * Mathf.Deg2Rad;
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
        baseSpeed = 1000f;
        maxSpeed = 10000f;
        
        float angle = slingshot.angleShot * Mathf.Deg2Rad;
        float velocity = SpeedInitial(angle, slingshot.powerShot);
        float friction = slingshot.frictionShot;

        trajectoryPoints = GameManager.GameManagerInstance.useFriction ? 
            ComputeJumpTrajectoryWithFriction(angle, velocity, friction)
            : ComputeJumpTrajectoryWithoutFriction(angle, velocity);
        
        lineRenderer.positionCount = trajectoryPoints.Count;

        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
        
        LaunchBird(slingshot.angleShot, slingshot.powerShot);
    }
    
    public void DrawTrajectoryWithLineRenderer(float angleDegrees, float l1, bool withFriction)
    {
        if (!slingshot) slingshot = FindSlingshot();
        
        float angle = angleDegrees * Mathf.Deg2Rad;
        float velocity = SpeedInitial(angle, l1);
        gravity = Mathf.Abs(Physics2D.gravity.y);

        trajectoryPoints = withFriction ? 
            ComputeTrajectoryWithFriction(angle, velocity, true) : 
            ComputeTrajectoryWithoutFriction(angle, velocity, true);

        if (GameManager.GameManagerInstance.debugDraw)
        {
            lineRenderer.positionCount = trajectoryPoints.Count;
            lineRenderer.SetPositions(trajectoryPoints.ToArray());
        }

        canUpdated = true;
        
        LaunchBird(angleDegrees, l1);
    }

    /// \brief Calcule la vitesse initiale d'éjection.
    /// \param angle Angle de lancement en radians.
    /// \param l1 Longueur du ressort.
    /// \return La vitesse initiale.
    ///
    /// La formule utilisée est :
    /// \f[
    /// v_{eject} = l1 \times \sqrt{\frac{spring}{mass}} \times 
    /// \sqrt{1 - \left(\frac{mass \times |g| \times \sin(angle)}{spring \times l1}\right)^2}
    /// \f]
    private float SpeedInitial(float angle, float l1)
    {
        var v_eject = l1 * Mathf.Sqrt(spring / mass) *
                      Mathf.Sqrt(1 - Mathf.Pow(mass * Mathf.Abs(Physics2D.gravity.y) * Mathf.Sin(angle) / (spring * l1), 2));
        return v_eject;
    }

    /// \brief Calcule la trajectoire d'un projectile sans frottement.
    /// \param angle Angle de lancement en radians.
    /// \param velocity Vitesse initiale.
    /// \param isForCapacities Indique si la trajectoire doit être ajustée à la position initiale.
    /// \return Une liste de points représentant la trajectoire.
    ///
    /// La position du projectile est donnée par les équations :
    /// \f[
    /// x = v_0 \cos(\theta) t
    /// \f]
    /// \f[
    /// y = v_0 \sin(\theta) t - \frac{1}{2} g t^2
    /// \f]
    private List<Vector3> ComputeTrajectoryWithoutFriction(float angle, float velocity, bool isForCapacities = false)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 startPosition = transform.position;

        float timeMax = (velocity * Mathf.Sin(angle) +
                         Mathf.Sqrt(Mathf.Pow(velocity * Mathf.Sin(angle), 2) + 2 * gravity * startPosition.y));
        float timeStep = timeMax / numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep;
            float x = velocity * Mathf.Cos(angle) * t;
            float y = velocity * Mathf.Sin(angle) * t - 0.5f * gravity * t * t;
            
            if (float.IsNaN(x) || float.IsNaN(y)) continue;
            points.Add(new Vector3(isForCapacities ? startPosition.x + x : x, isForCapacities ? startPosition.y + y : y, 0));
        }

        return points;
    }

    /// \brief Calcule la trajectoire d'un projectile avec frottement.
    /// \param angle Angle de lancement en radians.
    /// \param velocity Vitesse initiale.
    /// \param isForCapacities Indique si la trajectoire doit être ajustée à la position initiale.
    /// \return Une liste de points représentant la trajectoire.
    ///
    /// La trajectoire avec frottement suit ces équations :
    /// \f[
    /// x = \frac{\lambda_x}{k} (1 - e^{-k t})
    /// \f]
    /// \f[
    /// y = \frac{\lambda_y}{k} (1 - e^{-k t}) - \frac{g}{k} t
    /// \f]
    /// Où \( \lambda_x = v_0 \cos(\theta) \) et \( \lambda_y = v_0 \sin(\theta) + \frac{g}{k} \).
    private List<Vector3> ComputeTrajectoryWithFriction(float angle, float velocity, bool isForCapacities = false)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 startPosition = transform.position;

        float lambdaX = velocity * Mathf.Cos(angle);
        float lambdaY = velocity * Mathf.Sin(angle) + gravity / slingshot.frictionShot;
        float timeMax = (velocity * Mathf.Sin(angle) +
                         Mathf.Sqrt(Mathf.Pow(velocity * Mathf.Sin(angle), 2) + 2 * gravity * startPosition.y));
        float timeStep = timeMax / numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep;
            float x = (lambdaX / slingshot.frictionShot) * (1 - Mathf.Exp(-slingshot.frictionShot * t));
            float y = (lambdaY / slingshot.frictionShot) * (1 - Mathf.Exp(-slingshot.frictionShot * t)) - (gravity / slingshot.frictionShot) * t;
            
            if (float.IsNaN(x) || float.IsNaN(y)) continue;
            points.Add(new Vector3(isForCapacities ? startPosition.x + x : x, isForCapacities ? startPosition.y + y : y, 0));
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
        
        float velocityX = GameManager.GameManagerInstance.useFriction ? adjustedVelocity * Mathf.Cos(angle) : velocity * Mathf.Cos(angle);
        float velocityY = GameManager.GameManagerInstance.useFriction ? adjustedVelocity * Mathf.Sin(angle) : velocity * Mathf.Sin(angle);
        
        // applique la vitesse au rigidbody pour simuler la physique sur la trajectoire
        // - cela permet de laisser unity gérer la collision avec les autres objects
        if (GameManager.GameManagerInstance.enableGravity)
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
                GameManager.GameManagerInstance.enableGravity || !canUpdated) return;
        }
        else
        {
            if (!slingshot.CanResetCamera || trajectoryPoints.Count <= 0 || trajectoryFinish ||
                GameManager.GameManagerInstance.enableGravity) return;
        }
        
        if (jumpDeclenched)
        {
            elapsedTime = 0f;
            jumpDeclenched = false;
        }
        
        elapsedTime += Time.deltaTime;

        float totalDistance = 0;
        for (int i = 0; i < trajectoryPoints.Count - 1; i++) {
            totalDistance += Vector3.Distance(trajectoryPoints[i], trajectoryPoints[i + 1]);
        }

        float speed = Mathf.Lerp(baseSpeed, maxSpeed, totalDistance / 50f);

        durationTpPoints = totalDistance / speed; 

        float t = elapsedTime / durationTpPoints;
        int index = Mathf.Clamp((int)t, 0, trajectoryPoints.Count - 2);

        float lerpFactor = t - index;
        transform.position = Vector3.Lerp(trajectoryPoints[index], trajectoryPoints[index + 1], lerpFactor);



        if (index >= trajectoryPoints.Count - 2)
        {
            rigidBody2D.gravityScale = 1f;
            trajectoryFinish = true;
            detectionObstacle.antiSpam = true;

            if (trajectoryPoints.Count < 2) return;
            Vector3 lastPoint = trajectoryPoints[^1];
            Vector3 secondLastPoint = trajectoryPoints[^2];

            float distanceX = lastPoint.x - secondLastPoint.x;
            float distanceY = lastPoint.y - secondLastPoint.y;
            float timeDelta = durationTpPoints;

            float velocityX = distanceX / timeDelta;
            float velocityY = distanceY / timeDelta;

            rigidBody2D.velocity = new Vector2(velocityX, velocityY);
        }
    }
}