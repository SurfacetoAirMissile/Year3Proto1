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
    public GameObject trading;
    public bool playerControl;
    public bool playerInCombat;
    public bool playerGoingFast;
    HovercraftShared[] hovercraftList;
    List<GameObject> AIChasingPlayer;
    MusicPlayer musicPlayer;
    private void Start()
    {
        playerControl = false;
        musicPlayer = FindObjectOfType<MusicPlayer>();
        AIChasingPlayer = new List<GameObject>();
        //RefreshHovercraftList();
    }

    private void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            if (trading.activeSelf)
            {
                trading.SetActive(false);
                playerControl = true;
            }
            else
            {
                trading.SetActive(true);
                playerControl = false;
            }
        }
        if (hovercraftList.Length > 0)
        {
            foreach (HovercraftShared hovercraft in hovercraftList)
            {

            }
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

    public void RefreshHovercraftList()
    {
        hovercraftList = FindObjectsOfType<HovercraftShared>();
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
}
