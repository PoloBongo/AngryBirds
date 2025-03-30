using UnityEngine;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour
{
    public GameObject levelmenu;

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
}