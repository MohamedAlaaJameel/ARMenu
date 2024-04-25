using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Zenject;

public class MenuReader : MonoBehaviour
{
    [Inject]
    WebRequestManager requestManager;
    [Inject]
    MealUiElemnt.Factory mealFactory;
    [Inject]
    ImageTracker _imageTracker;
    [Inject]
    SignalBus _signalBus;
    [Inject]
    DebugPanel Debug;
    [Inject]
    LoadingBarPanel loadingBar;
    public string categoryURL = "https://www.themealdb.com/api/json/v1/1/categories.php";
    public string foodFilterURL = "https://www.themealdb.com/api/json/v1/1/filter.php?c=";

    ARMenu arMenu { get; set; }
    WaitForSeconds waitTime;
    Coroutine downloadCoroutine;
    private void Start()
    {
        DownloadBasicData();

        waitTime = new WaitForSeconds(1);
    }

    private void DownloadBasicData()//todo btn that handle download..
    {
        if (downloadCoroutine != null)
        {
            StopCoroutine(downloadCoroutine);
        }
        downloadCoroutine =  StartCoroutine(DownloadDataOnNetworkConnection());
    }

    public bool downloadedAllData { get; private set; }
    bool gotCategoryTextures { get; set; }
    bool gotMainCateogry { get; set; }
    bool gotMealsDataOfAllCategorys { get; set; }


    IEnumerator DownloadDataOnNetworkConnection()
    {
        loadingBar.Show();
        loadingBar.StartFillCoroutine("checking connection");

        while (!requestManager.IsConnected)
        {
            yield return waitTime;
        }
        loadingBar.StartFillCoroutine("Downloading categorys",.2f);

       
        GetCategoryesRequest();
        while (!gotMainCateogry)
        {
            Debug.Log("waiting MainCateogry");

            yield return waitTime;
        }
        loadingBar.StartFillCoroutine("Downloading categorys Textures",.3f);

        DownloadCategorysTextures(arMenu);

        while (!gotCategoryTextures)
        {
            Debug.Log("waiting gotCategoryTextures");

            yield return waitTime;
        }
        loadingBar.StartFillCoroutine("Downloading Meals Data",.7f);

        foreach (var category in arMenu.categories)
        {
            GetMealsOfCategoryRequest(category.strCategory);
        }

        while (!gotMealsDataOfAllCategorys)
        {

            Debug.Log("waiting meal data");
            yield return waitTime;
        }

        GenerateTrackingLibrary();

        loadingBar.StartFillCoroutine("GenerateTrackingLibrary");
        loadingBar.StartFillCoroutine("Download Done",1f);

        Debug.Log("got all data");

        downloadedAllData = true;
        //_signalBus.Fire( new OnFullMenuDownloadEvent());
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    public void GetCategoryesRequest()
    {
        requestManager.MakeWebRequest(
         categoryURL,
         onSuccess: (response) =>
         {
             arMenu = JsonConvert.DeserializeObject<ARMenu>(response);
             gotMainCateogry = true;
         }
       ,
         onFailure: (error) =>
         {
             Debug.LogError("Failure: " + error);
         }
     );
    }

    int loadedmealsCatCount = 0;
    public void GetMealsOfCategoryRequest(string category)
    {
        requestManager.MakeWebRequest(
         $"{foodFilterURL}{category}",
          onSuccess: (response) =>
          {
              var cat = arMenu.categories.Where(c => c.strCategory == category).FirstOrDefault();
              if (cat == null)
              {
                  Debug.LogError("the menu category couldnot be null");
                  return;
              }
              var mealdata = JsonConvert.DeserializeObject<MealsData>(response);
              cat.mealsData = mealdata;
              loadedmealsCatCount++;
              if (loadedmealsCatCount >= arMenu.categories.Length)
              {
                  gotMealsDataOfAllCategorys = true;
              }
          }
       ,
         onFailure: (error) =>
         {
             Debug.LogError("Failure: " + error);
         }
     );//
    }

    public void DownloadCategorysTextures(ARMenu arMenu)
    {
        if (arMenu == null || !arMenu.categories.Any())
        {
            Debug.Log("null menu ");
            return;
        }
        int texturesDownloaded = 0;

        foreach (var cat in arMenu.categories)
        {
            requestManager.MakeTextureRequest(cat.strCategoryThumb,
                            onSuccess: (System.Action<Texture2D>)((texture) =>
                            {
                                cat.texture = texture;
                                texturesDownloaded++;
                                if (texturesDownloaded >= arMenu.categories.Length)
                                {
                                    this.gotCategoryTextures = true;
                                }

                            }),
                            onFailure: (error) => Debug.LogError("Texture request failed: " + error)
                        );
        }

    }


    private void AddFoodListToUiByDownloadingCategory(Category cat, OnImageTrackedEvent arg)
    {
        foreach (var meal in cat.mealsData.meals)
        {
            if (meal.downloadedTexture)
            {
                continue;
            }

            int retryCount = 0;
            int maxRetries = 3;

            Action<Texture2D> onSuccessAction = (texture) =>
            {
                meal.texture = texture;
                meal.downloadedTexture = true;

                var food = mealFactory.Create(meal.texture, meal.strMeal, cat.strCategoryDescription);
                food.transform.SetParent(arg.listTransform, false);
            };

            Action<string> onFailureAction = null;
            onFailureAction = (error) =>
            {
                if (retryCount < maxRetries)
                {
                    Debug.LogError($"Meals Texture request failed: {error}. Retrying... Attempt: {retryCount + 1}");

                    // Retry the request
                    requestManager.MakeTextureRequest(meal.strMealThumb, onSuccessAction, onFailureAction);
                    retryCount++;
                }
                else
                {
                    Debug.LogError($"Meals Texture request failed after {maxRetries} attempts: {error}");
                }
            };
            requestManager.MakeTextureRequest(meal.strMealThumb, onSuccessAction, onFailureAction);
        }
    }

    public void GenerateTrackingLibrary()
    {

        foreach (var foodData in arMenu.categories)
        {
            if (foodData == null)
            {
                continue;
            }
            _imageTracker.AddTrackedImage(foodData.texture, foodData.strCategory);//todo event..
        }
    }
    public void ConstructFoodData(OnImageTrackedEvent arg)
    {
        if (arMenu.categories == null || !arMenu.categories.Any())
        {
            Debug.LogError("arMenu is not downloaded please check for network connection before invoking the event");
            return;
        }
        if (string.IsNullOrEmpty(arg.category))
        {
            Debug.Log("are u in simulator mode? cant get ref image of the lib");
            return;
        }
        var cat = arMenu.categories.Where(c => c.strCategory == arg.category).FirstOrDefault();
        if (cat == null)
        {
            Debug.Log("null category , thats impossible..!!");
            return;
        }
        AddFoodListToUiByDownloadingCategory(cat, arg);
    }

}

#region Download all data ....
//DownloadAllFoodTextures(arMenu);
//while (!gotMealsTextures)
//{
//    if (numOfProcessedMealsTextures >= totalMealsTextureCount)
//    {
//        Debug.Log("restarting Coroutine to get missed data");
//        StopCoroutine(downloadCoroutine);
//        downloadCoroutine = StartCoroutine(DownloadDataOnNetworkConnection());
//    }
//    Debug.Log($"waiting meals texures {mealsTexturesDownloaded} / {totalMealsTextureCount}");
//    yield return waitTime;
//} 
#endregion