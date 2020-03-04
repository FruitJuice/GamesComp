using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class GrenadeLogicScript : MonoBehaviour
    {
        public float grenadeDamage = 15.0f;
        public float grenadeTimer = 2.5f;
        public float min = 1000f;
        public float max = 200f;

        public GameObject explosionPrefab;
        public float radius = 5.0f;

        public AudioSource impactSound;

        private float force;

        private void Awake()
        {
            force = Random.Range(min, max);
            GetComponent<Rigidbody>().AddRelativeTorque(Random.Range(500, 1500), Random.Range(0, 0), Random.Range(0, 0) * Time.deltaTime * 5000);
        }

        void Start()
        {
            GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * force);
            StartCoroutine(ExplosionDelay());
        }

        private void OnCollisionEnter(Collision collision)
        {
            impactSound.Play();
        }

        private IEnumerator ExplosionDelay()
        {
            yield return new WaitForSeconds(grenadeTimer);

            RaycastHit groundCheck;
            if (Physics.Raycast(transform.position, Vector3.down, out groundCheck, 50))
            {
                Instantiate(explosionPrefab, groundCheck.point, Quaternion.FromToRotation(Vector3.forward, groundCheck.normal));
            }

            
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    collider.gameObject.GetComponent<PlayerManager>().TakeGrenadeDamage(grenadeDamage);
                }

            }

            Destroy(this.gameObject);
        }

    }

}