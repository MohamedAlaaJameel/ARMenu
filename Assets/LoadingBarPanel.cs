using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarPanel : MonoBehaviour
{
    public TMPro.TextMeshProUGUI debugText;
    public Image imageToFill;
    public float fillSpeed = 0.5f; 

    private IEnumerator FillToAmount(float targetFillAmount)
    {
        float currentFillAmount = imageToFill.fillAmount;

        while (Mathf.Abs(imageToFill.fillAmount - targetFillAmount) > 0.01f)
        {
            currentFillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, fillSpeed * Time.deltaTime);
            imageToFill.fillAmount = currentFillAmount;

            yield return null;
        }

        imageToFill.fillAmount = targetFillAmount;
        if (targetFillAmount==1)
        {
            Hide();
        }
    }

    // Function to start the coroutine with the desired fill amount
    public void StartFillCoroutine(string debugString,float targetFillAmount=0)
    {
        debugText.text = debugString;
        if (targetFillAmount!=0)
        {
            StopAllCoroutines(); // Stop any existing fill coroutines
            StartCoroutine(FillToAmount(targetFillAmount)); // Start the fill coroutine
        }
       
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
}
