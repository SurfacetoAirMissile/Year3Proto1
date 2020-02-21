using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TradeCategory : MonoBehaviour
{
    public string categoryName;
    public Image outline, outline2;
    public Image arrow;
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

            Switch();
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (index <= 0)
            {
                index = (resourceEntities.Count - 1);
            } 
            else
            {
                index--;
            }

            Switch();

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

    public void Switch()
    {
        float moveTime = 0.3f;

        /* FIRST ANIMATION
         
        DOTween.Sequence()
          .Join(outline2.transform.DOLocalMoveY(resourceEntities[index].transform.localPosition.y, 0.0f))
          .Append(arrow.transform.DOLocalMoveY(resourceEntities[index].transform.localPosition.y, moveTime).SetEase(Ease.InOutQuint))
          .Join(outline2.DOFade(1.0f, moveTime))
          .Join(outline.DOFade(0.0f, moveTime))
          .Append(outline.transform.DOLocalMoveY(resourceEntities[index].transform.localPosition.y, 0.0f))
          .Join(outline.DOFade(1.0f, 0.0f))
          .Join(outline2.DOFade(0.0f, 0.0f)).Play();

        */

        /* SECOND ANIMATION 
        
        DOTween.Sequence()
          .Join(outline2.transform.DOLocalMoveY(resourceEntities[index].transform.localPosition.y, 0.0f))
          .Append(arrow.transform.DOLocalMoveY(resourceEntities[index].transform.localPosition.y, moveTime).SetEase(Ease.InOutQuint))
          .Join(outline2.DOFade(1.0f, moveTime))
          .Join(outline.DOFade(0.0f, moveTime))
          .Append(outline.transform.DOLocalMoveY(resourceEntities[index].transform.localPosition.y, 0.0f))
          .Join(outline.DOFade(1.0f, 0.0f))
          .Join(outline2.DOFade(0.0f, 0.0f)).Play();

        */
    }
}
