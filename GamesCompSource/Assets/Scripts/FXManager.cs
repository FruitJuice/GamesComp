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
            public GameObject bulletPrefab;
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
            var ParticleEffect = Instantiate(Prefabs.playerImpact, hitPoint, Prefabs.playerImpact.transform.rotation);
            ParticleEffect.transform.LookAt(origin);

            Vector3 dir = (hitPoint - origin).normalized;
            var BulletEffect = Instantiate(Prefabs.bulletPrefab, origin, Quaternion.identity);
            BulletEffect.GetComponent<Rigidbody>().velocity = dir * 200;


            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = origin;
            //Prefabs.muzzleParticles.Emit(emitParams, 1);
        }

        [PunRPC]
        private void ShotOther(Vector3 origin, Vector3 hitPoint)
        {
            var ParticleEffect = Instantiate(Prefabs.otherImpact, hitPoint, Prefabs.otherImpact.transform.rotation);
            ParticleEffect.transform.LookAt(origin);

            Vector3 dir = (hitPoint - origin).normalized;
            var BulletEffect = Instantiate(Prefabs.bulletPrefab, origin, Quaternion.identity);
            BulletEffect.GetComponent<Rigidbody>().velocity = dir * 200;

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = origin;
            //Prefabs.muzzleParticles.Emit(emitParams, 1);
        }

        [PunRPC]
        private void ShootNoCollision(Vector3 origin, Vector3 hitPoint)
        {
            Vector3 dir = (hitPoint - origin).normalized;
            var BulletEffect = Instantiate(Prefabs.bulletPrefab, origin, Quaternion.identity);
            BulletEffect.GetComponent<Rigidbody>().velocity = dir * 200;

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = origin;
            //Prefabs.muzzleParticles.Emit(emitParams, 1);
        }
    }
}
