using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class GrenadeLogicScript : MonoBehaviour
    {

        [Header("Force")]
        public float min = 400f;
        public float max = 600f;
        private float force;

        [Header("Audio")]
        public AudioSource impactSound;

        private void Awake()
        {
            force = Random.Range(min, max);

            //Random rotation of the grenade
            GetComponent<Rigidbody>().AddRelativeTorque(Random.Range(500, 1500), Random.Range(0, 0), Random.Range(0, 0) * Time.deltaTime * 5000);
        }

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * force);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}