using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering.HighDefinition;


public class UpgradeSystem : MonoBehaviour
{
    [Header("Header")]
    public TMP_Text title1;
    public TMP_Text title2;
    public TMP_Text title3;

    public Transform dotCanvas;
    public Transform categoryContainer;
    public Image dot;

    [Header("Information")]
    public UpgradeInformation upgradeInformation;

    [Header("Container")]
    public GameObject categoryPrefab;
    public UpgradeCategoryEntity[] upgradeCategoryEntities;

    private List<UpgradeCategory> upgradeCategories;
    private List<Image> dots;
    private int index = 0;
    private bool switchable = true;

    private void Start()
    {
        upgradeCategories = new List<UpgradeCategory>();
        dots = new List<Image>();

        for(int i = 0; i < upgradeCategoryEntities.Length; i++)
        {
            dots.Add(Instantiate(dot, dotCanvas, false));
            dots[i].color = new Color(255, 255, 255, 0.5f);


            GameObject @object = Instantiate(categoryPrefab, categoryContainer, false);
            UpgradeCategory upgradeCategory = @object.GetComponent<UpgradeCategory>();
            UpgradeCategoryEntity upgradeCategoryEntity = upgradeCategoryEntities[i];

            upgradeCategories.Add(upgradeCategory.Initialise(
                upgradeCategoryEntity.upgradeResourceEntities,
                upgradeCategoryEntity.name
            ));
        }

        if (upgradeCategories.Count > index)
        {
            upgradeCategories[index].gameObject.SetActive(true);
            dots[index].color = new Color(255, 255, 255, 1.0f);
        }
    }

    private void Update()
    {
        upgradeInformation.Refresh(upgradeCategories[index].GetUpgradeResource().GetEntity());

  
            if (Input.GetKeyDown(KeyCode.A) && switchable)
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

                Switch(upgradeCategory, upgradeCategories[index], KeyCode.A);
            }

            if (Input.GetKeyDown(KeyCode.D) && switchable)
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

                Switch(upgradeCategory, upgradeCategories[index], KeyCode.D);
            }
    }

    public void Switch(UpgradeCategory upgradeCategory, UpgradeCategory nextUpgradeCategory, KeyCode keyCode)
    {

        switchable = false;
        float moveTime = 0.3f;

        float initialMove = 0.0f;
        float finalMove = 0.0f;

        if (keyCode == KeyCode.D)
        {
            initialMove = -(upgradeCategory.transform.position.x * 4.0f);
            finalMove = upgradeCategory.GetComponent<RectTransform>().rect.width;
        }

        if (keyCode == KeyCode.A)
        {
            initialMove = (upgradeCategory.transform.position.x * 2.0f);
            finalMove = -upgradeCategory.GetComponent<RectTransform>().rect.width * 2.0f;
        }

        //TODO: Title Switch Category Header
        DOTween.Sequence()
            .Play();

        //Category Items

        DOTween.Sequence()
                .Join(nextUpgradeCategory.transform.DOLocalMoveX(initialMove, 0.0f).SetEase(Ease.OutQuint))
                .Join(upgradeCategory.transform.DOLocalMoveX(finalMove, moveTime).SetEase(Ease.OutQuint))
                .Join(upgradeCategory.GetComponent<CanvasGroup>().DOFade(0.0f, moveTime))
                .Join(nextUpgradeCategory.transform.DOLocalMoveX(upgradeCategory.transform.localPosition.x, moveTime).SetEase(Ease.OutQuint))
                .Join(nextUpgradeCategory.GetComponent<CanvasGroup>().DOFade(1.0f, moveTime))
                .OnPlay(() => {
                    nextUpgradeCategory.gameObject.SetActive(true);
                    for (int i = 0; i < dots.Count; i++) dots[i].color = new Color(255, 255, 255, 0.5f);
                    dots[index].color = new Color(255, 255, 255, 1.0f);
                })
                .OnComplete(() =>
                {
                    upgradeCategory.gameObject.SetActive(false);
                    switchable = true;
                }).Play();
    }
}
