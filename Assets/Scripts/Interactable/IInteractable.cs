
public interface IInteractable 
{
    public bool CanInteract();

    public bool Interact(Interactor interactor);

    public void FocusGained();

    public void FocusLost();
}
