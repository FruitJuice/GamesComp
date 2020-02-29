using Com.NUIGalaway.CompGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields
        [Tooltip("The health of the player")]
        public float health = 2f;

        [Tooltip("Local Player Instnace. Used to know if the local player is represented in the Scene")]
        public static GameObject localPlayerInstance;

        #endregion

        #region Private Fields

        FPController fpController;



        #endregion

        #region MonoBehaviour CallBacks

        //Called on GameObject by Unity during initialization
        void Awake()
        {
            //prevent the local player from getting instantiated when loading a new scene
            if (photonView.IsMine)
            {
                PlayerManager.localPlayerInstance = this.gameObject;
            }

            //the instance isn't destoryed when loading a new scene through level synchornization, smooth transitions.
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {

            if (photonView.IsMine) //only follow the local player with the camera
            {
                ConfigureFPV();
            }
            else
            {
                Renderer[] fpvMesh = this.transform.GetChild(3).GetComponentsInChildren<Renderer>();
                foreach (Renderer r in fpvMesh)
                {
                    r.enabled = false;
                }
            }

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Update is called once per frame
        void Update()
        {
            //only execute the inputs if it's the local player
            if (photonView.IsMine)
            {
                fpController.InputProcess();

                if (health <= 0f)
                {
                    GameManager.instance.LeaveRoom();
                }
            }

        }

    
        void CalledOnLevelWasLoaded(int level)
        {
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f)) //Sends a raycast downward to see if anything below the player, if not puts them in the centre of the arena
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
            if (photonView.IsMine)
            {
                ConfigureFPV();
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Private Methods
        //Proccesses the inputs

        void ConfigureFPV()
        { 
            fpController = this.transform.Find("arms_assault_rifle_02").gameObject.GetComponent<FPController>();


            Renderer[] addons = this.transform.GetChild(0).GetComponentsInChildren<Renderer>();
            foreach (Renderer r in addons)
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }

            Renderer[] meshSkin = this.transform.GetChild(1).GetComponentsInChildren<Renderer>();
            foreach (Renderer r in meshSkin)
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }

            Renderer[] fpvMesh = this.transform.GetChild(3).GetComponentsInChildren<Renderer>();
            foreach (Renderer r in fpvMesh)
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

        }



        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }

        #endregion

        #region IPunObservable

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

            if (stream.IsWriting)
            {
                stream.SendNext(health);
            }
            else
            {
                this.health = (float)stream.ReceiveNext();
            }
        }

        #endregion

        #region PunRPC

        [PunRPC]
        private void TakeDamage(float damage)
        {
            health -= damage;
        }

        #endregion
    }
}
