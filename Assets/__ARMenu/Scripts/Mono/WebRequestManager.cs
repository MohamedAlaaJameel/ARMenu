using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class WebRequestManager : MonoBehaviour
{
    [Inject]
    DebugPanel Debug;

    public string connectionTest = "https://www.google.com/";
    internal bool connected;
    internal bool IsConnected => connected;
    WaitForSeconds waitTime { get; set; }

    private void Start()
    {
        StartCoroutine(CheckConnectionCoroutine());
        waitTime = new WaitForSeconds(5f);
    }

    IEnumerator CheckConnectionCoroutine()
    {
 
        while (true)
        {
            MakeWebRequest(connectionTest,
                onSuccess: _ => connected = true,
                onFailure: _ => { connected = false;
                    Debug.Log("No Internet connection...");

                }
            );

            yield return waitTime;  
        }
    }


    public void MakeWebRequest(string url, Action<string> onSuccess = null, Action<string> onFailure = null)
    {
        StartCoroutine(GetStrRequest(url, onSuccess, onFailure));
    }

    public void MakeTextureRequest(string url, Action<Texture2D> onSuccess = null, Action<string> onFailure = null)
    {
        StartCoroutine(GetTextureRequest(url, onSuccess, onFailure));
    }

    IEnumerator GetStrRequest(string url, Action<string> onSuccess = null, Action<string> onFailure = null)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(webRequest.downloadHandler.text);
        }
        else
        {
            onFailure?.Invoke(webRequest.error);
        }
    }
    IEnumerator GetTextureRequest(string url, Action<Texture2D> onSuccess = null, Action<string> onFailure = null)
    {
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
            onSuccess?.Invoke(texture);
        }
        else
        {
            onFailure?.Invoke(webRequest.error);
        }
    }
}
