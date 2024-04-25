using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionPanel : MonoBehaviour
{
    public TMPro.TextMeshProUGUI foodDescriptionTMP;
    public void SetDescription(string foodDescription)
    {
        transform.gameObject.SetActive(true);
        foodDescriptionTMP.text = foodDescription;
    }
    public void ReturnBTN()
    {
        transform.gameObject.SetActive(false);
    }
}
