using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
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


        #endregion


        #region Monobehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            cameraTransform = Camera.main.transform;
            cameraMain = cameraTransform.GetComponent<Camera>();
            color1 = ClipperGate.GetColor(GetComponentInParent<PhotonView>().Owner.GetPlayerNumber());
            color2 = ClipperGate.GetColor2(GetComponentInParent<PhotonView>().Owner.GetPlayerNumber());
        }


        void Update()
        {

            if (portal1 != null && portal2 != null)
            {
                PortalUpdate(portal1.transform, portal2.transform, port2Cam);
                PortalUpdate(portal2.transform, portal1.transform, port1Cam);
                
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

                        //if (ang > 0) ang -= 90;
                        //else if (ang < 0) ang += 90;

                        Vector3 charV = portal2.transform.rotation.eulerAngles;
                        player.rotation = Quaternion.Euler(0f, charV.y + ang, 0f);

                        player.position = portal2.transform.position + (portal2.transform.forward * 0.10f);

                    }
                    else if (collidedPortal.GetInstanceID() == portal2.GetInstanceID())
                    {

                        float ang = Vector3.Angle(portal2.transform.right, cameraTransform.forward) - 90;

                        //if (ang > 0) ang -= 90;
                        //else if (ang < 0) ang += 90;

                        Vector3 charV = portal1.transform.rotation.eulerAngles;
                        player.rotation = Quaternion.Euler(0f, charV.y + ang, 0f);


                        player.position = portal1.transform.position + (portal1.transform.forward * 0.10f);     

                    }
                }
                proccessCollision = false;
            }
        }

        #endregion

        void PortalUpdate(Transform mainPortal, Transform otherPortal, Camera otherCam)
        {
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




           otherCam.transform.localPosition = new Vector3(horiDistance * -1, distance.y, (vertiDistance - 0.6f) * -1);



            Vector4 clipPlaneWorldSpace =
                    new Vector4(
                        otherPortal.forward.x,
                        otherPortal.forward.y-0.15f,
                        otherPortal.forward.z,
                        Vector3.Dot(otherPortal.position, -otherPortal.forward));

            Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(otherCam.worldToCameraMatrix)) * clipPlaneWorldSpace;


            otherCam.projectionMatrix = cameraMain.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }

        void CreateRenderTexture(RenderTexture portalRender, Camera sourceCam, GameObject target, Color borderColor)
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
            portal1.GetComponent<PortalCollision>().Initialise(this);
            port1Cam = portal1.GetComponentInChildren<Camera>();
            CreateRenderTexture(camTexture2, port2Cam, portal1, color1);
            CreateRenderTexture(camTexture1, port1Cam, portal2, color2);
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
            portal2.GetComponent<PortalCollision>().Initialise(this);
            port2Cam = portal2.GetComponentInChildren<Camera>();
            CreateRenderTexture(camTexture1, port1Cam, portal2, color2);
            CreateRenderTexture(camTexture2, port2Cam, portal1, color1);
        }

        #endregion

        private void OnDisable()
        {
            Destroy(portal1);
            Destroy(portal2);
        }
    }
}


