using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Levels : MonoBehaviour
{
    public GameObject levelmenu;
    [SerializeField] private Sprite green;
    [SerializeField] private Sprite red;
    
    [Header("Debug/Options")]
    [SerializeField] private GameObject gravity;
    [SerializeField] private GameObject debug;
    [SerializeField] private GameObject friction;

    private void Start()
    {
        UseFriction(friction);
        EnableGravity(gravity);
        DebugDraw(debug);
    }

    public void Levelselection()
    {
        levelmenu.SetActive(true);
    }
    
    public void SwitchScene(string _name)
    {
        SceneManager.LoadSceneAsync(_name);
    }

    public void closeselection()
    {
        levelmenu.SetActive(false);
    }

    public void DebugDraw(GameObject _gameObject)
    {
        _gameObject.GetComponent<Image>().sprite = GameManager.GameManagerInstance.debugDraw switch
        {
            true => green,
            false => red
        };
    }
    
    public void EnableGravity(GameObject _gameObject)
    {
        _gameObject.GetComponent<Image>().sprite = GameManager.GameManagerInstance.enableGravity switch
        {
            true => green,
            false => red
        };
    }
    
    public void UseFriction(GameObject _gameObject)
    {
        _gameObject.GetComponent<Image>().sprite = GameManager.GameManagerInstance.useFriction switch
        {
            true => green,
            false => red
        };
    }
    
    public void DebugDrawBool()
    {
        GameManager.GameManagerInstance.debugDraw = !GameManager.GameManagerInstance.debugDraw;
    }
    
    public void EnableGravityBool()
    {
        GameManager.GameManagerInstance.enableGravity = !GameManager.GameManagerInstance.enableGravity;
    }
    
    public void UseFrictionBool()
    {
        GameManager.GameManagerInstance.useFriction = !GameManager.GameManagerInstance.useFriction;
    }
}