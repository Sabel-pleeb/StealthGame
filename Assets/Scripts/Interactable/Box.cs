using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    public CBBoxManager _cbboxManager;
    private Outline outline;
    [SerializeField] private GameObject _gameObject;

    private void Awake()
    {
        outline = _gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 2f;
        outline.enabled = false;
    }
    public bool CanInteract()
    {
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        _cbboxManager.inBox();

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
}
