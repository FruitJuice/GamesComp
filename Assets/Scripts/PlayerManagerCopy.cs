using Com.NUIGalaway.CompGame;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class PlayerManagerCopy : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("Local Player Instnace. Used to know if the local player is represented in the Scene")]
        public static GameObject localPlayerInstance;


        #endregion

        #region Private Fields

        float health = 2f;
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

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == photonView.OwnerActorNr)
                {
                    string characterSelection1 = (string)p.CustomProperties[ClipperGate.CHOSEN_CHARACTER];
                    health = (float)p.CustomProperties[ClipperGate.PLAYER_HEALTH];
                    ConfigureCharacter(characterSelection1);
                }
            }

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


        #endregion

        #region Private Methods
        void ConfigureCharacter(string characterSelected)
        {
            var characters = this.gameObject.GetComponentsInChildren<Renderer>();

            foreach(Renderer c in characters)
            {
                if(c.CompareTag("PlayableCharacter") && c.name != characterSelected)
                {

                    c.gameObject.SetActive(false);

                } else if(c.name == characterSelected && photonView.IsMine)
                {
                    c.gameObject.layer = 8;
                }

                if (c.CompareTag("GunModel"))
                {
                    if (photonView.IsMine)
                    {
                        foreach (Transform trans in c.gameObject.GetComponentsInChildren<Transform>(true))
                        {
                            trans.gameObject.layer = 8;
                        }
                    }
                }
            }

            Renderer[] fpvMesh = this.transform.Find("arms_assault_rifle_02").gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in fpvMesh)
            {
                if (!photonView.IsMine)
                {
                    r.enabled = false;
                }
            }

            if (photonView.IsMine)
            {
                fpController = this.transform.Find("arms_assault_rifle_02").gameObject.GetComponent<FPController>();
            }

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
