using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UpgradeCategory : MonoBehaviour
{
    //Prefabrication
    public GameObject resource;
    public Image outline, outline2;
    public Image arrow;
    public Transform parent;

    //Local Variables
    private string categoryName;
    private List<UpgradeResource> upgradeResources;
    private int index = 0;

    public UpgradeCategory Initialise(UpgradeResourceEntity[] upgradeResourceEntities, string name)
    {
        upgradeResources = new List<UpgradeResource>();
        foreach (UpgradeResourceEntity upgradeResourceEntity in upgradeResourceEntities)
        {
            GameObject prefabResource = Instantiate(resource, parent, false);
            UpgradeResource upgradeResource = prefabResource.GetComponent<UpgradeResource>();
            if (upgradeResource != null)
            {
                upgradeResource.itemName.text = upgradeResourceEntity.displayName;
                upgradeResource.imageIcon.sprite = upgradeResourceEntity.icon;
                upgradeResource.cost.text = upgradeResourceEntity.cost.ToString();
                upgradeResource.upgradeType = upgradeResourceEntity.upgradeType;
                upgradeResources.Add(upgradeResource);
            }
        }

        categoryName = name;
        gameObject.SetActive(false);

        return this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (index >= (upgradeResources.Count - 1))
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
                index = (upgradeResources.Count - 1);
            }
            else
            {
                index--;
            }

            Switch();

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpgradeManager.Instance.Execute(upgradeResources[index].upgradeType);
        }
    }

    public void Switch()
    {
        float moveTime = 0.4f;

        DOTween.Sequence()
          .Join(outline2.transform.DOLocalMoveY(upgradeResources[index].transform.localPosition.y, 0.0f))
          .Append(arrow.transform.DOLocalMoveY(upgradeResources[index].transform.localPosition.y, moveTime).SetEase(Ease.InOutQuint))
          .Join(outline2.DOFade(1.0f, moveTime))
          .Join(outline.DOFade(0.0f, moveTime))
          .Append(outline.transform.DOLocalMoveY(upgradeResources[index].transform.localPosition.y, 0.0f))
          .Join(outline.DOFade(1.0f, 0.0f))
          .Join(outline2.DOFade(0.0f, 0.0f)).Play();
    }

    public string GetName()
    {
        return categoryName;
    }
}
