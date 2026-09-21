using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FinalDoor : MonoBehaviour, IInteractable  
{
    [SerializeField] private Vector3 _targetRotation = new Vector3(0, -100f, 0f);
    [SerializeField] private float _rotationSpeed = 1f;
    private Quaternion _closedRotation;
    public bool _isOpen;
    public bool allCollected;
    private Outline outline;
    public AudioSource audioSource;
    public UIManager manager;


    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        audioSource = gameObject.GetComponent<AudioSource>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 2f;
        outline.enabled = false;
        allCollected = false;
        _closedRotation = transform.localRotation;
    }
    public bool CanInteract()
    {
            return true; //eg something that can always be interacted with 
    }

    public bool Interact(Interactor interactor)   //result of interaction or checks 
    {
        if (allCollected)
        {
            manager.gameWin();
            return true; // if interaction finished ?
        }
        else
        {
            AudioManager.Instance.Play("DoorLocked", audioSource);
            return false;
        }
    }

    public void FocusGained()
    {
        outline.enabled = true;
    }
    public void FocusLost()
    {
        outline.enabled = false;
    }

    public void DoorToggle(Transform opener)
    {
        if (_isOpen)
        {

            close();
        }
        else
        {
            open(opener);

        }
    }

    public void open(Transform opener)
    {
        if (_isOpen) return;

        _isOpen = true;

        Vector3 directionToPlayer = opener.transform.position - transform.position;
        // Determine which side of the door the player is on
        float side = Vector3.Dot(transform.forward, directionToPlayer);
        float rotationDirection; if (side > 0f)
        {
            // Player is in front of the door
            rotationDirection = 1f;
        }
        else
        {
            // Player is behind the door
            rotationDirection = -1f;
        }
        Vector3 openRotation = _closedRotation.eulerAngles + (_targetRotation * rotationDirection);
        transform.DOKill();
        transform.DOLocalRotate(openRotation, _rotationSpeed);
    }
    private void close()
    {
        if (!_isOpen) return;
        _isOpen = false;
        transform.DOKill();
        transform.DOLocalRotateQuaternion(_closedRotation, _rotationSpeed);

    }



}
