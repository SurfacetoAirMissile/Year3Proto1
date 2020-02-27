using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleScreen : MonoBehaviour
{
    public bool showScreen;
    private bool screenVisible;
    private bool hideScreen;
    private float timer;
    public float startDelay = 1.0f;

    private CanvasGroup canwas;
    private GameObject anykeyPrompt;
    private CanvasGroup canvasAnyKey;

    void Start()
    {
        canwas = GetComponent<CanvasGroup>();
        anykeyPrompt = transform.Find("PressAnyKey").gameObject;
        canvasAnyKey = anykeyPrompt.GetComponent<CanvasGroup>();
    }


    void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= 2.5f && !hideScreen)
        {
            showScreen = true;
        }

        if (timer >= startDelay)
        {
            if (Input.anyKeyDown && showScreen)
            {
                showScreen = false;
                hideScreen = true;
                GameManager.Instance.playerControl = true;
                anykeyPrompt.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.0f), 0.33f, 1, 1.0f);
            }

            canvasAnyKey.alpha = 1.0f;
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
    }

    private void Show()
    {
        canwas.DOKill(true);
        canwas.DOFade(1.0f, 0.5f);

        Debug.Log("Showing Title Screen");
    }

    private void Hide()
    {
        canwas.DOKill(true);
        canwas.DOFade(0.0f, 0.5f);

        Debug.Log("Hiding Title Screen");
    }
}
