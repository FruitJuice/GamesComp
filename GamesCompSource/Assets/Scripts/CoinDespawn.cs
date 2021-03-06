﻿using Photon.Pun;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class CoinDespawn : MonoBehaviourPunCallbacks
    {

        [PunRPC]
        private void Destruct()
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.photonView);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.CompareTag("Player")) GetComponent<BoxCollider>().enabled = false;
        }
    }
}
