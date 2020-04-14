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

            SpawnPlayer();

        }

        #endregion

        #region Private Methods

        void SpawnPlayer()
        {
            if (PlayerManager.localPlayerInstance == null)
            {
                int index = Random.Range(0, spawnpoints.Length);
                PhotonNetwork.Instantiate(this.playerFab.name, spawnpoints[index], Quaternion.identity, 0);
            }
        }

        #endregion


        #region Public Methods

        public void LeaveGame()
        {
            PhotonNetwork.LeaveRoom();
        }


        public void Respawn(PhotonView player)
        {
            PhotonNetwork.Destroy(player);
            StartCoroutine(RespawnTimer());
            var index = Random.Range(0, spawnpoints.Length);
            player.transform.position = spawnpoints[index];
        }

        #endregion

        private void OnPlayerNumberingChanged()
        {
            Debug.Log("ON Player Numbering changed");
        }

        #region IENumerator
        private IEnumerator RespawnTimer()
        {
            yield return new WaitForSeconds(ClipperGate.PLAYER_RESPAWN_TIME);
            SpawnPlayer();
        }

        #endregion
    }
}
