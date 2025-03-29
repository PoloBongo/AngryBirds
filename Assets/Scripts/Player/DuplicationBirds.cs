using System.Collections.Generic;
using UnityEngine;

public class DuplicationBirds : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private GameObject birdDuplication; 
    private readonly List<BirdTrajectory> birdsDuplication = new List<BirdTrajectory>();
    
    public List<BirdTrajectory> GetBirdsDuplication() { return birdsDuplication; }

    public void DrawMultipleTrajectories(BirdTrajectory bird)
    {
        birdsDuplication.Clear();
        
        float l1 = slingshot.powerShot;
        float alphaMax = Mathf.PI / 2;
        
        for (int i = 0; i <= 10; i++)
        {
            if (i == 0) continue;
            GameObject birdTrajectory = Instantiate(birdDuplication);
            birdTrajectory.transform.position = bird.transform.position;
            float alpha = Mathf.Lerp(0, alphaMax, i / 10f); // Génère les angles de 0 à π/2
            
            BirdTrajectory birdTrajectoryScript = birdTrajectory.GetComponent<BirdTrajectory>();
            birdTrajectoryScript.DrawTrajectoryWithLineRenderer(alpha * Mathf.Rad2Deg, l1, bird.GetUseFriction());
            if (birdTrajectoryScript) birdsDuplication.Add(birdTrajectoryScript);
        }
    }
}
