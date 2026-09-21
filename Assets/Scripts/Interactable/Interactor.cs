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
    }
    private void Update()
    {

        DoInteractionTest(out IInteractable interactable);

        if (interactable != _currentInteractable)
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.FocusLost();
                interactText.enabled = false;

            }

            if (interactable != null)
            {
                interactable.FocusGained();
                interactText.enabled = true;
            }

            _currentInteractable = interactable;
        }

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

        Debug.DrawRay(
        playerCamera.transform.position,
        playerCamera.transform.forward * 10f,
        Color.green, 2f
        );


        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hitInfo, _castDistance))  // if hit something, store in hitInfo
        {
            interactable = hitInfo.collider.GetComponent<IInteractable>();  // check if the hitInfo gameObject contains any IInteractables inside it


            if (interactable != null)
            {

                return true;
            }

            return false;
        }
        return false; 
    }
}
