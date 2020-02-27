using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;

public class TitleScreen : MonoBehaviour
{
    public bool showScreen;
    private bool screenVisible;
    private bool hideScreen;
    private float timer;
    public float startDelay = 2.0f;

    private bool gameStarted;

    private CanvasGroup canwas;
    private GameObject anykeyPrompt;
    private CanvasGroup canvasAnyKey;

    public GameObject blurVolume;
    private Volume ppvolume;

    private void Awake()
    {
        ppvolume = Instantiate(blurVolume).GetComponent<Volume>();
    }

    void Start()
    {
        canwas = GetComponent<CanvasGroup>();
        anykeyPrompt = transform.Find("PressAnyKey").gameObject;
        canvasAnyKey = anykeyPrompt.GetComponent<CanvasGroup>();
    }


    void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= 3.5f && !hideScreen)
        {
            showScreen = true;
        }

        if (timer >= startDelay)
        {
            if (Input.GetKeyDown(KeyCode.W) && showScreen)
            {
                showScreen = false;
                hideScreen = true;
                anykeyPrompt.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.0f), 0.33f, 1, 1.0f);
            }

            canvasAnyKey.alpha = 1.0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showScreen = !showScreen;
        }

        if (showScreen && !screenVisible)
        {
            Show();
            screenVisible = true;
        }

        if (!showScreen && screenVisible)
        {
            Hide();
            screenVisible = false;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            DOTween.To(() => ppvolume.weight, x => ppvolume.weight = x, 1.0f, 0.2f).SetEase(Ease.InOutSine);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DOTween.To(() => ppvolume.weight, x => ppvolume.weight = x, 0.0f, 0.2f).SetEase(Ease.InOutSine);
        }
    }

    private void Show()
    {
        canwas.DOKill(true);
        canwas.DOFade(1.0f, 0.5f);
        DOTween.To(() => ppvolume.weight, x => ppvolume.weight = x, 1.0f, 0.2f).SetEase(Ease.InOutSine);

        GameManager.Instance.playerControl = false;
        GameManager.Instance.hud.GetComponent<CanvasGroup>().DOKill(true);
        GameManager.Instance.hud.GetComponent<CanvasGroup>().DOFade(0.0f, 0.2f).SetEase(Ease.InOutSine);
        GameManager.Instance.waveStats.GetComponent<CanvasGroup>().DOKill(true);
        GameManager.Instance.waveStats.GetComponent<CanvasGroup>().DOFade(0.0f, 0.2f).SetEase(Ease.InOutSine);


        Debug.Log("Showing Title Screen");
    }

    private void Hide()
    {
        canwas.DOKill(true);
        canwas.DOFade(0.0f, 0.5f);

        DOTween.To(() => ppvolume.weight, x => ppvolume.weight = x, 0.0f, 0.2f).SetEase(Ease.InOutSine);

        GameManager.Instance.playerControl = true;

        if (!gameStarted)
        {
            GameManager.Instance.Switch(GameState.TITLE);
        }
        else
        {
            GameManager.Instance.hud.GetComponent<CanvasGroup>().DOKill(true);
            GameManager.Instance.hud.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).SetEase(Ease.InOutSine);
            GameManager.Instance.waveStats.GetComponent<CanvasGroup>().DOKill(true);
            GameManager.Instance.waveStats.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).SetEase(Ease.InOutSine);
        }

        gameStarted = true;

        Debug.Log("Hiding Title Screen");
    }
}
