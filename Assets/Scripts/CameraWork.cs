using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class CameraWork : MonoBehaviour
    {

        #region Private 

        [Tooltip("Controls how sensitive mouse inputs are")]
        [SerializeField]
        private float mouseSensitivity = 100.0f;

        [Tooltip("Set this false when a component of a prefab is instanciated by PhotonNetwork, manually call OnStartFollowing() when necessary")]
        [SerializeField]
        private bool followOnStart = false;


        Transform cameraTransform;
        bool isFollowing; //reconnect if target lost or camera changed

        private float xRotation = 0f;
        private float yRotation = 0f;


        GameObject FPView;

        #endregion

        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            //check if we want to follow the target
            if (followOnStart)
            {
                OnStartFollowing();
            }
        }

        // LateUpdate is called only once all of the Updates are done. Important for camera since objects can move in Update
        void LateUpdate()
        { 
            //The target we follow may not be destroyed on a level load.
            //so need to check if the Main Camera is different when we load a new scene and reconnect if it is
            if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }
            //Have to be following to keep going
            
            if (isFollowing)
            {
                Apply();
            }
        }

        #endregion

        #region Public Methods

        //useful for instances managed by Photon as we don't know what we will follow
        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            Cursor.lockState = CursorLockMode.Locked;

            FPView = Resources.Load("arms_assault_rifle_02") as GameObject;
            FPView = Instantiate(FPView, cameraTransform);

            //cameraTransform.SetParent(transform);

            //FPView.transform.position = new Vector3(0f, 0f, 0f);

            Apply();
        }

        #endregion

        #region Private Methods

        //follow the target smoothly
        void Apply()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            } else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            yRotation += mouseX;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
            //cameraTransform.Rotate(Vector3.up * mouseX);

            cameraTransform.position = new Vector3(transform.position.x, transform.position.y + 1.35f, transform.position.z);




            //Vector3 targetCentre = transform.position + centreOffset;
            //float originTargetAngle = transform.eulerAngles.y;
            //float currentAngle = cameraTransform.eulerAngles.y;
            //float targetAngle = originTargetAngle;
            //currentAngle = targetAngle;
            //targetHeight = targetCentre.y + height;

            //float currentHeight = cameraTransform.position.y;
            //currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightVelocity, heightSmooth);
            //Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);
            //cameraTransform.position = targetCentre;
            //cameraTransform.position += currentRotation * Vector3.back * distance;
            //cameraTransform.position = new Vector3(cameraTransform.position.x, currentHeight, cameraTransform.position.z);
            //SetUpRotation(targetCentre); //always look at target.
        }


        #endregion
    }
}
