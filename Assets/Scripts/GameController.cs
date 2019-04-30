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

    private void Start()
    {
        sceneChanger = SceneChanger.instance;
    }

    public void UpdateAliveDino()
    {
        aliveDino = GameObject.FindGameObjectsWithTag("PlayersDino");
        Debug.Log(aliveDino);

        if (aliveDino.Length == 0)
        {
            GameOver();
            return;
        }

        damageOfArmy = 0;
        foreach (var dino in aliveDino)
        {
            damageOfArmy += dino.GetComponent<DinoController>().damage;
        }

        numOfDinosText.text = aliveDino.Length.ToString();
        damageOfDinosText.text = damageOfArmy.ToString();
    }

    public void DinoDeathReport()
    {
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
}