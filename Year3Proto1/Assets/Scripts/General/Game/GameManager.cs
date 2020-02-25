using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    INGAME,
    GRACE_PERIOD
};

public class GameManager : Singleton<GameManager>
{
    [Header("User Interface")]
    public TMP_Text header;
    public TMP_Text footer;

    public GameObject popup;
    public GameObject hud;
    public GameObject trading;

    [Header("Enemies")]
    public GameSpawner gameSpawner;
    public int remaining = 10;
    public int wave = 1;


    [Header("Player")]
    public bool playerControl;
    public bool playerInCombat;
    public bool playerGoingFast;
    MusicPlayer musicPlayer;

    private GameState gameState = GameState.GRACE_PERIOD;
    private float time = 5.0f;
    private bool popupActive = true;

    private void Update()
    {
        if (remaining <= 0 && time <= 0) Switch(gameState);

        if (gameState == GameState.GRACE_PERIOD)
        {
            time -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.F))
            {
                trading.SetActive(trading.activeSelf ? false : true);
                popup.GetComponentInChildren<Image>().transform.DOKill(true);
                popup.GetComponentInChildren<Image>().transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.0f), 0.33f, 1, 1.0f);
            }
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

    private void Start()
    {
        playerControl = false;
        musicPlayer = FindObjectOfType<MusicPlayer>();
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
            position = position - popup.GetComponent<RectTransform>().rect.width;
            popupActive = false;
        }
        else
        {
            position = position + popup.GetComponent<RectTransform>().rect.width;
            popupActive = true;
        }

        popup.transform.DOKill(true);
        popup.transform.DOLocalMoveX(position, 1.5f).SetEase(Ease.OutQuint);

        trading.SetActive(false);
    }
}
