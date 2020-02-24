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
    public GameObject[] enemies;
    public Transform enemiesParent;

    private GameState gameState = GameState.GRACE_PERIOD;
    private float time = 5.0f;
    private int waves = 1;
    private int enemiesRemaining = 10;
    private bool popupActive = true;

    private void Update()
    {
        //if (time <= 0)
       // {
        //    Popup();
        //    Check(gameState);
        //}

        time -= Time.deltaTime;

        if (gameState == GameState.GRACE_PERIOD)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                trading.SetActive(trading.activeSelf ? false : true);
                popup.GetComponentInChildren<Image>().transform.DOKill(true);
                popup.GetComponentInChildren<Image>().transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.0f), 0.33f, 1, 1.0f);
            }
        }
    }

    public void Check(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GRACE_PERIOD:
                this.gameState = GameState.INGAME;
                time = 5.0f;
                break;
            case GameState.INGAME:
                this.gameState = GameState.GRACE_PERIOD;
                time = 5.0f;
                break;
        }
    }

    public void Spawn(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //Instantiate(enemies[Random.Range(0, enemies.Length)], enemiesParent);
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
