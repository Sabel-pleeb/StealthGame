/*using UnityEngine;
using DG.Tweening;

public class CabinetDoor : MonoBehaviour, IInteractable  // eg door 
{
    [SerializeField] private Vector3 _targetRotation = new Vector3(0, -100f, 0f);
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private CabinetDoor2 _otherDoor;

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
            _targetRotation,
            _rotationSpeed,
            RotateMode.WorldAxisAdd
        );
            return;
        }

        transform.DORotate(
            -_targetRotation,
            _rotationSpeed,
            RotateMode.WorldAxisAdd
        );

        _isOpen = true;
    }
} */

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CabinetDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private CabinetManager cabinetManager;
    private Outline outline;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] NavMeshObstacle navMeshObstacle;


    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 2f;
        outline.enabled = false;
        navMeshObstacle = GetComponent<NavMeshObstacle>();

    }
    public bool CanInteract()
    {
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        cabinetManager.ToggleDoors();

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

    public void isNavMesh(bool isEntityOpening)
    {
        if (isEntityOpening) navMeshObstacle.enabled = false;
        else navMeshObstacle.enabled = true;
    }
}
