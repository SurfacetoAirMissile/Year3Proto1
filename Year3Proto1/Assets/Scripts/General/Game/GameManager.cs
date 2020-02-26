using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameState
{
    INGAME,
    GRACE_PERIOD
};

public class GameManager : Singleton<GameManager>
{
    [Header("User Interface")]
    public GameHUD gameHUD;
    public GameWave gameWave;

    public GameObject popup;
    public GameObject hud;
    public GameObject trading;

    [Header("Enemies")]
    public GameSpawner gameSpawner;
    public int remaining = 10;
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
    private float time = 5.0f;
    private bool popupActive = true;

    private void Start()
    {
        playerControl = false;
        musicPlayer = FindObjectOfType<MusicPlayer>();
        AIChasingPlayer = new List<GameObject>();
    }


    private void Update()
    {
        if (remaining <= 0 && time <= 0) Switch(gameState);

        switch(gameState)
        {
            case GameState.GRACE_PERIOD:
                {
                    time -= Time.deltaTime;

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        trading.SetActive(trading.activeSelf ? false : true);
                        popup.GetComponentInChildren<Image>().transform.DOKill(true);
                        popup.GetComponentInChildren<Image>().transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.0f), 0.33f, 1, 1.0f);
                    }
                    gameHUD.Refresh(gameState, playerScrap, playerKills, gameWave.GetTime(), wave);
                }
                break;
            case GameState.INGAME:
                gameHUD.Refresh(gameState, playerScrap, playerKills, gameWave.GetRemaining(), wave);
                break;
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
            case GameState.GRACE_PERIOD:
                this.gameState = GameState.INGAME;
                remaining = wave * 10;
                gameSpawner.Spawn(remaining);
                time = 120.0f;
                break;
            case GameState.INGAME:
                this.gameState = GameState.GRACE_PERIOD;
                wave += 1;
                time = 1.0f;
                break;
        }
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
        float position = popup.transform.localPosition.x;

        if (popupActive)
        {
            position -= popup.GetComponent<RectTransform>().rect.width;
            popupActive = false;
        }
        else
        {
            position += popup.GetComponent<RectTransform>().rect.width;
            popupActive = true;
        }

        popup.transform.DOKill(true);
        popup.transform.DOLocalMoveX(position, 1.5f).SetEase(Ease.OutQuint);

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

    public void AddKill()
    {
        Debug.Log("works");
        gameWave.AddKill(1);
    }
}
