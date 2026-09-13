using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
   [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    [Header("Crouching")]
    public float crouchSpeed;  // put in movement header instead ?? 
    public float crouchYScale;
    private float startYScale;

    public float groundDrag = 6;

  /*  [Header("Ground Check")]  // probs just for jumping mechanics which i most likely won't use 
    public float playerHeight;
    public LayerMask moveable; 
    public bool grounded;   */

    public Transform orientation;
    public CinemachineCamera FPCam;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    public bool isHiding;
    public bool isCaught;
    public bool isJumpscareAudioOn;

    Rigidbody rb;

    private Generator currentGenerator;
    public PlayerHiding _playerHiding;
    [SerializeField] public GameObject gameOverScreen;
    public UIManager uiManager;
    [SerializeField] public GameObject Widgets;
    public GameObject Env;
   // public GameManager gameManager;

    public MovementState state;  //always store state player is in currently 
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        crouching,
        caught,
        hiding// add hiding state ? 
    }
    void Awake()
    {
        gameOverScreen.SetActive(false);
       // Widgets.SetActive(true);
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = groundDrag;
        isHiding = false;
        isCaught = false;
        isJumpscareAudioOn = false;

        startYScale = transform.localScale.y;
        moveSpeed = walkSpeed;
    }

    void Update()
    {
        //ground check
        //  grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, moveable);
        if (uiManager.startScreen.activeInHierarchy) return;
        MyInput();
        SpeedControl();
        StateHandler();

        //handle drag 
       /*   if (grounded) rb.linearDamping = groundDrag;
          else rb.linearDamping = 0; */

      /*  if (Input.GetKeyDown(KeyCode.E) && currentGenerator != null)
        {
           // currentGenerator.repaired = true;
            currentGenerator.Repair(); // and add visual cue here
           // Debug.Log("PLAYER CONTROLLER E PRESSED FOR GEN");
        } */

    }

   /* public MovementState State()
    {
        switch (state)
        {
            case MovementState.idle:
                {
                    moveSpeed = 0;
                    return MovementState.idle;
                }
            case MovementState.walking:
                {
                    moveSpeed = walkSpeed;
                    return MovementState.walking;
                }
            case MovementState.sprinting:
                {
                    moveSpeed = sprintSpeed;
                    return MovementState.sprinting;
                }
            case MovementState.crouching:
                {
                    moveSpeed = crouchSpeed;
                    return MovementState.crouching;
                }
            case MovementState.hiding:
                {
                    return MovementState.hiding;
                }
            default:
                return MovementState.idle;

        } 
    } */

    private void FixedUpdate()
    {
        MovePlayer();
    //    Debug.Log("player state = " + state);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(crouchKey)) // change to toggle ? 
        {
            Crouch(true);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            Crouch(false);
        }
    }

    public void Crouch(bool isCrouching)
    {
        if (isCrouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);  // this shrinks player ugh mayne change to just change camPos
            rb.AddForce(Vector3.down * 5, ForceMode.Impulse);
        } else
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);  // this shrinks player ugh mayne change to just change camPos

        }
    }

    private void StateHandler()
    {
        // if (uiManager.startScreen.activeInHierarchy) return;

        if (isCaught)
        {
            state = MovementState.caught;
            moveSpeed = 0f;
        }
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (isHiding)
        {

            state = MovementState.hiding;
            // increment a hiding count here 
        }
        else if (horizontalInput != 0 || verticalInput != 0)
        {
            state = MovementState.walking;  
            moveSpeed = walkSpeed;
        }
        else if (Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else
        {
            state = MovementState.idle;
            //moveSpeed = 0;
        }
    }

    public void Jumpscare(CinemachineCamera cam)
    {
        if (!isCaught) return;
      //  AudioManager.Instance.StopAll();
       /* if (!isJumpscareAudioOn)
        {
            isJumpscareAudioOn = true;
            AudioManager.Instance.PlayAtPosition("Jumpscare", transform.position);
        } */

        // gameManager.Bleh();  // change to gameManager ?
        // _playerHiding.hidingCamera.gameObject.SetActive(false);
        if (isHiding)
            _playerHiding.ExitHide();
        Env.gameObject.SetActive(false);
    //    Debug.Log("Jumpscare method");  // problem here is cameras. FPCam is already not active
        FPCam.gameObject.SetActive(false); // need to change to current cam
        cam.gameObject.SetActive(true);
        
        StartCoroutine(GameOver());
    }

    public void SetHiding(bool hiding)  // can i use this ?
    {
        isHiding = hiding;

        if (hiding)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void MovePlayer()
    {
        if (isHiding || isCaught) return;
     //   {
            //calculate movement direction
            Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput);
            movement = FPCam.transform.TransformDirection(movement);
            movement.y = 0;
            rb.AddForce(10f * moveSpeed * movement.normalized, ForceMode.Force);
            //moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            // rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
       // }
    }

    private void SpeedControl()
    {

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //limit velocity 
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Generator generator = other.GetComponent<Generator>();

        if (generator != null)
        {
            currentGenerator = generator;  // enable visual 'press e'
          //  Debug.Log("within range of generator");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Generator generator = other.GetComponent<Generator>();

        if (generator == currentGenerator)
            currentGenerator = null;  // disable visual 'press e'
    }

    IEnumerator GameOver()
    {
       // AudioManager.Instance.PlayAtPosition("Jumpscare", transform.position);

        yield return new WaitForSeconds(1.8f);
        gameOverScreen.SetActive(true);


    }
}
