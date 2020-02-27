using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameState
{
    INGAME,
    INTERVAL
};

public class GameManager : Singleton<GameManager>
{
    [Header("User Interface")]
    public GameHUD gameHUD;
    public GameWave gameWave;

    public GameObject popup;
    public GameObject waveStats;

    public GameObject hud;
    public GameObject trading;

    [Header("Enemies")]
    public GameSpawner gameSpawner;
    public int remaining = 0;
    public int wave = 1;

    public List<HovercraftShared> aliveCraft;
    private List<GameObject> AIChasingPlayer;


    [Header("Player")]
    public int playerScrap;
    public int playerKills;
    public bool playerControl;
    public bool playerInCombat;
    public bool playerGoingFast;
    MusicPlayer musicPlayer;

    private GameState gameState = GameState.INGAME;
    private float time = 0.0f;
    private bool popupActive = true;

    private void Start()
    {
        playerControl = false;
        musicPlayer = FindObjectOfType<MusicPlayer>();
        AIChasingPlayer = new List<GameObject>();
        Switch(gameState);

        gameWave.wave = wave;
    }

    private void Update()
    {
        if (time <= 0 || remaining <= 0) Switch(gameState);

        if(gameState == GameState.INTERVAL)
        {
            time -= Time.deltaTime;
            gameHUD.Refresh(gameState, playerScrap, playerKills, (int) time, wave);

            if (Input.GetKeyDown(KeyCode.F))
            {
                trading.SetActive(trading.activeSelf ? false : true);
                waveStats.SetActive(trading.activeSelf ? false : true);
                popup.GetComponentInChildren<Image>().transform.DOKill(true);
                popup.GetComponentInChildren<Image>().transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.0f), 0.33f, 1, 1.0f);
            }
        } 

        else if(gameState == GameState.INGAME)
        {
            gameHUD.Refresh(gameState, playerScrap, playerKills, remaining, wave);
        }

        if (AIChasingPlayer.Count > 0)
        {
            SetPlayerInCombat(true);
        }
        else
        {
            SetPlayerInCombat(false);
        }
    }

    public void Switch(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.INTERVAL:
                this.gameState = GameState.INGAME;
                wave += 1;
                remaining = (wave * 10 / 2);
                gameSpawner.Spawn(remaining);
                time = 1.0f;
                break;
            case GameState.INGAME:
                this.gameState = GameState.INTERVAL;
                gameWave.Refresh();
                remaining = 1;
                time = 10.0f;
                break;
        }

        Popup();
        Debug.Log("GameManager switched to state: " + gameState);
    }

    public void SetPlayerGoingFast(bool _playerGoingFast)
    {
        if (playerGoingFast != _playerGoingFast)
        {
            musicPlayer.ToggleSpeed();
        }
    }

    public void SetPlayerInCombat(bool _playerInCombat)
    {
        if (playerInCombat != _playerInCombat)
        {
            musicPlayer.ToggleCombat();
        }
    }

    public void Popup()
    {
        float popupPosition = popup.transform.localPosition.x;
        float wavePosition = waveStats.transform.localPosition.x;

        if (popupActive)
        {
            wavePosition += (waveStats.GetComponent<RectTransform>().rect.width + 64);
            popupPosition -= popup.GetComponent<RectTransform>().rect.width;
            popupActive = false;
        }
        else
        {
            wavePosition -= (waveStats.GetComponent<RectTransform>().rect.width + 64);
            popupPosition += popup.GetComponent<RectTransform>().rect.width;
            popupActive = true;
        }

        popup.transform.DOKill(true);
        popup.transform.DOLocalMoveX(popupPosition, 1.5f).SetEase(Ease.OutQuint);

        waveStats.transform.DOKill(true);
        waveStats.transform.DOLocalMoveX(wavePosition, 1.5f).SetEase(Ease.OutQuint);

        trading.SetActive(false);
    }

    public void AddAIChasing(GameObject _AI)
    {
        if (!AIChasingPlayer.Contains(_AI))
        {
            AIChasingPlayer.Add(_AI);
        }
    }

    public void RemoveAIChasing(GameObject _AI)
    {
        if (AIChasingPlayer.Contains(_AI))
        {
            AIChasingPlayer.Remove(_AI);
        }
    }

    public void AddAlive(HovercraftShared _AI)
    {
        if (!aliveCraft.Contains(_AI))
        {
            aliveCraft.Add(_AI);
        }
    }

    public void RemoveAlive(HovercraftShared _AI)
    {
        if (aliveCraft.Contains(_AI))
        {
            aliveCraft.Remove(_AI);
        }
        // go through each AI and if their target is this AI's chassis, set them to wander
        foreach (HovercraftShared hovercraftShared in aliveCraft)
        {
            if (hovercraftShared.controller == HovercraftShared.ControllerType.AIController)
            {
                if (hovercraftShared.name.Contains("Scimitar"))
                {
                    ScimitarAIController aiScript = hovercraftShared.GetComponent<ScimitarAIController>();
                    // if the AI's target is the AI that just died, set that AI to wander
                    if (aiScript.stateO.target == _AI.GetChassis())
                    {
                        aiScript.ChangeState(ScimitarAIController.HovercraftAIState.Wander);
                    }
                }
                else if (hovercraftShared.name.Contains("Tortoise"))
                {
                    TortoiseAIController aiScript = hovercraftShared.GetComponent<TortoiseAIController>();
                    // if the AI's target is the AI that just died, set that AI to wander
                    if (aiScript.stateO.target == _AI.GetChassis())
                    {
                        aiScript.ChangeState(TortoiseAIController.HovercraftAIState.Wander);
                    }
                }
            }
        }
    }
}
