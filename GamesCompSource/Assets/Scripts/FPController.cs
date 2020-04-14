using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Com.NUIGalway.CompGame
{
    public class FPController : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public float damage = 7.5f;
        public float fireRate = 0.1f;
        public float spreadFactor = 0.02f;
        public float grenadeForce = 0.25f;

        public Animator parentAnimator;

        public GameObject casingPrefab;
        public GameObject grenadePrefab;

        public Transform bulletSpawnPoint;
        public Transform casingSpawnPoint;
        public Transform grenadeSpawnPoint;

        public AudioSource bodyAudio;
        public AudioSource bulletAudio;

        public AudioClip shoot;
        public AudioClip aim;
        public AudioClip reload;

        #endregion

        #region Private 

        private float grenadeSpawnDelay = 0.56f;

        Text canvasAmmo;
        PhotonView fxManager;
        Animator animator;
        PortalManager portalManager;
        PhotonView myView;
     

        bool aimSoundPlayed;
        bool isReloading;
        bool isAiming;
        float lastFired;

        static int maxAmmo = 40;
        private int currentAmmo = 40;

        #endregion

        #region Monobehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            portalManager = GetComponent<PortalManager>();
            myView = gameObject.GetComponent<PhotonView>();
            bulletAudio.clip = shoot;
        }

        private void Awake()
        {
            if (photonView.IsMine)
            {
                fxManager = GameObject.Find("FXManager").GetComponent<PhotonView>();
                canvasAmmo = GameObject.Find("PlayerCanvas").transform.Find("PlayerInformationPanel").transform.Find("Contrast1").Find("CurrentAmmo").GetComponent<Text>();
                canvasAmmo.text = (maxAmmo.ToString() + " / " + maxAmmo.ToString());
            }

        }

        void OnDisable()
        {
            if(photonView.IsMine) canvasAmmo.text = "RESPAWNING";
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Reload Out Of Ammo")){
                isReloading = true;
            }
            else
            {
                isReloading = false;
            }


            if (Input.GetButton("Fire2") && !isReloading)
            {
                isAiming = true;
                animator.SetBool("Aim", isAiming);

                if (!aimSoundPlayed)
                {
                    bodyAudio.clip = aim;
                    bodyAudio.Play();

                    aimSoundPlayed = true;
                }

            }
            else
            {
                isAiming = false;
                animator.SetBool("Aim", isAiming);
                aimSoundPlayed = false;
            }

            if (Input.GetButton("Fire1") && !isReloading && currentAmmo > 0)
            {
                if (Time.time > fireRate + lastFired)
                {
                    lastFired = Time.time;

                    currentAmmo -= 1;
                    canvasAmmo.text = (currentAmmo.ToString() + " / " + maxAmmo.ToString());

                    photonView.RPC("isShooting", RpcTarget.All);
                    if (!isAiming)
                    {
                        Fire();
                        animator.Play("Fire", 0, 0f);
                    }
                    else
                    {
                        Fire();
                        animator.Play("Aim Fire", 0, 0f);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                bodyAudio.clip = reload;
                bodyAudio.Play();

                animator.Play("Reload Out Of Ammo", 0, 0f);
                parentAnimator.SetTrigger("Reload");

                currentAmmo = maxAmmo;
                canvasAmmo.text = (currentAmmo.ToString() + " / " + maxAmmo.ToString());
            }

            if (Input.GetKeyDown(KeyCode.G) && !isReloading && !isReloading)
            {
                Grenade();
            }


            if (Input.GetKeyDown(KeyCode.Q) && !isAiming && !isReloading)
            {
                RaycastHit hit = new RaycastHit();
                
                if (Physics.Raycast(bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.forward, out hit))
                {
                    if (hit.transform.CompareTag("PortalPlace"))
                    {
                        myView.RPC("ShootPortal1", RpcTarget.All, hit.point + (hit.transform.forward*0.01f) - hit.transform.up.normalized, hit.transform.rotation);
                    }
                    else
                    {
                        myView.RPC("DestroyPortal1", RpcTarget.All);
                    }
                }

            }

            if (Input.GetKeyDown(KeyCode.E) && !isAiming && !isReloading)
            {
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.forward, out hit))
                {
                    if (hit.transform.CompareTag("PortalPlace"))
                    {
                        myView.RPC("ShootPortal2", RpcTarget.All, hit.point + (hit.transform.forward*0.01f) - hit.transform.up.normalized, hit.transform.rotation);
                    }
                    else
                    {
                        myView.RPC("DestroyPortal2", RpcTarget.All);
                    }
                }

            }


        }

        #endregion

        #region Private Methods

        private void Fire()
        {
            Vector3 precision = new Vector3(0.5f,0.5f);
            if (!isAiming)
            {
                precision.x += Random.Range(-spreadFactor, spreadFactor);
                precision.y += Random.Range(-spreadFactor, spreadFactor);
            }

            photonView.RPC("parentShoot", RpcTarget.All);

            var layerMask = ~(1 << 8);
            Ray ray = Camera.main.ViewportPointToRay(precision);
            RaycastHit hit;
            
            
            if(Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                if(hit.transform.CompareTag("Player"))
                {
                    hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                  
                    fxManager.RPC("ShotPlayer", RpcTarget.All, bulletSpawnPoint.transform.position, hit.point);
                }
                else if(hit.transform.CompareTag("Portal"))
                {
                    if (portalManager.CheckOwnership(hit.collider.transform.parent.gameObject))
                    {
                        fxManager.RPC("ShootPortal", RpcTarget.All, bulletSpawnPoint.transform.position, hit.point);
                        Debug.DrawRay(ray.origin, ray.direction, Color.green, 4);

                        Ray shootThroughPortalRay = portalManager.ShootThroughPortal(hit.collider.transform.parent.gameObject, hit.point, ray.direction);
                        if(Physics.Raycast(shootThroughPortalRay, out hit))
                        {
                            Debug.DrawRay(shootThroughPortalRay.origin, shootThroughPortalRay.direction, Color.green, 4);
                            if (hit.transform.CompareTag("Player"))
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
                    else fxManager.RPC("ShotOther", RpcTarget.All, bulletSpawnPoint.transform.position, hit.point);
                }
                else
                {
                    fxManager.RPC("ShotOther", RpcTarget.All, bulletSpawnPoint.transform.position, hit.point);
                    
                }
            }
            else
            {
                fxManager.RPC("ShootNoCollision", RpcTarget.All, bulletSpawnPoint.transform.position, ray.direction);
            }


        }


        private void Grenade()
        {
            animator.Play("GrenadeThrow", 0);
            parentAnimator.SetTrigger("ThrowGrenade");



            GetComponent<PhotonView>().RPC("ThrowGrenade", RpcTarget.All);


        }

        #endregion

        #region PunRPC

        [PunRPC]
        private void ThrowGrenade()
        {
            StartCoroutine(GrenadeDelay());
        }

        [PunRPC]
        private void isShooting()
        {
            bulletAudio.Play();
        }

        [PunRPC]
        private void parentShoot()
        {
            parentAnimator.Play("Fire", 1, 0f);
        }

        #endregion

        #region IENumerator
        private IEnumerator GrenadeDelay()
        {
            GameObject grenade;

            yield return new WaitForSeconds(grenadeSpawnDelay);

            grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.transform.position, grenadeSpawnPoint.rotation);
            grenade.GetComponent<Rigidbody>().velocity = grenade.transform.forward * grenadeForce;
        }

        #endregion
    }
}
