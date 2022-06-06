using System.Collections;
using UnityEngine.Networking;
using System;
using System.IO;
namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UniityWebSprite : MonoBehaviour
    {
        private string url = @"https://doutuji.cn/h5/ruixingkaku.png";//����ͼƬ��ַ
        private string tName = "ruixingkaku";

        [SerializeField]
        private bool isSaveLocally = true;       

        [SerializeField]
        private Camera arCamera = null;//����AR�����
        
        private GameObject placedPrefabCube = null;//CubeԤ�Ƽ�
        private GameObject placedGameObjectCube = null;//CubeԤ�Ƽ�ʵ�����Ķ���

        public UnityEngine.UI.Image _img;//����Image����
        void Start()
        {   
            Logger.Log("UniityWebSprite-Start");
            placedPrefabCube = Resources.Load<GameObject>("Prefabs/Cube");//��Resouces����Ԥ�Ƽ�
            Vector3 leftPosition = new Vector3(0.0f, 0.0f, 1.0f);//ƫ����            
            Pose signRuixing = GetCameraPose();//��ȡ�����pose
            signRuixing.position = arCamera.transform.position + leftPosition;
            signRuixing.rotation = Quaternion.Euler(0.0f, 205, 0.0f);//��ת
            placedGameObjectCube = Instantiate(placedPrefabCube, signRuixing.position, signRuixing.rotation);//ʵ����
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
            yield return wr.SendWebRequest();//yield,�������������ִ�к���Ĵ���
            if (wr.result != UnityWebRequest.Result.ConnectionError)
            {                
                Texture2D tex = texDl.texture;
                placedGameObjectCube.GetComponent<Renderer>().material.mainTexture = tex;//��ͼCube
                Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),Vector2.zero, 1f);
                _img.sprite = s;//��ͼImage

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

