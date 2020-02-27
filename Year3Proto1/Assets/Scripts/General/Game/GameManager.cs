using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;

public enum GameState
{
    TITLE,
    INGAME,
    INTERVAL,
    DEATH_SCREEN
};

public class GameManager : Singleton<GameManager>
{
    //This is one ugly game manager, but it works... for now!

    [Header("User Interface")]
    public GameHUD gameHUD;
    public GameWave gameWave;

    public GameObject popup;
    public GameObject skip;
    public GameObject waveStats;

    public GameObject hud;
    public GameObject trading;

    public GameObject blurVolume;
    private Volume ppvolume;

    [Header("Enemies")]
    public GameSpawner gameSpawner;
    public int remaining = 2;
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

    private GameState gameState = GameState.TITLE;
    private float time = 1.0f;
    private bool popupActive = false;

    private void Start()
    {
        ppvolume = Instantiate(blurVolume).GetComponent<Volume>();
        ppvolume.weight = 0.0f;

        hud.GetComponent<CanvasGroup>().alpha = 0.0f;
        waveStats.GetComponent<CanvasGroup>().alpha = 0.0f;
        skip.GetComponent<CanvasGroup>().alpha = 0.0f;

        playerControl = false;
        musicPlayer = FindObjectOfType<MusicPlayer>();
        AIChasingPlayer = new List<GameObject>();

        gameWave.wave = wave;
    }

    private void LateUpdate()
    {
        if (gameState == GameState.INGAME)
        {
            if ((aliveCraft.Count - 1) <= 0) Switch(gameState);
            gameHUD.Refresh(gameState, playerScrap, playerKills, aliveCraft.Count - 1, wave);
        }
    }

    private void Update()
    {

        if(gameState == GameState.INTERVAL)
        {
            if (time <= 0) Switch(gameState);
            time -= Time.deltaTime;
            gameHUD.Refresh(gameState, playerScrap, playerKills, (int) time, wave);

            if (Input.GetKeyDown(KeyCode.F))
            {
                trading.SetActive(trading.activeSelf ? false : true);
                waveStats.SetActive(trading.activeSelf ? false : true);
                playerControl = (trading.activeSelf ? false : true);

                DOTween.To(() => ppvolume.weight, x => ppvolume.weight = x, trading.activeSelf ? 1.0f : 0.0f, 0.1f).SetEase(Ease.InOutSine);
            }
            if(Input.GetKeyDown(KeyCode.E)) time = 0.0f;
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
                Popup();
                break;
            case GameState.INGAME:
                this.gameState = GameState.INTERVAL;
                gameWave.Refresh();
                remaining = 1;
                time = 60.0f;
                Popup();
                break;
            case GameState.TITLE:
                this.gameState = GameState.INGAME;
                gameSpawner.Spawn(remaining);
                time = 1.0f;

                hud.GetComponent<CanvasGroup>().DOKill(true);
                hud.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).SetEase(Ease.InOutSine);

                waveStats.GetComponent<CanvasGroup>().DOKill(true);
                waveStats.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).SetEase(Ease.InOutSine);

                skip.GetComponent<CanvasGroup>().DOKill(true);
                skip.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).SetEase(Ease.InOutSine);
                break;
        }
        Debug.Log("GameManager switched to state: " + gameState);
    }
    
    public bool GetPlayerControl()
    {
        return playerControl;
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
        float skipPosition = skip.transform.localPosition.x;

        if (popupActive)
        {
            wavePosition -= (waveStats.GetComponent<RectTransform>().rect.width + 64);
            popupPosition += popup.GetComponent<RectTransform>().rect.width;
            skipPosition += popup.GetComponent<RectTransform>().rect.width;
            popupActive = false;
        }
        else
        {
            wavePosition += (waveStats.GetComponent<RectTransform>().rect.width + 64);
            popupPosition -= popup.GetComponent<RectTransform>().rect.width;
            skipPosition -= popup.GetComponent<RectTransform>().rect.width;
            popupActive = true;
        }

        popup.transform.DOKill(true);
        popup.transform.DOLocalMoveX(popupPosition, 2.0f).SetEase(Ease.OutQuint);

        skip.transform.DOKill(true);
        skip.transform.DOLocalMoveX(skipPosition, 1.7f).SetEase(Ease.OutQuint);

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
