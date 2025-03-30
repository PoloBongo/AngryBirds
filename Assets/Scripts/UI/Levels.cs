using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour
{
    public GameObject levelmenu;
    public GameObject level_1;
    public GameObject level_2;
    

    public void Levelselection()
    {
        levelmenu.SetActive(true);
    }
    
    public void Level1()
    {
        level_1.SetActive(true);
        level_2.SetActive(false);
    }
    
    public void Level2()
    {
        level_2.SetActive(true);
        level_1.SetActive(false);
    }

    public void closeselection()
    {
        levelmenu.SetActive(false);
    }
}