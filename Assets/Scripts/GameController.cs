﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
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

    public void UpdateAliveDino()
    {
        var aliveDino = GameObject.FindGameObjectsWithTag("PlayersDino");

        if (aliveDino == null)
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
        Invoke("UpdateAliveDino", 0.1f);
    }

    private void FixedUpdate()
    {
//        if()
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
    }
}