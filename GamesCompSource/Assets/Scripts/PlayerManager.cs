using Com.NUIGalaway.CompGame;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

namespace Com.NUIGalway.CompGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("Local Player Instnace. Used to know if the local player is represented in the Scene")]
        public static GameObject localPlayerInstance;

        public GameObject coinsPrefab;

        #endregion

        #region Private Fields

        private float health;
        Text canvasHealth;

        #endregion

        #region MonoBehaviour CallBacks

        //Called on GameObject by Unity during initialization
        void Awake()
        {
            //prevent the local player from getting instantiated when loading a new scene
            if (photonView.IsMine)
            {
                PlayerManager.localPlayerInstance = this.gameObject;
                this.gameObject.layer = 8;
                canvasHealth = GameObject.Find("PlayerCanvas").transform.Find("PlayerInformationPanel").transform.Find("Contrast").Find("CurrentHealth").GetComponent<Text>();
                canvasHealth.text = ClipperGate.PLAYER_MAX_HEALTH.ToString();
            }

        }

        void Start()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == photonView.OwnerActorNr)
                {
                    string characterSelection = (string)p.CustomProperties[ClipperGate.CHOSEN_CHARACTER];
                    health = ClipperGate.PLAYER_MAX_HEALTH;
                    ConfigureCharacter(characterSelection);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            //only execute the inputs if it's the local player
            if (photonView.IsMine)
            {
                if (health <= 0f)
                {
                    float score = (float)PhotonNetwork.LocalPlayer.CustomProperties[ClipperGate.PLAYER_SCORE];
                    if (score >= 25)
                    {
                        Hashtable newScore = new Hashtable { { ClipperGate.PLAYER_SCORE, (score - 25f) } };
                        PhotonNetwork.LocalPlayer.SetCustomProperties(newScore);
                    }
                    PlayerManager.localPlayerInstance = null;
                    PhotonNetwork.Instantiate(coinsPrefab.name, this.gameObject.transform.position, Quaternion.identity);
                    GameManager.instance.Respawn(photonView);
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
                if (c.CompareTag("PlayableCharacter") && c.name != characterSelected)
                {

                    c.gameObject.SetActive(false);

                }
                else if (c.name == characterSelected && photonView.IsMine)
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

            Renderer[] fpvMesh = this.transform.Find("arms_assault_rifle_01").gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in fpvMesh)
            {
                if (!photonView.IsMine)
                {
                    r.enabled = false;
                }
            }

        }

        #endregion


        #region Public Methods
        public void TakeGrenadeDamage(float damage)
        {
            if (photonView.IsMine)
            {
                health -= damage;
                canvasHealth.text = health.ToString();
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
            if (photonView.IsMine)
            {
                canvasHealth.text = health.ToString();
            }
        }
        #endregion

    }
}
