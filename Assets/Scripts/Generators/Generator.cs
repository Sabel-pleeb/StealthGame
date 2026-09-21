using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Generator : MonoBehaviour, IInteractable
{
    public bool repaired = false;
    private GeneratorManager generatorManager;
    private Outline outline;
    public AudioSource audioSource;
    public AudioSource audioSourceMonster;
    [SerializeField] private GameObject _switch;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 2f;
        outline.enabled = false;
    }
    private void Start()
    {
        generatorManager = FindFirstObjectByType<GeneratorManager>();
    }

    public bool CanInteract()
    {
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        if (!repaired)
        {
            AudioManager.Instance.Play("Switch", audioSource);
            AudioManager.Instance.Play("Growl", audioSourceMonster);
            _switch.transform.DOLocalRotate(new Vector3(5f, 0f, 0f), 0.3f).SetEase(Ease.OutBack);
            repaired = true;
            generatorManager.GeneratorCompleted();
            generatorManager.GeneratorRepaired(this);
            return true;
        }
       else return false;
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