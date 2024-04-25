using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Zenject;

public class ImageTracker : MonoBehaviour
{
    [Inject]
    ARTrackedImageManager _trackedImagesManager;
    [Inject]
    SignalBus _signalBus;

    [Inject]
    WebRequestManager requestManager;
    [Inject]
    MenuReader menuReader;
    [Inject]
    DebugPanel Debug;
    [Inject]
    JoyStickData joystickData;

    bool KeepGeneratedMenus { get; set; }
    private void OnEnable()
    {
        _trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged;
    }
    void OnDisable()
    {
        _trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    private List<ARTrackedImage> allTrackedImages = new List<ARTrackedImage>();
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {

        foreach (var trackedImage in eventArgs.added)
        {
           
            allTrackedImages.Add(trackedImage);
        }
        if (!requestManager.IsConnected || !menuReader.downloadedAllData)
        {
            Debug.Log("waiting connection or data download");
            return;
        }


        foreach (var trackedImage in allTrackedImages)
        {

            //ref image always null name in simulator mode .bug. 
            Debug.Log("referenceImage :" + trackedImage.referenceImage.name);
            var cardListView = trackedImage.gameObject.GetComponentInChildren<VerticalLayoutGroup>();
            _signalBus.Fire(new OnImageTrackedEvent(trackedImage.referenceImage.name, cardListView.transform));

        }
        allTrackedImages.Clear();
        if (!KeepGeneratedMenus)
        {
            foreach (var trackedImage in eventArgs.updated)
            {
                Vector3 currentPosition = trackedImage.transform.position;
                currentPosition.z += joystickData.verticalInput;
                currentPosition.x += joystickData.horizontalInput;
                trackedImage.transform.position = currentPosition;
                trackedImage.gameObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
            }
        }

    }
    public void AddTrackedImage(Texture2D texture2D, string name)
    {
        //   Debug.Log("Add Image " + ARSession.state);

        if (!(ARSession.state == ARSessionState.SessionInitializing || ARSession.state == ARSessionState.SessionTracking))
            return;

        Debug.Log("Add Image to lib" + name);

        if (_trackedImagesManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            //   Debug.Log($"Add Image Mutable {name}");

            mutableLibrary.ScheduleAddImageWithValidationJob(texture2D, name, 0.5f);

        }
    }


    public void KeepMenuChange()
    {
        KeepGeneratedMenus = !KeepGeneratedMenus;
    }//disable tracking  update ..
}