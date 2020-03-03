using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class PortalCollision : MonoBehaviour
    {

        private PortalManager parentScript;
        private GameObject parent;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Grenade"))
            {
                Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<BoxCollider>());
                ContactPoint contactPoint = collision.contacts[0];
                parentScript.TeleportGrenade(parent, collision.gameObject, contactPoint.point);
            }
        }


        public void Initialise(PortalManager portalManager, GameObject parent)
        {
            parentScript = portalManager;
            this.parent = parent;
        }

        public void Teleport(Transform character)
        {

            parentScript.HandleCollision(parent, character);            
        }


    }
}
