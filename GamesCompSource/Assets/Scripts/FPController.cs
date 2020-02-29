using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class FPController : MonoBehaviour
    {
        #region Public Fields

        [Tooltip("Damage per bullet")]
        public float damage = 0.5f;

        #endregion

        #region Private 

        private float grenadeSpawnDelay = 0.56f;
        
       
        Animator animator;
        Animator parentAnimator;
        PhotonView fxManager;
        PortalManager portalManager;

        #endregion

        #region Monobehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            parentAnimator = transform.GetComponentInParent<Animator>();
            fxManager = GameObject.Find("FXManager").GetComponent<PhotonView>();
            portalManager = GetComponent<PortalManager>();

        }

        // Update is called once per frame

        private void Awake()
        {
            
        }

        void LateUpdate()
        {
            if(fxManager == null)
            {
                fxManager = GameObject.Find("FXManager").GetComponent<PhotonView>(); 
            }
        }

        #endregion

        #region Public Methods


        [System.Serializable]
        public class prefabs
        {
            [Header("Prefabs")]
            public GameObject casingPrefab;
            public GameObject grenadePrefab;
        }
        public prefabs Prefabs;

        [System.Serializable]
        public class spawnpoints
        {
            [Header("Spawnpoints")]
            public Transform casingSpawnPoint;
            public Transform bulletSpawnPoint;
            public Transform grenadeSpawnPoint;
            [Range(-10, 10)]
            public float bulletSpawnPointMinRotation = -5f;
            [Range(-10, 10)]
            public float bulletSpawnPointMaxRottion = 5f;

        }
        public spawnpoints Spawnpoints;

        #endregion

        

        public void InputProcess()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                Cursor.visible = !Cursor.visible; // toggle visibility
                if (Cursor.visible)
                { // if visible, unlock
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {

                Fire();

            }
            if (Input.GetButton("Fire2"))
            {
                animator.SetBool("Aim", true);
            }
            else
            {
                animator.SetBool("Aim", false);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {

                Grenade();

            }


            if (Input.GetKeyDown(KeyCode.O))
            {
                RaycastHit hit = new RaycastHit();
                
                if (Physics.Raycast(Spawnpoints.bulletSpawnPoint.transform.position, Spawnpoints.bulletSpawnPoint.transform.forward, out hit))
                {
                    if (hit.transform.CompareTag("PortalPlace"))
                    {
                        gameObject.GetComponent<PhotonView>().RPC("ShootPortal1", RpcTarget.All, hit.point + (hit.transform.forward*0.01f) - hit.transform.up.normalized, hit.transform.rotation);
                    }
                }

            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(Spawnpoints.bulletSpawnPoint.transform.position, Spawnpoints.bulletSpawnPoint.transform.forward, out hit))
                {
                    if (hit.transform.CompareTag("PortalPlace"))
                    {
                        gameObject.GetComponent<PhotonView>().RPC("ShootPortal2", RpcTarget.All, hit.point + (hit.transform.forward*0.01f) - hit.transform.up.normalized, hit.transform.rotation);
                    }
                }

            }

        }

        #region Private Methods

        private void Fire()
        {

            animator.Play("Fire", 0);
            //print(parentAnimator.GetLayerIndex("Additive Layer"));
            parentAnimator.Play("Fire", parentAnimator.GetLayerIndex("Additive Layer"));


            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f));
            RaycastHit hit;
            
            
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform.CompareTag("Player"))
                {
                    hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                    fxManager.RPC("ShotPlayer", RpcTarget.All, Spawnpoints.bulletSpawnPoint.transform.position, hit.point);
                }
                else if(hit.transform.CompareTag("Portal"))
                {
                    if (portalManager.CheckOwnership(hit.collider.transform.parent.gameObject))
                    {
                        //fxManager.RPC("ShootNoCollision", RpcTarget.All, Spawnpoints.bulletSpawnPoint.transform.position, hit.point);
                        Debug.DrawRay(ray.origin, ray.direction, Color.green, 4);

                        Ray shootThroughPortalRay = portalManager.ShootThroughPortal(hit.collider.transform.parent.gameObject, hit.point, ray.direction);
                        if(Physics.Raycast(shootThroughPortalRay, out hit))
                        {
                            Debug.DrawRay(shootThroughPortalRay.origin, shootThroughPortalRay.direction, Color.green, 4);
                            if (hit.transform.CompareTag("Player") && hit.transform != this.transform)
                            {
                                hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                                fxManager.RPC("ShotPlayer", RpcTarget.All, shootThroughPortalRay.origin, hit.point);
                            }
                            else
                            {
                                fxManager.RPC("ShotOther", RpcTarget.All, shootThroughPortalRay.origin, hit.point);
                            }
                        }
                        
                    }
                }
                else
                {
                    fxManager.RPC("ShotOther", RpcTarget.All, Spawnpoints.bulletSpawnPoint.transform.position, hit.point);
                }
            }


        }


        private void Grenade()
        {
            GetComponent<PhotonView>().RPC("ThrowGrenade", RpcTarget.All);
        }

        #endregion

        #region PunRPC

        [PunRPC]
        private void ThrowGrenade()
        {
            StartCoroutine(GrenadeDelay());
            parentAnimator.Play("Grenade_Throw", parentAnimator.GetLayerIndex("Additive Layer"));
            animator.Play("GrenadeThrow", 0);
        }

        #endregion

        #region IENumerator
        private IEnumerator GrenadeDelay()
        {
            GameObject grenade;

            yield return new WaitForSeconds(grenadeSpawnDelay);

            grenade = Instantiate(Prefabs.grenadePrefab, Spawnpoints.grenadeSpawnPoint.transform.position, Spawnpoints.grenadeSpawnPoint.rotation);
            grenade.GetComponent<Rigidbody>().velocity = grenade.transform.forward * 5f;
        }

        #endregion
    }
}
