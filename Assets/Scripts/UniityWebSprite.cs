using System.Collections;
using UnityEngine.Networking;
using System;
using System.IO;
namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UniityWebSprite : MonoBehaviour
    {
        private string url = @"https://doutuji.cn/h5/ruixingkaku.png";//下载图片地址
        private string tName = "ruixingkaku";

        [SerializeField]
        private bool isSaveLocally = true;       

        [SerializeField]
        private Camera arCamera = null;//挂载AR摄像机
        
        private GameObject placedPrefabCube = null;//Cube预制件
        private GameObject placedGameObjectCube = null;//Cube预制件实例化的对象

        public UnityEngine.UI.Image _img;//挂载Image对象
        void Start()
        {   
            Logger.Log("UniityWebSprite-Start");
            placedPrefabCube = Resources.Load<GameObject>("Prefabs/Cube");//从Resouces加载预制件
            Vector3 leftPosition = new Vector3(0.0f, 0.0f, 1.0f);//偏移量            
            Pose signRuixing = GetCameraPose();//获取摄像机pose
            signRuixing.position = arCamera.transform.position + leftPosition;
            signRuixing.rotation = Quaternion.Euler(0.0f, 205, 0.0f);//旋转
            placedGameObjectCube = Instantiate(placedPrefabCube, signRuixing.position, signRuixing.rotation);//实例化
            placedGameObjectCube.transform.parent = transform;

        }
        private Pose GetCameraPose()
        {
            return new Pose(arCamera.transform.position,
                arCamera.transform.rotation);
        }
        public void DownAll()
        {
            StartCoroutine(DownMaterial());
        }
       
        IEnumerator DownMaterial()
        {
            Logger.Log("DownMaterial");
            UnityWebRequest wr = new UnityWebRequest(url);
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            wr.downloadHandler = texDl;
            yield return wr.SendWebRequest();//yield,网络请求结束后执行后面的代码
            if (wr.result != UnityWebRequest.Result.ConnectionError)
            {                
                Texture2D tex = texDl.texture;
                placedGameObjectCube.GetComponent<Renderer>().material.mainTexture = tex;//贴图Cube
                Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),Vector2.zero, 1f);
                _img.sprite = s;//贴图Image

                if (isSaveLocally)
                {
                    SaveLocally(tex, tName, ".png");
                }
            }
        }
        private void SaveLocally(Texture2D texture2D, string texName, string spriteType)
        {
            Byte[] bytes = texture2D.EncodeToPNG();
            string local = Application.dataPath + " / Images / " + texName + spriteType;
            File.WriteAllBytes(local, bytes); //
            Logger.Log("SaveLocally:"+ local);
        }

        private void OnApplicationQuit()
        {
            StopAllCoroutines();
        }
    }
}

