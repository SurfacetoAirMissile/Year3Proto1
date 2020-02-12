using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarrelUI : MonoBehaviour
{
    public TMP_Text tmpText;

    private void Update()
    {
        transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }
}
