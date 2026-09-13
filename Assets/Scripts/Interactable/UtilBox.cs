using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class UtilBox : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 _targetRotation = new Vector3(0, -100f, 0f);
    [SerializeField] private float _rotationSpeed = 1f;
    private Quaternion _closedRotation;
    public bool _isOpen;
    private Outline outline;
    // [SerializeField] private GameObject _gameObject;


    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 2f;
        outline.enabled = false;

        _closedRotation = transform.localRotation;
    }
    public bool CanInteract()
    {
       // Debug.Log("NEAR INTERACTABLE OBJECT");
        return true; //eg something that can always be interacted with 
    }

    public bool Interact(Interactor interactor)   //result of interaction or checks 
    {
        DoorToggle();

        return true; // if interaction finished ?
    }

    public void FocusGained()
    {
        outline.enabled = true;
    }
    public void FocusLost()
    {
        outline.enabled = false;
    }

    public void DoorToggle()
    {
        if (_isOpen)
        {

            close();
        }
        else
        {
            open();

        }

        // _isOpen = !_isOpen;
    }

    public void open()
    {
        if (_isOpen) return;
        _isOpen = true;
         transform.DORotate(-_targetRotation, _rotationSpeed, RotateMode.WorldAxisAdd);

    }
    private void close()
    {
        if (!_isOpen) return;
        _isOpen = false;
        transform.DORotate(_targetRotation, _rotationSpeed, RotateMode.WorldAxisAdd);


    }

}
