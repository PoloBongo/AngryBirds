using System.Collections.Generic;
using UnityEngine;

public class ManageBirds : MonoBehaviour
{
    [SerializeField] private List<BirdTrajectory> birds;
    public int Index { get; set; }
    
    public List<BirdTrajectory> Birds { get => birds; set => birds = value; }

    private void Start()
    {
        Index = 0;
        
        ManageBirdsVisibility();
    }

    public void ManageBirdsVisibility()
    {
        for (int i = 0; i < birds.Count; i++)
        {
            birds[i].enabled = Index == i;
        }
    }
}
