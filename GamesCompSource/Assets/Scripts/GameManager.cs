using Com.NUIGalway.CompGame;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.NUIGalaway.CompGame
{ 
    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Public Fields

        public static GameManager instance;

        [Tooltip("Prefab to represent the player")]
        public GameObject playerFab;

        [Tooltip("Player Spawn Points")]
        public Vector3[] spawnpoints;

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks
        //When a user leaves the room, calls this Photon method that we override to execute code
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterCleint {0}", PhotonNetwork.IsMasterClient);

                //LoadArena();
            }

        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
                //LoadArena();
            }

        }

        public override void OnEnable()
        {
            base.OnEnable();
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public override void OnDisable()
        {
            base.OnEnable();
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion


        #region MonoBehaviour Callbacks

        private void Start()
        {
            instance = this;
            
            if (playerFab == null)
            {
                Debug.LogError("Missing playerPrefab reference. Please set up in GameObject");
            }
            
            if (PlayerManager.localPlayerInstance == null) //only instantiate player if they don't already exist
            {
                Debug.LogFormat("Instantiating local player from {0}", SceneManager.GetActiveScene().name);
                //Inside a room, create a character for the local player. Gets synced by using the PhotonNetwork Instantiate.
                PhotonNetwork.Instantiate(this.playerFab.name, spawnpoints[0], Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene loaf for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        #endregion



        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        private void OnPlayerNumberingChanged()
        {
            Debug.Log("ON Player Numbering changed");
        }
    }
}
