using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class PlayerMovementManager : MonoBehaviourPunCallbacks
    {

        #region Private Fields

        private Animator animator;
        private Animator fpvAnimator;
        private GameObject fpvModel;
        private CharacterController characterController;
        private PortalManager portManager;
        private Transform cameraTransform;

        [SerializeField]
        private float speed = 6.0f;

        [SerializeField]
        private float gravity = 3f;

        [SerializeField]
        private float jumpSpeed = 5f;

        [Tooltip("Controls how sensitive mouse inputs are")]
        [SerializeField]
        private float mouseSensitivity = 100.0f;

        private Vector3 move;
        private float vSpeed = 0f;

        float xRotation;
        float yRotation;

        float lastRun = 0.0f;
        float executeRate = 0.5f;


        #endregion Private Fields


        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            portManager = this.transform.Find("arms_assault_rifle_02").gameObject.GetComponent<PortalManager>();
            fpvModel = this.transform.Find("arms_assault_rifle_02").gameObject;
            fpvAnimator = fpvModel.GetComponent<Animator>();

        }

        // Update is called once per frame
        void Update()
        {
            //we only want to animate the local player, not every single other player that appears
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
                cameraTransform.SetParent(fpvModel.gameObject.transform);
                cameraTransform.position = fpvModel.gameObject.transform.position + new Vector3(0f, 0.0869f, 0f);
                cameraTransform.rotation = fpvModel.gameObject.transform.rotation;
            }

            //deal with jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


            

            if (!animator)
            {
                return;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            if (x != 0 || z != 0)
            {
                fpvAnimator.SetBool("Walk", true);
            } else fpvAnimator.SetBool("Walk", false);



            animator.SetFloat("Horizontal", x);
            animator.SetFloat("Vertical", z);


            //moveDirection = new Vector3(x, 0.0f, z);
            //moveDirection = Camera.main.transform.TransformDirection(moveDirection);

            move = ((transform.right * x) + (transform.forward * z)) * speed;

            //moveDirection *= speed;
            if (characterController.isGrounded)
            {
                vSpeed = 0;
                if (Input.GetButtonDown("Jump"))
                {
                    vSpeed = jumpSpeed;
                    //animator.SetTrigger("Jump");
                }

            }




            vSpeed -= gravity * Time.deltaTime;
            move.y = vSpeed;
            //move.y -= gravity * Time.deltaTime;
            characterController.Move(move * Time.deltaTime);

            //velocity.y += gravity * Time.deltaTime;





            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            yRotation += mouseX;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);


            fpvModel.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            //cameraTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            this.gameObject.transform.Rotate(Vector3.up * mouseX);


            //fpvModel.transform.Rotate(Vector3.up * mouseX);

            //cameraTransform.transform.position = transform.position + new Vector3(0f, 1.3f, 0f);


            //animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (photonView.IsMine)
            {
                if(hit.gameObject.tag == "Portal")
                {
                    if (Time.time > executeRate + lastRun)
                    {
                        lastRun = Time.time;
                        var collider = hit.gameObject.GetComponent<PortalCollision>();
                        collider.Teleport(characterController);
                    }
                }
                
            }
        }


        #endregion


    }
}
