/*using UnityEngine;
using DG.Tweening;

public class CabinetDoor2 : MonoBehaviour, IInteractable  // eg door 
{
    [SerializeField] private Vector3 _targetRotation = new Vector3(0, -100f, 0f);
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private CabinetDoor _otherDoor;

    public bool _isOpen;
    private bool hasHidden;
    public bool CanInteract()
    {
        Debug.Log("NEAR INTERACTABLE OBJECT");
        return true; //eg something that can always be interacted with 
    }

    public bool Interact(Interactor interactor)   //result of interaction or checks 
    {
        OpenDoor();
        _otherDoor.OpenDoor();

        return true; // if interaction finished ?
    }

    public void OpenDoor()
    {
        if (_isOpen)
        {
            transform.DORotate(
            -_targetRotation,
            _rotationSpeed,
            RotateMode.WorldAxisAdd
        );
            return;
        }


        transform.DORotate(
            _targetRotation,
            _rotationSpeed,
            RotateMode.WorldAxisAdd
        );

        _isOpen = true;
    }
} */