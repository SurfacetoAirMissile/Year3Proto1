using DG.Tweening;
using TMPro;
using UnityEngine;

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

    private GameState gameState = GameState.INGAME;
    private float time = 5.0f;
    private int waves = 1;
    private int enemiesRemaining = 10;
    private bool popupActive = true;

    private void Update()
    {
        if (time <= 0)
        {
            Popup();
            Check(gameState);
        }

        time -= Time.deltaTime;

        if (gameState == GameState.GRACE_PERIOD)
        {

            if (Input.GetKeyDown(KeyCode.F)) trading.SetActive(trading.activeSelf ? false : true);
        }
    }

    public void Check(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GRACE_PERIOD:
                trading.SetActive(trading.activeSelf ? false : true);
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

        DOTween.Sequence()
            .Join(popup.transform.DOLocalMoveX(position, 3.0f))
            .SetEase(Ease.OutQuint);
    }
}
