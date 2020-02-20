using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trading : MonoBehaviour
{
    public List<string> categoryNames;
    public GameObject categoryPrefab;

    private List<TradeCategory> tradeCategories;
    private int index = 0;

    private void Start()
    {
        tradeCategories = new List<TradeCategory>();
        foreach(string categoryName in categoryNames)
        {
            GameObject category = Instantiate(categoryPrefab, transform, false);
            TradeCategory tradeCategory = category.GetComponent<TradeCategory>();
            if (tradeCategory != null)
            {
                tradeCategory.categoryName = categoryName;
                tradeCategory.gameObject.SetActive(false);
            }
            else
            {
                Destroy(category);
            }

            tradeCategories.Add(tradeCategory);
        }

        tradeCategories[0].gameObject.SetActive(true);
    }
}
