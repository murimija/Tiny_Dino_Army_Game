using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }

    private static int numOfAliveDino;

    public void ChangeNumOfAliveDino(int numOfChange)
    {
        numOfAliveDino += numOfChange;
        if (numOfAliveDino <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
    }

}
