using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

namespace UnityEngine.XR.ARFoundation.Samples
{

    public class DynamicImageLib : MonoBehaviour
    {
        ARTrackedImageManager aRTrackedImageManager;
        GameObject mTrackedImagePrefab;
        Texture2D imgToTexture2d;
        public MutableRuntimeReferenceImageLibrary myRuntimeImageLibrary;
        public Text msgText;
        //public Text numText;
        bool isRunning = false;
        List<string> allURLs = new List<string> { "https://doutuji.cn/h5/one.png","https://doutuji.cn/h5/two.png"};

        void Awake()
        {
            //Screen.sleepTimeout = SleepTimeout.NeverSleep;
            aRTrackedImageManager = GetComponent<ARTrackedImageManager>();
            aRTrackedImageManager.enabled = false;
            myRuntimeImageLibrary = aRTrackedImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
            aRTrackedImageManager.requestedMaxNumberOfMovingImages = 4;
            aRTrackedImageManager.trackedImagePrefab = mTrackedImagePrefab;
            aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
        }
      
        public void DownLibImages()
        {
            StartCoroutine(AddImageTrackerByUrl());
        } 
   
        public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
        {
            foreach (var trackedImage in args.added)
            {               
                //Logger.Log("DynamicImageLib myRuntimeImageLibrary" + trackedImage.name);
                msgText.text = "已识别：" + trackedImage.referenceImage.name;
            }
        }

        IEnumerator AddImageTrackerByUrl()
        {
            Logger.Log("DynamicImageLib-AddImageTrackerByUrl");
            msgText.text = "lib图数量：0" ;
            isRunning = true;
            aRTrackedImageManager.enabled = false;
            if (aRTrackedImageManager.descriptor.supportsMutableLibrary)
            {
                foreach (var link in allURLs)
                {
                    using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(link))
                    {
                        yield return webRequest.SendWebRequest();

                        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                        {
                            Debug.Log("Error is " + webRequest.error);
                        }
                        else
                        {
                            imgToTexture2d = DownloadHandlerTexture.GetContent(webRequest);
                            //Unity.Jobs.JobHandle jobHandle = myRuntimeImageLibrary.ScheduleAddImageJob(imgToTexture2d, Path.GetFileName(link), 0.2f);
                            AddReferenceImageJobState jobState = myRuntimeImageLibrary.ScheduleAddImageWithValidationJob(imgToTexture2d, Path.GetFileName(link), imgToTexture2d.width);
                            jobState.jobHandle.Complete();
                            if (!jobState.jobHandle.IsCompleted)
                            {
                                Logger.Log("!jobState.jobHandle.IsCompleted");
                            }
                            if (myRuntimeImageLibrary != null)
                            {
                                //Debug.Log("Image Library Count: " + myRuntimeImageLibrary.count);
                                msgText.text = "lib图数量：" + myRuntimeImageLibrary.count.ToString();
                                aRTrackedImageManager.referenceLibrary = myRuntimeImageLibrary;
                                Logger.Log("DynamicImageLib myRuntimeImageLibrary");
                            }
                            webRequest.downloadHandler.Dispose();
                            imgToTexture2d = null;
                            aRTrackedImageManager.enabled = true;
                        }
                    }
                }
            }
        }
    }
}