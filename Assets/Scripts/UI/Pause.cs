using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseCanvas; // Le canvas de pause
    public Button resumeButton; // Le bouton pour reprendre le jeu
    public Button quitButton; // Le bouton pour quitter le jeu
    [SerializeField] private bool showInfo;

    private bool isPaused = false;

    public void PauseMenu()
    {
        if (isPaused)
        {
            // Si le jeu est déjà en pause, reprendre le jeu
            Time.timeScale = 1f;
            pauseCanvas.SetActive(false); // Désactiver le canvas de pause
        }
        else
        {
            // Si le jeu n'est pas en pause, mettre en pause
            Time.timeScale = 0f;
            pauseCanvas.SetActive(true); // Activer le canvas de pause
        }

        isPaused = !isPaused;
    }

    private void Start()
    {
        // Initialiser les boutons pour reprendre ou quitter
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void ResumeGame()
    {
        // Reprendre le jeu
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        isPaused = false;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    public void ShowInfo(GameObject info)
    {
        showInfo = !showInfo;
        info.SetActive(showInfo);
    }
}