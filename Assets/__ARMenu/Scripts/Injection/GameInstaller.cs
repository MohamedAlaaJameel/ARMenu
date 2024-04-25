using System.ComponentModel;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public WebRequestManager webManager;
    public MealUiElemnt MealUiElemntPrefab;
    public override void InstallBindings()
    {

        #region Signals
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<OnImageTrackedEvent>();


        #endregion
        Container.Bind<MenuReader>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LoadingBarPanel>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ImageTracker>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DescriptionPanel>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DebugPanel>().FromComponentInHierarchy().AsSingle();
        Container.Bind<JoyStickData>().FromComponentInHierarchy().AsSingle();

        Container.BindSignal<OnImageTrackedEvent>()
            .ToMethod<MenuReader>((menu, args) => menu.ConstructFoodData(args))
            .FromResolve();

  


        Container.Bind<WebRequestManager>().FromInstance(webManager).AsSingle();
        Container.Bind<MainMenu>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ARTrackedImageManager>().FromComponentInHierarchy().AsSingle();
        Container.BindFactory<Texture2D, string, string, MealUiElemnt, MealUiElemnt.Factory>().FromComponentInNewPrefab(MealUiElemntPrefab).WithGameObjectName("meal"); ;
    }
}
public class OnImageTrackedEvent
{
 
    public string category;
    public Transform listTransform;

    public OnImageTrackedEvent(string category, Transform listGameObject)
    {
        this.category = category;
        this.listTransform = listGameObject;
    }
}

#region in case of tracking multiple objects 
//I will subscripe to OnTrackedImagesChanged if menu has been downloaded.. 
//Container.BindSignal<OnFullMenuDownloadEvent>()
// .ToMethod<ImageTracker>((tracker, args) => tracker.OnNetworkDataLoading())
// .FromResolve();
//public class OnFullMenuDownloadEvent
//{
//    //public ARMenu aRMenu;

//    //public OnFullMenuDownloadEvent(ARMenu aRMenu)
//    //{
//    //    this.aRMenu = aRMenu;
//    //}
//} 
#endregion