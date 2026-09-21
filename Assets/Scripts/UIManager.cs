using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public CanvasGroup canvas;
    public Transform uiElement;
    public Image image;
    //public bool visible = false;
    private Tween pulseTween;
    private Tween fadeTween;
    public GameObject entity;
    public Transform destination;
    public GameObject startScreen;

    public GameObject Vision;
    public GameObject interact;
    public int progressAmount;
    public GameObject gameWinScreen;

    [SerializeField] GameObject light1;
    [SerializeField] GameObject light2;
    [SerializeField] GameObject light3;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSource2;

    private EntityVision.VisionConeType currentCone;


    private void Awake()
    {
        light1.gameObject.SetActive(false);
        light2.gameObject.SetActive(false);
        light3.gameObject.SetActive(false);
        canvas.alpha = 0.5f;
        uiElement.localScale = Vector3.one;
        Vision.SetActive(true); 
        interact.SetActive(true);
        progressAmount = 0;
        gameWinScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    private void Update()
    {
        if (startScreen.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
        {
            startScreen.SetActive(false);
        }
    }

    public void VisibleUI(EntityVision.VisionConeType coneType)
    {
        if (currentCone == coneType) return;
        currentCone = coneType;

        pulseTween?.Kill(); // stops prev animation
        fadeTween?.Kill();
        switch (coneType)
        {
            case
            EntityVision.VisionConeType.Wide:
                fadeTween = canvas.DOFade(1f, 0.3f);
                uiElement.localScale = Vector3.one; pulseTween = uiElement.DOScale(1.5f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                image.color = Color.yellow;//new Color(255, 165, 0);
                AudioManager.Instance.Play("DetectionSound", audioSource);
                break;
            case
            EntityVision.VisionConeType.Close:
                fadeTween = canvas.DOFade(1f, 0.2f);
                uiElement.localScale = Vector3.one;
                pulseTween = uiElement.DOScale(1.3f, 0.25f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                image.color = Color.red;
                break;
            case
            EntityVision.VisionConeType.Normal:
                fadeTween = canvas.DOFade(1f, 0.3f);
                uiElement.localScale = Vector3.one;
                pulseTween = uiElement.DOScale(1.3f, 0.25f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine); 
                image.color = Color.red;
                break;
            case
            EntityVision.VisionConeType.HidingPlaces:
                fadeTween = canvas.DOFade(1f, 0.3f);
                uiElement.localScale = Vector3.one;
                pulseTween = uiElement.DOScale(0.95f, 0.95f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine); 
                image.color = Color.white;
                break; 
            case
            EntityVision.VisionConeType.WideLight:
                fadeTween = canvas.DOFade(1f, 0.3f);
                uiElement.localScale = Vector3.one;
                pulseTween = uiElement.DOScale(0.95f, 0.95f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine); 
                image.color = Color.yellow;
                AudioManager.Instance.Play("DetectionSound", audioSource);
                break; 
            default:
                fadeTween = canvas.DOFade(1f, 0.3f);
                uiElement.localScale = Vector3.one;
                image.color = Color.white;
                return;
        }

    }

    public void teleport()
    {
        entity.SetActive(false);
        entity.transform.position = destination.position;
        entity.SetActive(true);
    }

    private void OnDestroy()
    {       
       pulseTween?.Kill();    
        fadeTween.Kill();
    }

    public void WidgetsOff()
    {
        Vision.SetActive(false);
        interact.SetActive(false);
    }



    public void gameWin()
    {
        WidgetsOff();
        AudioManager.Instance.StopAll();
        gameWinScreen.SetActive(true);
    }

    public void lightProgressDoor()
    {
        if (!light1.activeInHierarchy && !light2.activeInHierarchy && !light3.activeInHierarchy)
        {
            light1.gameObject.SetActive(true);
        }
        else if (light1.activeInHierarchy && !light2.activeInHierarchy && !light3.activeInHierarchy)
        {
            light2.gameObject.SetActive(true);
        }
        else if (light1.activeInHierarchy && light2.activeInHierarchy && !light3.activeInHierarchy)
        {
            light3.gameObject.SetActive(true);
        }
        else return;


    }

    public void RestartScene()
    {
        try
        {
            // Get the active scene's name
            string currentSceneName = SceneManager.GetActiveScene().name;

            // Reload the scene
            SceneManager.LoadScene(currentSceneName);
        }
        catch (System.Exception ex)
        {
          //  Debug.LogError("Failed to restart scene: " + ex.Message);
        }
    }
}
