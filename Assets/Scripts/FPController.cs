using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class FPController : MonoBehaviour
    {
        #region Public Fields

        public AudioSource audio;

        [Tooltip("Damage per bullet")]
        public float damage = 0.5f;

        #endregion

        #region Private 

        private float grenadeSpawnDelay = 0.56f;
        private FXManager fxManager;


        Animator animator;
        Animator parentAnimator;
        Transform cameraTransform;

        bool isActive; //reconnect if target lost or camera changed

        #endregion

        #region Monobehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            parentAnimator = gameObject.GetComponentInParent<Animator>();
        }

        // Update is called once per frame

        private void Awake()
        {
            
        }

        void LateUpdate()
        {
            if(fxManager == null)
            {
                fxManager = GameObject.Find("FXManager").GetComponent<FXManager>(); 
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

        //public void OnStartFPView(GameObject givenParent)
        //{
        //    parent = givenParent;

        //    parent.transform.GetChild(0).gameObject.SetActive(false);
        //    parent.transform.GetChild(1).gameObject.SetActive(false);

        //    cameraTransform = Camera.main.transform;
        //    cameraTransform.SetParent(this.gameObject.transform);
        //    cameraTransform.position = this.gameObject.transform.position + new Vector3(0f, 0.0869f, 0f);
        //    cameraTransform.rotation = this.gameObject.transform.rotation;

        //    Cursor.lockState = CursorLockMode.Locked;

        //}

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
                //var bullet = PhotonNetwork.Instantiate(Prefabs.bulletPrefab.name, Spawnpoints.bulletSpawnPoint.transform.position, Spawnpoints.bulletSpawnPoint.rotation,0); //spawning it for all players

                //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * 1;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {

                Grenade();
                //var bullet = PhotonNetwork.Instantiate(Prefabs.bulletPrefab.name, Spawnpoints.bulletSpawnPoint.transform.position, Spawnpoints.bulletSpawnPoint.rotation,0); //spawning it for all players

                //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * 1;
            }
        }

        #region Private Methods

        private void Fire()
        {

            audio.Play();
            animator.Play("Fire", 0);

            //Ray bullet = new Ray(Spawnpoints.bulletSpawnPoint.transform.position, Spawnpoints.bulletSpawnPoint.transform.forward);
            RaycastHit hit = new RaycastHit();
            
            
            if(Physics.Raycast(Spawnpoints.bulletSpawnPoint.transform.position, Spawnpoints.bulletSpawnPoint.transform.up, out hit))
            {
                if(hit.transform.CompareTag("Player"))
                {
                    hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                    fxManager.GetComponent<PhotonView>().RPC("ShotPlayer", RpcTarget.All, Spawnpoints.bulletSpawnPoint.transform.position, hit.point);
                }
                else
                {
                    fxManager.GetComponent<PhotonView>().RPC("ShotOther", RpcTarget.All, Spawnpoints.bulletSpawnPoint.transform.position, hit.point);
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
            parentAnimator.Play("Grenade_Throw", 1, 0f);
            animator.Play("GrenadeThrow", 0);
        }

        #endregion

        #region IENumerator
        private IEnumerator GrenadeDelay()
        {
            GameObject grenade;

            yield return new WaitForSeconds(grenadeSpawnDelay);

            grenade = Instantiate(Prefabs.grenadePrefab, Spawnpoints.grenadeSpawnPoint.transform.position, Spawnpoints.grenadeSpawnPoint.rotation);
            grenade.GetComponent<Rigidbody>().velocity = grenade.transform.forward * 10f;

            //Wait for set amount of time before spawning grenade
            //Spawn grenade prefab at spawnpoint
            //Instantiate(grenadePrefab,
               // grenadeSpawnpoint.transform.position,
                //grenadeSpawnpoint.transform.rotation);
        }

        #endregion
    }
}
