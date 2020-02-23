using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameState
{
    INGAME,
    GRACE_PERIOD
};

public class GameManager : Singleton<GameManager>
{
    public TMP_Text header;
    public TMP_Text footer;

    private GameState gameState = GameState.INGAME;
    private float time = 0.0f;
    private int waves = 1;
    private int enemiesRemaining;

    private void Update()
    {
        if (enemiesRemaining <= 0 && gameState == GameState.INGAME) Check(gameState);
        if (time <= 0 && gameState == GameState.GRACE_PERIOD) Check(gameState);
        if (gameState == GameState.GRACE_PERIOD) time -= Time.deltaTime;
    }

    public void Check(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.GRACE_PERIOD:
                waves += 1;
                header.text = "Grace Peroid";
                time = 120.0f;
                break;
            case GameState.INGAME:
                header.text = "Wave " + waves;
                time = 120.0f;
                break;
        }
    }
}
