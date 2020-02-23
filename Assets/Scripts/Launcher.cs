using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {

        #region Private Serializable Fields

        [Tooltip("Max num ber of players per room. If a room is full, a new room will be made")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("The UI Panel allows the user enter name, to connect and to play")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("The UI Label to let the user know that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        #endregion

        #region Private Fields

        //The clients version number. Allows to seperate users by gameVersion, thus can break stuff later.
        string gameVersion = "1";

        //Keep track of the current process. The connection is asynchronous and is based on several callbacks from Photon.
        //Need to track this properly to adjust the behaviour when we get a call back from Photon.
        //This will be used for OnConnectedToMaster()
        bool isConnecting;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            // Allows to use PhotonNetwork.LoadLevel() on the master client and all clients connected clients to it,
            // to sync the level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods


        // Starting the connection
        // - If we are already connected to Photon, try to join a random room
        // - If not connected, then Connect this client to the Photon Cloud Network
        public void Connect()
        {
            //keeping track when joining room we want to join room. To prevent from joining a game we just immediately left.
            //will wait for the player to want to connect again.
            isConnecting = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected) //Checks if connected or not
            {
                PhotonNetwork.JoinRandomRoom(); //Attempt to join random room. If fails, get notified in OnJoinRandomFailed().
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion; //The network now knows what version the client is on, so to match like versions.
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
             
            //Only need to load the first player, as everyone else will be Automatically Synced by Photon
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                PhotonNetwork.LoadLevel("Room for 1");
            }

        }

        #endregion
    }
}
