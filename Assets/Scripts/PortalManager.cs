using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    #region Public Variables
    public GameObject portal1;
    public GameObject portal2;

    public GameObject portalCam;

    #endregion

    #region Private Variables;

    public GameObject port1Cam;
    public GameObject port2Cam;

    Transform cameraTransform;
    RenderTexture camTexture1;
    RenderTexture camTexture2;



    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        CreateRenderTexture();
        
        
    }

    // Update is called once per frame
    void Update()
    {

        float angle = Mathf.Atan2(portal1.transform.position.x - cameraTransform.position.x, portal1.transform.position.z - cameraTransform.position.z) * 180 / Mathf.PI;
        //float angle2 = Mathf.Atan2(portal1.transform.position.x - cameraTransform.position.x, portal1.transform.position.y - cameraTransform.position.y) * 180 / Mathf.PI;
        //print(angle);
        port2Cam.transform.rotation = Quaternion.Euler(0, angle, 0);

        port2Cam.transform.rotation = cameraTransform.rotation;

        //float distance = Vector3.Distance(cameraTransform.position, portal1.transform.position);

        Vector3 distance = cameraTransform.position - portal1.transform.position;
        port2Cam.transform.position = portal2.transform.position + distance;

        //port2Cam.transform.position.x = distance;
        //print(distance);
        //port2Cam.transform.position = new Vector3(distance + portal2.transform.position.x, port2Cam.transform.position.y, port2Cam.transform.position.z);

        //Vector3 topRight = portal2.GetComponent<Renderer>().bounds.max;
        //Vector3 topLeft = portal2.GetComponent<Renderer>().bounds.max;
        //topLeft.z = portal2.GetComponent<Renderer>().bounds.min.z;

        //Vector3 bottomLeft = portal2.GetComponent<Renderer>().bounds.min;
        //Vector3 bottomRight = portal2.GetComponent<Renderer>().bounds.min;
        //bottomRight.z = portal1.GetComponent<Renderer>().bounds.max.z;

        //uvs = port1Mesh.uv;
        //for (var i = 0; i < uvs.Length; i++)
        //{
        //    uvs[0] = port2Cam.GetComponent<Camera>().WorldToViewportPoint(bottomLeft);
        //    uvs[1] = port2Cam.GetComponent<Camera>().WorldToViewportPoint(topLeft);
        //    uvs[2] = port2Cam.GetComponent<Camera>().WorldToViewportPoint(topRight);
        //    uvs[3] = port2Cam.GetComponent<Camera>().WorldToViewportPoint(bottomRight);
        //    port1Mesh.uv = uvs;
        //}

        //print(uvs[0]);

    }

    void CreateRenderTexture()
    {
        if(camTexture2 != null)
        {
            camTexture2.Release();
        }
        camTexture2 = new RenderTexture(Screen.width, Screen.height, 24);
        //camTexture2.depthBuffer
        port2Cam.GetComponent<Camera>().targetTexture = camTexture2;
        port2Cam.GetComponent<Camera>().Render();
        portal1.GetComponent<Renderer>().material.SetTexture("_MainTex", camTexture2);

    }

}
