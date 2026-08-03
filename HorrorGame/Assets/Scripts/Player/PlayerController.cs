using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.WSA;
using UnityEditorInternal;

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
    public LayerMask whatIsGround; 
    public bool grounded;  */

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private Generator currentGenerator;

    public MovementState state;  //always store state player is in currently 
    public enum MovementState
    {
        walking,
        sprinting,
        crouching
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = groundDrag;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        //ground check
    //    grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        //handle drag 
        /*  if (grounded) rb.linearDamping = groundDrag;
          else rb.linearDamping = 0; */

        if (Input.GetKeyDown(KeyCode.E) && currentGenerator != null)
        {
           // currentGenerator.repaired = true;
            currentGenerator.Repair(); // and add visual cue here
           // Debug.Log("PLAYER CONTROLLER E PRESSED FOR GEN");
        }

    }

    private void FixedUpdate()
    {
        MovePlayer();
        Debug.Log("player state = " + state);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);  // this shrinks player ugh mayne change to just change camPos
            rb.AddForce(Vector3.down * 5, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);  // this shrinks player ugh mayne change to just change camPos
        }
    }

    private void StateHandler()
    {
        if (Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        } 
        else
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //limit velocity 
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Generator generator = other.GetComponent<Generator>();

        if (generator != null)
            currentGenerator = generator;  // enable visual 'press e'
            Debug.Log("within range of generator");
    }

    private void OnTriggerExit(Collider other)
    {
        Generator generator = other.GetComponent<Generator>();

        if (generator == currentGenerator)
            currentGenerator = null;  // disable visual 'press e'
    }
}
