using ExitGames.Client.Photon;
using Photon.Pun;
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
        private AudioSource mainAudio;

        [SerializeField]
        private float speed = 6.0f;

        [SerializeField]
        private float gravity = 3f;

        [SerializeField]
        private float jumpSpeed = 5f;

        [Tooltip("Controls how sensitive mouse inputs are")]
        [SerializeField]
        private float mouseSensitivity = 100.0f;

        [SerializeField]
        private AudioClip runningSound;

        private Vector3 move;
        private float vSpeed = 0f;

        float xRotation;
        float yRotation;

        float lastRun = 0.0f;
        float executeRate = 0.5f;

        bool running;


        #endregion Private Fields


        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            mainAudio = GetComponent<AudioSource>();
            mainAudio.clip = runningSound;
            mainAudio.loop = true;

            running = false;
            fpvModel = transform.Find("arms_assault_rifle_01").gameObject;
            fpvAnimator = fpvModel.GetComponent<Animator>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            SoundCheck();
            if (!photonView.IsMine) return;

            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
                cameraTransform.SetParent(fpvModel.gameObject.transform);
                cameraTransform.position = fpvModel.gameObject.transform.position + new Vector3(0f, 0.0869f, 0f);
                cameraTransform.rotation = fpvModel.gameObject.transform.rotation;
            }

            if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.visible = !Cursor.visible; // toggle visibility
                if (Cursor.visible)
                { // if visible, unlock
                    Cursor.lockState = CursorLockMode.None;
                    
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }


            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            animator.SetFloat("Horizontal", x);
            animator.SetFloat("Vertical", z);

            if (x != 0 || z != 0 && !Cursor.visible)
            {
                fpvAnimator.SetBool("Walk", true);
                photonView.RPC("IsRunning", RpcTarget.All, true);
            }
            else
            {
                fpvAnimator.SetBool("Walk", false);
                photonView.RPC("IsRunning", RpcTarget.All, false);
            }

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


            move = (transform.right * x + transform.forward * z) * speed;
            move.y = vSpeed;
            characterController.Move(move * Time.deltaTime);

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            yRotation += mouseX;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);


            fpvModel.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            this.gameObject.transform.Rotate(Vector3.up * mouseX);
        }


        void OnDisable()
        {
            cameraTransform.SetParent(null);
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!photonView.IsMine) return;

            if (hit.gameObject.tag == "Portal")
            {
                if (Time.time > executeRate + lastRun)
                {
                    lastRun = Time.time;
                    var collider = hit.gameObject.GetComponentInParent<PortalCollision>();
                    collider.Teleport(this.gameObject.transform);
                }
            }
            else if (hit.gameObject.tag == "Coins")
            {
                hit.transform.GetComponent<PhotonView>().RPC("Destruct", RpcTarget.All);

                float score = (float)PhotonNetwork.LocalPlayer.CustomProperties[ClipperGate.PLAYER_SCORE];
                Hashtable newScore = new Hashtable { { ClipperGate.PLAYER_SCORE, (score + 25f) } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(newScore);
            }

        }
        
        
        #endregion


        #region Private Methods

        private void SoundCheck()
        {
            if (running)
            {
                if (!mainAudio.isPlaying)
                {
                    mainAudio.Play();
                }
            } else
            {
                mainAudio.Pause();
            }
        }

        #endregion


        #region RPC
        [PunRPC]
        private void IsRunning(bool state)
        {
            running = state;
        }
        #endregion

    }
}
