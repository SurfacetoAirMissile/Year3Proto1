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
    public List<HovercraftShared> aliveCraft;
    List<GameObject> AIChasingPlayer;
    MusicPlayer musicPlayer;
    private void Start()
    {
        playerControl = false;
        musicPlayer = FindObjectOfType<MusicPlayer>();
        AIChasingPlayer = new List<GameObject>();
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
        if (AIChasingPlayer.Count > 0)
        {
            SetPlayerInCombat(true);
        }
        else
        {
            SetPlayerInCombat(false);
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
