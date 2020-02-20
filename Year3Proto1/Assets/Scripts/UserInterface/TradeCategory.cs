using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TradeCategory : MonoBehaviour
{
    public string categoryName;
    public Image image;
    public Transform parent;

    private List<ResourceEntity> resourceEntities;
    private int index = 0;
    private int playerMoney = 100;

    private void Start()
    {
        resourceEntities = new List<ResourceEntity>();

        foreach (ResourceEntity resourceEntity in Resources.LoadAll<ResourceEntity>("Trading/" + categoryName))
        {
            resourceEntities.Add(Instantiate(resourceEntity, parent, false));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (index >= (resourceEntities.Count - 1))
            {
                index = 0;
            }
            else
            {
                index++;
            }

            image.transform.DOLocalMoveY(resourceEntities[index].transform.localPosition.y, 0.2f).SetEase(Ease.OutQuint);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (index <= 0)
            {
                index = (resourceEntities.Count - 1);
            } 
            else
            {
                index--;
            }

            image.transform.DOLocalMoveY(resourceEntities[index].transform.localPosition.y, 0.2f).SetEase(Ease.OutQuint);

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(playerMoney >= resourceEntities[index].price) resourceEntities[index].unityEvent.Invoke();
        }
    }

    public ResourceEntity GetCurrentResource()
    {
        return resourceEntities[index];
    }
}
