using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class Interactor : MonoBehaviour
{
    // use raycast to check whether object is interactable 

    [SerializeField] private float _castDistance = 5f;
    [SerializeField] private Vector3 _raycastOffset = new Vector3(0, 1f, 0);
    [SerializeField] CinemachineCamera playerCamera;
    private IInteractable _currentInteractable;
    [SerializeField] TextMeshProUGUI interactText;
    private Outline outline;

    private void Awake()
    {
        interactText.enabled = false;
      /*  outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 1f;
        outline.enabled = false;*/
        // player = GetComponent<PlayerController>();
    }
    private void Update()
    {
        //    Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        //   Debug.DrawLine(transform.position, forward, Color.red);

        //   if (Input.GetKeyDown(KeyCode.E))
        // {
        /* if(DoInteractionTest(out IInteractable interactable))  // swap this and E ?
         {
           interactable.FocusGained();  // need focus lost 

           if (Input.GetKeyDown(KeyCode.E))
           {
               if (interactable.CanInteract())
               {
                   interactable.Interact(this);
               }
           }
           //   } 
         }  */

        DoInteractionTest(out IInteractable interactable);

        if (interactable != _currentInteractable)
        {
            // We stopped looking at the previous object
            if (_currentInteractable != null)
            {
                _currentInteractable.FocusLost();
                interactText.enabled = false;

            }

            // We started looking at a new object
            if (interactable != null)
            {
                interactable.FocusGained();
                interactText.enabled = true;
            }

            _currentInteractable = interactable;
        }

        // Interaction input
        if (_currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            if (_currentInteractable.CanInteract())
            {
                _currentInteractable.Interact(this);
            }
        }


    }

    private bool DoInteractionTest(out IInteractable interactable) //output variable, needs initialising
    {
        interactable = null;

     //   Vector3 rayOrigin = playerCamera.transform.position; // + _raycastOffset;
    //    Vector3 rayDirection = playerCamera.transform.forward;
      //  Ray ray = new Ray(rayOrigin, transform.forward); //casting ray foreward
      //  Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
     //   Debug.DrawLine(rayOrigin, rayDirection * _castDistance, Color.red, 2f);
        Debug.DrawRay(
        playerCamera.transform.position,
        playerCamera.transform.forward * 10f,
        Color.green, 2f
        );


        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hitInfo, _castDistance))  // if hit something, store in hitInfo
        {
            interactable = hitInfo.collider.GetComponent<IInteractable>();  // check if the hitInfo gameObject contains any IInteractables inside it
         //   interactText.enabled = true;

            if (interactable != null)
            {
              //  Debug.Log("Hit something interactable ");
             //   interactText.enabled = false;
                return true;
            }
          //  Debug.Log("HIT something not interactable");
          //  interactText.enabled = false;
            return false;
        }
     //   Debug.Log("Didnt hit anything");
      //  interactable.FocusLost();

        //   interactText.enabled = false;
        return false; 
    }
}
