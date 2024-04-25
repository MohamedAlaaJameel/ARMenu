using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public bool debugActive;
    public TMPro.TextMeshProUGUI debugTMP;
    public void Log(string msg)
    {
        if (debugActive)
        {
           // transform.gameObject.SetActive(true);
            debugTMP.text += "\n" + msg;
        }
     
    }
    public void LogError(string msg)
    {
        if (debugActive)
        {
            //transform.gameObject.SetActive(true);
            debugTMP.text = "err: " + msg;
        }


        //todo : change text color 
        //todo : error recording it persistance data..
    }
    
    public void ReturnBTN()
    {
        transform.gameObject.SetActive(false);
    }
    public void ClearTXT()
    {
        debugTMP.text = "";

    }

}
