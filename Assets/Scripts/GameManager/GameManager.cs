using System.Collections;
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
    
    private Coroutine checkCoroutine;
    
    public bool canPlay {get; set;}
    public bool debugDraw {get; set;}
    public bool enableGravity {get; set;}
    public bool useFriction {get; set;}
    
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
        
        enableGravity = true;
        debugDraw = false;
        useFriction = false;
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
        checkCoroutine ??= StartCoroutine(CheckLooseCoroutine());
    }

    private IEnumerator CheckLooseCoroutine()
    {
        if (!loosePopup) FoundLoosePopup();
        if (!victoryPopup) FoundVictoryPopup();
        
        numbersOfTry--;
        
        yield return new WaitForSeconds(1f);
        
        if (numbersOfTry <= 0 && actualBirdsKill != totalBirds)
        {
            loosePopup.SetActive(true);
            ResetStats();
        }
        else if (actualBirdsKill >= totalBirds)
        {
            victoryPopup.SetActive(true);
            ResetStats();
        }

        checkCoroutine = null;
    }

    public void AddBirdKill()
    {
        actualBirdsKill++;
    }
    
    public void SetActualBirdsKill(int _amount)
    {
        actualBirdsKill = _amount;
    }

    private void ResetStats()
    {
        totalBirds = 3;
        numbersOfTry = 3;
        actualBirdsKill = 0;
    }
}
