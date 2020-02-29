using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class PortalCollision : MonoBehaviour
    {

        private PortalManager parentScript;

        public void Initialise(PortalManager portalManager)
        {
            parentScript = portalManager;
        }

        public void Teleport(Transform character)
        {

            parentScript.HandleCollision(this.gameObject, character);            
        }

    }
}
