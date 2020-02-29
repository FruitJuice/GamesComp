using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class FXManager : MonoBehaviour
    {
        [System.Serializable]
        public class prefabs
        {
            [Header("Prefabs")]
            public GameObject playerImpact;
            public GameObject otherImpact;
        }
        public prefabs Prefabs;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        [PunRPC]
        private void ShotPlayer(Vector3 origin, Vector3 hitPoint)
        {
            Transform temp = Prefabs.playerImpact.transform;
            temp.transform.position = hitPoint;
            
            var temp2 = Instantiate(Prefabs.playerImpact, temp.transform.position, temp.transform.rotation);
            temp2.transform.LookAt(origin);

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = origin;
            //Prefabs.muzzleParticles.Emit(emitParams, 1);
        }

        [PunRPC]
        private void ShotOther(Vector3 origin, Vector3 hitPoint)
        {
            Transform temp = Prefabs.otherImpact.transform;
            temp.transform.position = hitPoint;
            var temp2 = Instantiate(Prefabs.otherImpact, temp.transform.position, temp.transform.rotation);
            temp2.transform.LookAt(origin);

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = origin;
            //Prefabs.muzzleParticles.Emit(emitParams, 1);
        }
    }
}
