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

    private void Update()
    {
        Check();
    }

    public void Check()
    {

        if (Input.GetKeyDown("f"))
        {
            if(trading.activeSelf)
            {
                trading.SetActive(false);
            }
            else
            {
                trading.SetActive(true);
            }
        }
  
    }
}
