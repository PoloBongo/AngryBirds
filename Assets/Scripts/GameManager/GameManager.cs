using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManagerInstance;
    private int totalBirds;
    private int numbersOfTry;
    private int actualBirdsKill;

    [Header("Popup")]
    [SerializeField] private GameObject loosePopup;
    [SerializeField] private GameObject victoryPopup;
    
    public bool canPlay {get; set;}
    private void Awake()
    {
        if (GameManagerInstance != null && GameManagerInstance != this)
        {
            Destroy(this);
        }
        else
        {
            GameManagerInstance = this;
            DontDestroyOnLoad(GameManagerInstance);
        }
    }

    private void Start()
    {
        totalBirds = 3;
        numbersOfTry = 3;
        actualBirdsKill = 0;

        if (!loosePopup) FoundLoosePopup();
        if (!victoryPopup) FoundVictoryPopup();
    }

    private void FoundLoosePopup()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        if (loosePopup) return;
        
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Loose"))
            {
                loosePopup = obj;
                break;
            }
        }
    }
    
    private void FoundVictoryPopup()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        if (victoryPopup) return;

        GameObject[] allObjectsVictory = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjectsVictory)
        {
            if (obj.CompareTag("Victory"))
            {
                victoryPopup = obj;
                break;
            }
        }
    }

    public void CheckLoose()
    {
        if (!loosePopup) FoundLoosePopup();
        if (!victoryPopup) FoundVictoryPopup();
        
        numbersOfTry--;
        if (numbersOfTry <= 0 && actualBirdsKill != totalBirds)
        {
            loosePopup.SetActive(true);
            ResetStats();
        }

        if (actualBirdsKill >= totalBirds)
        {
            victoryPopup.SetActive(true);
            ResetStats();
        }
    }

    public void AddBirdKill()
    {
        actualBirdsKill++;
    }

    private void ResetStats()
    {
        totalBirds = 3;
        numbersOfTry = 3;
        actualBirdsKill = 0;
    }
}
