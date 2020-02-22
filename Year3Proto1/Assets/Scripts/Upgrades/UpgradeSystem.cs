using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class UpgradeSystem : MonoBehaviour
{
    public GameObject categoryPrefab;
    public UpgradeCategoryEntity[] upgradeCategoryEntities;

    private List<UpgradeCategory> upgradeCategories;
    private int index = 0;
    private bool switchable = true;

    private void Start()
    {
        upgradeCategories = new List<UpgradeCategory>();

        for(int i = 0; i < upgradeCategoryEntities.Length; i++)
        {
            GameObject @object = Instantiate(categoryPrefab, transform, false);
            UpgradeCategory upgradeCategory = @object.GetComponent<UpgradeCategory>();
            UpgradeCategoryEntity upgradeCategoryEntity = upgradeCategoryEntities[i];

            upgradeCategories.Add(upgradeCategory.Initialise(
                upgradeCategoryEntity.upgradeResourceEntities,
                upgradeCategoryEntity.name
            ));
        }

        upgradeCategoryEntities = new UpgradeCategoryEntity[] { }; 
        if(upgradeCategories.Count > 0) upgradeCategories[0].gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && switchable)
        {
            UpgradeCategory upgradeCategory = upgradeCategories[index];

            if (index >= (upgradeCategories.Count - 1))
            {
                index = 0;
            }
            else
            {
                index++;
            }

            Switch(upgradeCategory, upgradeCategories[index], KeyCode.A);
        }

        if (Input.GetKeyDown(KeyCode.D) && switchable)
        {
            UpgradeCategory upgradeCategory = upgradeCategories[index];

            if (index <= 0)
            {
                index = (upgradeCategories.Count - 1);
            }
            else
            {
                index--;
            }

            Switch(upgradeCategory, upgradeCategories[index], KeyCode.D);
        }
    }

    public void Switch(UpgradeCategory upgradeCategory, UpgradeCategory nextTradeCategory, KeyCode keyCode)
    {
        switchable = false;
        float moveTime = 0.8f;

        float initialMove = 0.0f;
        float finalMove = 0.0f;

        if (keyCode == KeyCode.D)
        {
            initialMove = -(upgradeCategory.transform.localPosition.x * 2);
            finalMove = upgradeCategory.GetComponent<RectTransform>().rect.width * 2;
        }

        if (keyCode == KeyCode.A)
        {
            initialMove = (upgradeCategory.transform.localPosition.x * 2);
            finalMove = -upgradeCategory.GetComponent<RectTransform>().rect.width;
        }


        //Category Header
        DOTween.Sequence()

            .Play();


        //Category Items

        DOTween.Sequence()
                .Join(nextTradeCategory.transform.DOLocalMoveX(initialMove, 0.0f))
                .Join(upgradeCategory.transform.DOLocalMoveX(finalMove, moveTime))
                .Join(upgradeCategory.GetComponent<CanvasGroup>().DOFade(0.0f, moveTime))
                .Join(nextTradeCategory.transform.DOLocalMoveX(upgradeCategory.transform.localPosition.x, moveTime))
                .Join(nextTradeCategory.GetComponent<CanvasGroup>().DOFade(1.0f, moveTime))
                .OnPlay(() => nextTradeCategory.gameObject.SetActive(true))
                .OnComplete(() =>
                {
                    upgradeCategory.gameObject.SetActive(false);
                    switchable = true;
                }).Play();
    }
}
