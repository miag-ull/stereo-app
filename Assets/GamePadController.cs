using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityEngine.Windows;


public class GamePadController : MonoBehaviour
{
    public GameObject leftPlane;
    public GameObject rightPlane;
    public GameObject segmentationPlane;
    public GameObject imageNumberText;


    private WaypointCircuit.WaypointList waypoints;
    private int wpIndex = 0;
    public float planeSpeed = 0.5f;
    public float wpSpeed = 0.2f;
    private bool segmentando = false;
    private Vector3 iniPos;



    Object[] images;

    Material leftMat;
    Material segmentationMat;

    Material rightMat;
    int imageIndex = 0;
   
    // Start is called before the first frame update
    void Start()
    {
        iniPos = segmentationPlane.transform.position;
        Debug.Log(iniPos);
        segmentationPlane.transform.position = new Vector3(iniPos.x, iniPos.y, -10.0f);
        leftMat = leftPlane.GetComponent<Renderer>().material;
        segmentationMat = segmentationPlane.GetComponent<Renderer>().material;

        rightMat = rightPlane.GetComponent<Renderer>().material;

        images = Resources.LoadAll("StereoImages", typeof(Texture));
        
        waypoints = this.GetComponent<WaypointCircuit>().waypointList;
        imageNumberText.GetComponent<TextMesh>().text = imageIndex.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<WaypointCircuit>().DrawContour();
        Texture t;
        t = (Texture)images[imageIndex];
        leftMat.SetTexture("_MainTex", t);
        segmentationMat.SetTexture("_MainTex", t);
        rightMat.SetTexture("_MainTex", t);

        float translation = Input.GetAxis("Vertical2") * planeSpeed;
        translation *= Time.deltaTime;

        imageNumberText.GetComponent<TextMesh>().text = imageIndex.ToString() + "   " + images[imageIndex].name;


        foreach (Transform wp in waypoints.items)
        {
            wp.GetComponent<Renderer>().material.color = Color.white;
        }
        if (!segmentando) {
            leftPlane.transform.Translate(0, -translation, 0);
            rightPlane.transform.Translate(0, -translation, 0);
            iniPos = leftPlane.transform.position;

            if (Input.GetButtonDown("LeftTrigger") && (imageIndex > 0)) --imageIndex;
            if (Input.GetButtonDown("RightTrigger") && (imageIndex < (images.Length - 1))) ++imageIndex;
        }
        else
        {
            segmentationPlane.transform.Translate(0, -translation, 0);
            iniPos = segmentationPlane.transform.position;
            if (Input.GetButtonDown("LeftTrigger")) {
                if (wpIndex == 0)
                {
                    wpIndex = waypoints.items.Length - 1;
                }
                else --wpIndex;
            }
            if (Input.GetButtonDown("RightTrigger"))
            {
                if (wpIndex == waypoints.items.Length - 1)
                {
                    wpIndex = 0;
                }
                else ++wpIndex;
            }

            waypoints.items[wpIndex].GetComponent<Renderer>().material.color = Color.green;
            waypoints.items[wpIndex].transform.Translate(0, Input.GetAxis("Vertical") * wpSpeed,
                                                            Input.GetAxis("Horizontal") * wpSpeed);
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (segmentando)
            {
                segmentationPlane.transform.position = new Vector3(iniPos.x, iniPos.y, -10.0f);
                leftPlane.transform.position = iniPos;
                rightPlane.transform.position = iniPos;
                segmentando = false;
            }
            else
            {
                leftPlane.transform.position = new Vector3(iniPos.x, iniPos.y, -10.0f);
                rightPlane.transform.position = new Vector3(iniPos.x, iniPos.y, -10.0f);
                segmentationPlane.transform.position = iniPos;
                
                segmentando = true;
            }
            
        }
        if (Input.GetButtonDown("Fire2"))
        {
            DrawTexture();
        }
        }
    public void DrawTexture()
    {
        Vector3[] points = this.GetComponent<WaypointCircuit>().GetContourPoints();
        int total = points.Length;
        if (total > 1)
        {
            int textureWidth = 1072; //356
            int textureHeight = 1424; //536
            Color color = Color.white; //interest area
            Color bgColor = Color.black; //background
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            for (int i = 0; i < textureWidth; i++)
                for (int j = 0; j < textureHeight; j++)
                    texture.SetPixel(i, j, bgColor);
            //segmentationPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
            float xScale = segmentationPlane.transform.localScale.x * 10.0f;
            float yScale = segmentationPlane.transform.localScale.z * 10.0f;
            Vector3 center = segmentationPlane.transform.position;



            Vector3 p = points[0];

            int x = Mathf.RoundToInt((p.x - center.x) / xScale * textureWidth) - textureWidth / 2;
            int y = Mathf.RoundToInt((p.y - center.y) / yScale * textureHeight) - textureHeight / 2;
            Bresenham3D line;
            Vector3 a, b, inicio;
            a = new Vector3(x, y, 0);
            inicio = a;
            for (int i = 1; i < total; i++)
            {
                 
                p = points[i];
                x = Mathf.RoundToInt((p.x - center.x) / xScale * textureWidth) - textureWidth / 2;
                y = Mathf.RoundToInt((p.y - center.y) / yScale * textureHeight) - textureHeight / 2;
                b = new Vector3(x, y, 0);
                line = new Bresenham3D(a, b);
                foreach (Vector3 point in line)
                {
                    texture.SetPixel(Mathf.RoundToInt(point[0]), Mathf.RoundToInt(point[1]), color);
                }
                a = b;
            }
            line = new Bresenham3D(inicio, a);
            foreach (Vector3 point in line)
            {
                texture.SetPixel(Mathf.RoundToInt(point[0]), Mathf.RoundToInt(point[1]), color);
            }

            for (int j = 0; j < textureHeight; j++)
            {
                
                int left = -1;
                int right = -1;
                for (int i = 0; i < textureWidth; i++)
                {
                    if (texture.GetPixel(i, j) == color)
                    {
                        left = i;
                        break;
                    }
                }
                for (int i = textureWidth - 1; i >= 0; i--)
                {
                    if (texture.GetPixel(i, j) == color)
                    {
                        right = i;
                        break;
                    }
                }
                if (left != -1 && right != -1)
                {
                    for (int i = left; i < right; i++)
                        texture.SetPixel(i, j, color);
                }
            }
            //texture.Apply();
            //then Save To Disk as PNG
            string name = images[imageIndex].name;
            byte[] bytes = texture.EncodeToPNG();
            string file_name = name + "_seg";
            //Debug.Log(file_name);
            //NativeGallery.SaveImageToGallery(texture, "Segmented Images", file_name);
            var dirPath = Application.dataPath + "/../SaveImages/";
             if (!Directory.Exists(dirPath))
             {
                 Directory.CreateDirectory(dirPath);
             }
            
             File.WriteAllBytes(dirPath + name + "_seg.png", bytes);
             
        }

    }
}
