using System.Collections.Generic;
using UnityEngine;

public class RenderTrajectory : MonoBehaviour
{
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private Transform player;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float interval = 0.25f;

    private List<GameObject> listPoints = new List<GameObject>();
    private int currentIndex = 0;
    private float lastPointTime;

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject point = Instantiate(pointPrefab, parent);
            point.SetActive(false);
            listPoints.Add(point);
        }
    }

    public void ResetPoints()
    {
        if (listPoints.Count < 1) return;

        foreach (var point in listPoints)
        {
            point.SetActive(false);
        }
    }
    
    public void SwitchBirdTarget(Transform _player)
    {
        player = _player;
    }

    private void Update()
    {
        if (!slingshot.CanResetCamera) return;
        
        if (Time.time - lastPointTime > interval)
        {
            ShowPoint();
            lastPointTime = Time.time;
        }
    }

    private void ShowPoint()
    {
        GameObject point = listPoints[currentIndex];
        point.transform.position = player.position;
        point.SetActive(true);

        currentIndex = (currentIndex + 1) % poolSize;
    }
}