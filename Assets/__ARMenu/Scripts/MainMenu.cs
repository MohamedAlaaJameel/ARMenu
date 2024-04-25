using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuObject;
    public TMPro.TextMeshProUGUI startBTNtext;



    public void StartGame()
    {
        mainMenuObject.SetActive(false);
        startBTNtext.text = "Resume";
    }
    public void ExitApp()
    {
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call("finishAffinity");
        //activity.Call<bool>("moveTaskToBack", true);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mainMenuObject.SetActive(!mainMenuObject.activeSelf);
        }
    }
}
