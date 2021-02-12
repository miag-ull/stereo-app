using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccludeCameras : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera leftCam = GameObject.Find("Main Camera:Instant Preview Left").GetComponent<Camera>();
        Camera rightCam = GameObject.Find("Main Camera:Instant Preview Left").GetComponent<Camera>();
        if (leftCam != null)
        {
            leftCam.cullingMask = (1 << LayerMask.NameToLayer("left"));
        }
        if (rightCam != null)
        {
            rightCam.cullingMask = (1 << LayerMask.NameToLayer("right"));
        }
        
    }
}
