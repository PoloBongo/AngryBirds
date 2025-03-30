using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeAndPause : MonoBehaviour
{
   public void Home()
   {
      SceneManager.LoadScene(0);
   }
}
