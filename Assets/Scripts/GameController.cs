using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }

    [SerializeField] private Text numOfDinosText;
    [SerializeField] private Text damageOfDinosText;
    private int damageOfArmy;

    private SceneChanger sceneChanger;
    private GameObject[] aliveDino;

    public List<GameObject> listOfEggs = new List<GameObject>();
    public List<GameObject> listOfDino = new List<GameObject>();
    public List<GameObject> listOfEnemy = new List<GameObject>();

    private void Start()
    {
        sceneChanger = SceneChanger.instance;
        
    }

    public void UpdateAliveDino()
    {
        if (listOfDino.Count == 0)
        {
            GameOver();
            return;
        }

        damageOfArmy = 0;

/*        for (var i = 0; i < listOfDino.Count; i++)
        {
            var dino = aliveDino[i];
            damageOfArmy += dino.GetComponent<DinoController>().damage;
        }*/

        numOfDinosText.text = listOfDino.Count.ToString();
//        damageOfDinosText.text = damageOfArmy.ToString();
    }

    public void DinoDeathReport(GameObject dinoToRemove)
    {
        listOfDino.Remove(dinoToRemove);
        Invoke(nameof(UpdateAliveDino), 0.1f);
    }

    private void GoToGameOverScene()
    {
        sceneChanger.GoToScene("GameOver");
        Time.timeScale = 1f;
    }

    private void GameOver()
    {
        Time.timeScale = 0.5f;
        Invoke(nameof(GoToGameOverScene), 1f);
        Debug.Log("Game Over!");
    }

    public void AddDinoToList(GameObject dinoToAdd)
    {
        listOfDino.Add(dinoToAdd);
    }
    
}