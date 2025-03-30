using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeAndPause : MonoBehaviour
{
   public void Home()
   {
      SceneManager.LoadScene(0);
   }
}
