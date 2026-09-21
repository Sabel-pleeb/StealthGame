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
