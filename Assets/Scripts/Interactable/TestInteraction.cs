using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TestInteraction : MonoBehaviour, IInteractable  // eg door 
{
    [SerializeField] private Vector3 _targetRotation = new Vector3(0, -100f, 0f);
    [SerializeField] private float _rotationSpeed = 1f;
    private Quaternion _closedRotation;
    [SerializeField] NavMeshObstacle navMeshObstacle;
    public bool _isOpen;
    private Outline outline;
    public AudioSource audioSource;
   // [SerializeField] private GameObject _gameObject;


    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        audioSource = gameObject.GetComponent<AudioSource>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 2f;
        outline.enabled = false;
        navMeshObstacle = GetComponent<NavMeshObstacle>();

        _closedRotation = transform.localRotation;
    }
    public bool CanInteract()
    {
        return true; //eg something that can always be interacted with 
    }

    public bool Interact(Interactor interactor)   //result of interaction or checks 
    {
        DoorToggle(interactor.transform);

        return true; 
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
        AudioManager.Instance.Play("DoorCreak", audioSource);
    }

    public void open(Transform opener)
    {
        if (_isOpen) return;
        _isOpen = true;

        Vector3 directionToPlayer = opener.transform.position - transform.position; 

        float side = Vector3.Dot(transform.forward, directionToPlayer);
        float rotationDirection; if (side > 0f) { 
            rotationDirection = 1f;
        }
        else 
        {
            rotationDirection = -1f; 
        }
        Vector3 openRotation = _closedRotation.eulerAngles + (_targetRotation * rotationDirection); 
        transform.DOKill(); 
        transform.DOLocalRotate( openRotation, _rotationSpeed );
        navMeshObstacle.enabled = false; 
    }
    private void close() { 
        if (!_isOpen) return; 
        _isOpen = false; 
        transform.DOKill();
        transform.DOLocalRotateQuaternion( _closedRotation, _rotationSpeed );

        navMeshObstacle.enabled = true;
    }



}
