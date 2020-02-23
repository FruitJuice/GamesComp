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
        private Transform cameraTransform;

        [SerializeField]
        private float speed = 6.0f;

        [SerializeField]
        private float gravity = -1.0f;

        [Tooltip("Controls how sensitive mouse inputs are")]
        [SerializeField]
        private float mouseSensitivity = 100.0f;

        private Vector3 velocity;

        float xRotation;
        float yRotation;


        #endregion Private Fields


        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
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

            //checking if the player is running before allowing to jump
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetTrigger("Jump");
                //moveDirection = new Vector3(0f, 8f, 0f);
            }

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

            Vector3 move = transform.right * x + transform.forward * z;

            //moveDirection *= speed;
            
            //if (!characterController.isGrounded) { 
            //    moveDirection += Physics.gravity;
            //}

            

            //moveDirection.y -= gravity * Time.deltaTime;
            characterController.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            characterController.Move(velocity * Time.deltaTime);



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



        #endregion


    }
}
