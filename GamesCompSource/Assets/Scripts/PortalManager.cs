using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class PortalManager : MonoBehaviour
    {
        #region Public Variables

        public GameObject portal;

        #endregion

        #region Private Variables;

        private GameObject portal1;
        private GameObject portal2;

        private Camera port1Cam;
        private Camera port2Cam;
        private RenderTexture camTexture1;
        private RenderTexture camTexture2;
        private Color color1;
        private Color color2;

        private GameObject collidedPortal;
        private Transform player;
        private bool proccessCollision = false;


        private Transform cameraTransform;
        private Camera cameraMain;

        private bool grenadeTele = false;
        Vector3 grenadeHitPoint;
        GameObject grenadeTemp;
        GameObject grenadeTempPortalHit;
        GameObject grenadeTempPortalOpposite;

        #endregion


        #region Monobehaviour Callbacks
        void Start()
        {
            color1 = ClipperGate.GetColor(GetComponentInParent<PhotonView>().Owner.GetPlayerNumber());
            color2 = ClipperGate.GetColor2(GetComponentInParent<PhotonView>().Owner.GetPlayerNumber());
        }

        private void Awake()
        {
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
                cameraMain = cameraTransform.GetComponent<Camera>();
            }
        }

        void Update()
        { 
            if (portal1 != null && portal2 != null)
            {
                PortalUpdate(portal1.transform, portal2.transform, port2Cam, camTexture2, color1);
                PortalUpdate(portal2.transform, portal1.transform, port1Cam, camTexture1, color2);
                
            }
            if(Input.GetKeyDown(KeyCode.K) && portal1 != null)
            {
                Destroy(portal1);
                port1Cam = null;
            }
            if (Input.GetKeyDown(KeyCode.J) && portal2 != null)
            {
                Destroy(portal2);
                port2Cam = null;
            }

        }

        void FixedUpdate()
        {
            if (proccessCollision)
            {
                if (portal1 != null && portal2 != null)
                {
                    if (collidedPortal.GetInstanceID() == portal1.GetInstanceID())
                    {
                        float ang = Vector3.Angle(portal1.transform.right, cameraTransform.forward) - 90;

                        Vector3 charV = portal2.transform.rotation.eulerAngles;
                        player.rotation = Quaternion.Euler(0f, charV.y + ang, 0f);

                        player.position = portal2.transform.position + (portal2.transform.forward * 0.10f);

                    }
                    else if (collidedPortal.GetInstanceID() == portal2.GetInstanceID())
                    {

                        float ang = Vector3.Angle(portal2.transform.right, cameraTransform.forward) - 90;

                        Vector3 charV = portal1.transform.rotation.eulerAngles;
                        player.rotation = Quaternion.Euler(0f, charV.y + ang, 0f);


                        player.position = portal1.transform.position + (portal1.transform.forward * 0.10f);     

                    }
                }
                proccessCollision = false;
            }

            if (grenadeTele)
            {
                Vector3 localDirection = grenadeTempPortalHit.transform.InverseTransformDirection(grenadeTemp.transform.GetComponent<Rigidbody>().velocity.normalized);
                //localDirection.z *= -1;
                localDirection.x *= -1;
                Vector3 transformedDirection = grenadeTempPortalOpposite.transform.TransformDirection(localDirection);
                grenadeTemp.transform.GetComponent<Rigidbody>().velocity = transformedDirection * grenadeTemp.transform.GetComponent<Rigidbody>().velocity.magnitude;


                Vector3 localPosition = grenadeTempPortalHit.transform.InverseTransformPoint(grenadeHitPoint);
                localPosition.x *= -1;
                Vector3 newPosition = grenadeTempPortalOpposite.transform.TransformPoint(localPosition);
                newPosition += (grenadeTempPortalOpposite.transform.forward * 0.2f);

                grenadeTemp.transform.position = newPosition;
                grenadeTemp.GetComponent<TrailRenderer>().Clear();

                grenadeTele = false;

                Debug.DrawRay(newPosition, transformedDirection, Color.green, 4);

            }
        }

        void OnDisable()
        {
            Destroy(portal1);
            Destroy(portal2);
        }

        #endregion

        #region Private Methods

        void PortalUpdate(Transform mainPortal, Transform otherPortal, Camera otherCam, RenderTexture otherCamTex, Color border)
        {
            if (!VisibleFromCamera(mainPortal.GetComponent<MeshRenderer>(), cameraMain))
            {
                if (otherCamTex != null)
                {
                    otherCamTex.Release();
                    otherCam.enabled = false;
                }
                return;
            }
            if(!otherCam.enabled)
            {
                otherCam.enabled = true;
                CreateRenderTexture(otherCamTex, otherCam, mainPortal, border);
            }

            float horizontalDiff = Vector3.Angle(mainPortal.right, cameraTransform.forward);
            float verticalDiff = Vector3.Angle(cameraTransform.forward, mainPortal.up);


            otherCam.transform.localEulerAngles = new Vector3(verticalDiff - 90, horizontalDiff - 90, 0);


            Vector3 distance = cameraTransform.position - mainPortal.position;

            var heading = cameraTransform.position - mainPortal.position;
            heading.y = 0;
            var distance2 = heading.magnitude;
            var direction = heading / distance2;

            float angleY = Vector3.SignedAngle(mainPortal.forward.normalized, direction, mainPortal.up.normalized);

            float angleUnknown = 180 - 90 - angleY;

            float horiDistance = (distance2 / Mathf.Sin(90 * Mathf.Deg2Rad)) * Mathf.Sin(angleY * Mathf.Deg2Rad);
            float vertiDistance = (distance2 / Mathf.Sin(90 * Mathf.Deg2Rad)) * Mathf.Sin(angleUnknown * Mathf.Deg2Rad);




           otherCam.transform.localPosition = new Vector3(horiDistance * -1, distance.y, (vertiDistance) * -1);



            Vector4 clipPlaneWorldSpace =
                    new Vector4(
                        otherPortal.forward.x,
                        otherPortal.forward.y-0.15f,
                        otherPortal.forward.z,
                        Vector3.Dot(otherPortal.position, -otherPortal.forward));

            Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(otherCam.worldToCameraMatrix)) * clipPlaneWorldSpace;


            otherCam.projectionMatrix = cameraMain.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }

        void CreateRenderTexture(RenderTexture portalRender, Camera sourceCam, Transform target, Color borderColor)
        {
            if (portalRender != null)
            {
                portalRender.Release();
            }
            portalRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBHalf);
            portalRender.useMipMap = true;
            portalRender.filterMode = FilterMode.Trilinear;
            sourceCam.GetComponent<Camera>().targetTexture = portalRender;
            var temp = target.GetComponent<Renderer>().materials;
            temp[0].SetColor("_MainColor", borderColor);
            temp[1].SetColor("_MainColor", borderColor);
            temp[2].SetTexture("_MainTex", portalRender);           

        }

        bool VisibleFromCamera(Renderer renderer, Camera camera)
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
        }


        #endregion

        #region Public Methods

        public void HandleCollision(GameObject portal, Transform character)
        {
            collidedPortal = portal;
            player = character;
            proccessCollision = true;
        }

        public bool CheckOwnership(GameObject myPortal)
        {
            if (myPortal == portal1 || myPortal == portal2)
            {
                return true;
            }
            return false;
        }

        public Ray ShootThroughPortal(GameObject myPortal, Vector3 hitPoint, Vector3 directionForward)
        {
            GameObject tempPortalHit = null;
            GameObject tempPortalOpposite = null;

            if(myPortal == portal1)
            {
                tempPortalHit = portal1;
                tempPortalOpposite = portal2;

            } else if(myPortal == portal2)
            {
                tempPortalHit = portal2;
                tempPortalOpposite = portal1;
            }
            
            if(tempPortalHit != null)
            {
                Vector3 localDirection = tempPortalHit.transform.InverseTransformDirection(directionForward);
                localDirection.z *= -1;
                localDirection.x *= -1;
                Vector3 transformedDirection = tempPortalOpposite.transform.TransformDirection(localDirection);

                Vector3 localPosition = tempPortalHit.transform.InverseTransformPoint(hitPoint);
                localPosition.x *= -1;
                Vector3 rayPosition = tempPortalOpposite.transform.TransformPoint(localPosition);

                Debug.DrawRay(rayPosition, transformedDirection, Color.green, 4);

                Ray ray = new Ray(rayPosition, transformedDirection);

                return ray;
            }

            return new Ray();
        }


        public void TeleportGrenade(GameObject myPortal, GameObject grenade, Vector3 hitPoint)
        {

            if (myPortal == portal1)
            {
                grenadeTemp = grenade;
                grenadeHitPoint = hitPoint;
                grenadeTempPortalHit = portal1;
                grenadeTempPortalOpposite = portal2;
                grenadeTele = true;
            }
            else if (myPortal == portal2)
            {
                grenadeTemp = grenade;
                grenadeHitPoint = hitPoint;
                grenadeTempPortalHit = portal2;
                grenadeTempPortalOpposite = portal1;
                grenadeTele = true;
            }

            
           
            // This should give the relative rotation of the destination portal to the current one
            //Quaternion relativeRotation = Quaternion.Inverse(tempPortalHit.transform.rotation) * tempPortalOpposite.transform.rotation;
            //grenade.transform.rotation *= relativeRotation;

            
            // Whatever way velocity is being maintained, I'm simplifying it here
            // This should rotate the velocity vector in the same manner as the character is rotated
            //grenade.transform.GetComponent<Rigidbody>().velocity = relativeRotation * grenade.transform.GetComponent<Rigidbody>().velocity;
        }
        #endregion



        #region RPC

        [PunRPC]
        private void ShootPortal1(Vector3 spawnPoint, Quaternion rotAngle)
        {
            if(portal1 != null)
            {
                Destroy(portal1);
                camTexture1 = null;
            }
            portal1 = Instantiate(portal, spawnPoint, rotAngle);
            portal1.GetComponentInChildren<PortalCollision>().Initialise(this, portal1);
            port1Cam = portal1.GetComponentInChildren<Camera>();
            CreateRenderTexture(camTexture2, port2Cam, portal1.transform, color1);
            CreateRenderTexture(camTexture1, port1Cam, portal2.transform, color2);
        }

        [PunRPC]
        private void ShootPortal2(Vector3 spawnPoint, Quaternion rotAngle)
        {
            if (portal2 != null)
            {
                Destroy(portal2);
                camTexture2 = null;
            }
            portal2 = Instantiate(portal, spawnPoint, rotAngle);
            portal2.GetComponentInChildren<PortalCollision>().Initialise(this, portal2);
            port2Cam = portal2.GetComponentInChildren<Camera>();
            CreateRenderTexture(camTexture1, port1Cam, portal2.transform, color2);
            CreateRenderTexture(camTexture2, port2Cam, portal1.transform, color1);
        }

        #endregion

    }
}


