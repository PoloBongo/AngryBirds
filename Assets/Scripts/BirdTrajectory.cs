using System;
using System.Collections;
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

    private const int numPoints = 100;
    [SerializeField] private bool useFriction = false;

    public bool GetUseFriction() => useFriction;
    public LineRenderer GetLineRenderer() => lineRenderer;
    private void Start()
    {
        gravity = Mathf.Abs(Physics2D.gravity.y); // on utilise la gravité d'unity plutôt qu'en brute (c'est la même valeur soit 9.81f)
        mass = rigidBody2D.mass; // masse de l'oiseau (kg)
        slingshot.frictionShot /= mass;
    }

    public void DrawTrajectory(float angleDegrees, float stretchLength, bool withFriction)
    {
        float angle = angleDegrees * Mathf.Deg2Rad;
        float velocity = SpeedInitial(angle, stretchLength);

        List<Vector3> points = withFriction ? 
            ComputeTrajectoryWithFriction(angle, velocity) : 
            ComputeTrajectoryWithoutFriction(angle, velocity);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
    
    public void DrawTrajectoryRecurrence()
    {
        float angle = slingshot.angleShot * Mathf.Deg2Rad;
        float velocity = SpeedInitial(angle, slingshot.powerShot);

        List<Vector3> points = ComputeJumpTrajectory(10f, angle, velocity);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
        
        LaunchBirdJump(slingshot.angleShot, velocity);
    }

    private float SpeedInitial(float angle, float l1)
    {
        var v_eject = l1 * Mathf.Sqrt(spring / mass) *
                      Mathf.Sqrt(1 - Mathf.Pow(mass * Mathf.Abs(Physics2D.gravity.y) * Mathf.Sin(angle) / (spring * l1), 2));
        return v_eject;
    }

    private List<Vector3> ComputeTrajectoryWithoutFriction(float angle, float velocity)
    {
        List<Vector3> points = new List<Vector3>(); // stock le x,y
        float timeMax = (2 * velocity * Mathf.Sin(angle)) / gravity; // temps d'impact avec le sol
        float timeStep = timeMax / numPoints; // divise le temps entre chaque point de la trajectoire

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep; // actuel temps
            float x = velocity * Mathf.Cos(angle) * t; // pos horizontal
            float y = velocity * Mathf.Sin(angle) * t - 0.5f * gravity * t * t; // pos vertical
            points.Add(new Vector3(x, y, 0)); // stock les points x,y pour les draw ensuite
        }
        return points;
    }

    private List<Vector3> ComputeTrajectoryWithFriction(float angle, float velocity)
    {
        List<Vector3> points = new List<Vector3>(); // stock le x,y
        float lambdaX = velocity * Mathf.Cos(angle); // vitesse horizontal
        float lambdaY = velocity * Mathf.Sin(angle) + gravity / slingshot.frictionShot; // vitesse vertical en prenant compte de la résistance de l'air
        float timeMax = 2 * velocity * Mathf.Sin(angle) / gravity; // temps d'impact avec le sol
        float timeStep = timeMax / numPoints; // divise le temps entre chaque point de la trajectoire
        
        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep;
            float x = (lambdaX / slingshot.frictionShot) * (1 - Mathf.Exp(-slingshot.frictionShot * t)); // pos horizontal
            float y = (lambdaY / slingshot.frictionShot) * (1 - Mathf.Exp(-slingshot.frictionShot * t)) - (gravity / slingshot.frictionShot) * t; // pos vertical
            
            points.Add(new Vector3(x, y, 0)); // stock les points x,y pour les draw ensuite
        }
        return points;
    }
    
    private List<Vector3> ComputeJumpTrajectory(float jumpForce, float angle, float velocity)
    {
        List<Vector3> points = new List<Vector3>();
        float timeMax = (2 * jumpForce) / gravity;
        float timeStep = timeMax / numPoints;
        float velocityX = velocity * Mathf.Cos(angle);
        float velocityY = velocity * Mathf.Sin(angle);

        float x = 0;
        float y = 0;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep;
            x += velocityX * 0.01f * t;
            y += velocityY * 0.01f * t;

            points.Add(new Vector3(x, y, 0));

            velocityY += -gravity * 0.01f * t;
        }

        return points;
    }
    
    public void LaunchBird(float angleDegrees, float stretchLength)
    {
        float angle = angleDegrees * Mathf.Deg2Rad;
        float velocity = SpeedInitial(angle, stretchLength);
        float adjustedVelocity = velocity * (1 - slingshot.frictionShot + 0.1f);

        // applique la vitesse au rigidbody pour simuler la physique sur la trajectoire
        // - cela permet de laisser unity gérer la collision avec les autres objects
        float velocityX = useFriction ? adjustedVelocity * Mathf.Cos(angle) : velocity * Mathf.Cos(angle);
        float velocityY = useFriction ? adjustedVelocity * Mathf.Sin(angle) : velocity * Mathf.Sin(angle);
        rigidBody2D.velocity = new Vector2(velocityX, velocityY);
    }
    
    public void LaunchBirdJump(float angleDegrees, float velocity)
    {
        float angle = angleDegrees * Mathf.Deg2Rad;
        float adjustedVelocity = velocity * (1 - slingshot.frictionShot);

        // applique la vitesse au rigidbody pour simuler la physique sur la trajectoire
        // - cela permet de laisser unity gérer la collision avec les autres objects
        float velocityX = adjustedVelocity * Mathf.Cos(angle);
        float velocityY = adjustedVelocity * Mathf.Sin(angle);
        rigidBody2D.velocity = new Vector2(velocityX, velocityY);
    }
}