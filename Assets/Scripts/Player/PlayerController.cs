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
    public float crouchSpeed;  
    public float crouchYScale;
    private float startYScale;

    public float groundDrag = 6;

    public Transform orientation;
    public CinemachineCamera FPCam;

    float horizontalInput;
    float verticalInput;

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

    public MovementState state;  //always store state player is in currently 
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        crouching,
        caught,
        hiding
    }
    void Awake()
    {
        gameOverScreen.SetActive(false);
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
        if (uiManager.startScreen.activeInHierarchy) return;
        MyInput();
        SpeedControl();
        StateHandler();

    }

    private void FixedUpdate()
    {
        MovePlayer();
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
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);  
            rb.AddForce(Vector3.down * 5, ForceMode.Impulse);
        } else
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);  

        }
    }

    private void StateHandler()
    {

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

        if (isHiding)
            _playerHiding.ExitHide();
        Env.gameObject.SetActive(false);
        FPCam.gameObject.SetActive(false); 
        cam.gameObject.SetActive(true);
        
        StartCoroutine(GameOver());
    }

    public void SetHiding(bool hiding)  
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
            //calculate movement direction
            Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput);
            movement = FPCam.transform.TransformDirection(movement);
            movement.y = 0;
            rb.AddForce(10f * moveSpeed * movement.normalized, ForceMode.Force);
    }

    private void SpeedControl()
    {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

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
            currentGenerator = generator;  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Generator generator = other.GetComponent<Generator>();

        if (generator == currentGenerator)
            currentGenerator = null;  
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.8f);
        gameOverScreen.SetActive(true);
    }
}
