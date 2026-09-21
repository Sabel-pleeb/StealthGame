using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.UIElements;
using System.Timers;

public class PlayerHiding : MonoBehaviour
{
    public bool isHiding {  get; private set; }
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CinemachineCamera _FPCamera;
    [SerializeField] private Light _FPLight;
    public int hideCount {  get;  set; }

    private CabinetManager _cabinetManager;
    private CBBoxManager _cbboxManager;
    private CinemachineCamera _cabinetCamera;
    private CinemachineCamera _cbboxCamera;
    public CinemachineCamera hidingCamera;


    //  private Transform _hidingSpot;
    public bool usingCabinet = false;
    private void Awake()
    {
        hideCount = 0;
    }

    void Start()
    {
      /*  GameObject[] _cabinet = GameObject.FindGameObjectsWithTag("Cabinet");
        //_cabinetManager = _cabinet.GetComponent<CabinetManager>();
        foreach (GameObject cabinet in _cabinet)
        {
            _cabinetManager = cabinet.GetComponent<CabinetManager>();
           // _hidingSpot = cabinet.GetComponentInChildren<GamObject.Inside>();  // need to fix this, maybe add a mthod in Cabinet Manager that gets the inside ocation and grab it here 
        }
        //  _playerHidden = false;*/
    }

    void Update()
    {
        
    }

    public void SetCabinet(CabinetManager cabinet)
    {
        _cabinetManager = cabinet;

        _cabinetCamera = cabinet.CabinetCamera;
        usingCabinet = true;
      //  AudioManager.Instance.PlayAtPosition("CabinetDoor", transform.position);

        //  _hidingSpot = cabinet.HidingSpot;
    }
    public void SetBox(CBBoxManager box)
    {
        _cbboxManager = box;

        _cbboxCamera = box.BoxCamera;
        usingCabinet = false;
       // _hidingSpot = cabinet.HidingSpot;
    }


    public void Hide()
    {
        //   if (isHiding) return;
        //    isHiding = true;
        hideCount++;
          hideCameras();

          if (usingCabinet)
          {
              StartCoroutine(DoorsToggle());
          } 
     //   SetHidingCamera(true);
    }
    public void ExitHide()
    {
      //  if (!isHiding) return;
        // isHiding = false;
      //  Debug.Log("exithide");
        hidingCamera.gameObject.SetActive(false);
        _FPCamera.gameObject.SetActive(true);
        /*  hideCameras();

          if (usingCabinet)
          {
              StartCoroutine(DoorsToggle());
          } */
      //  SetHidingCamera(false);
    }

 /*   private void SetHidingCamera(bool hiding)
    {
        CinemachineCamera hidingCamera; 
        if (usingCabinet) 
        {
            hidingCamera = _cabinetCamera; 
        } 
        else 
        { 
            hidingCamera = _cbboxCamera; 
        }
        // First-person camera
        _FPCamera.gameObject.SetActive(!hiding); 
        // Hiding camera
        hidingCamera.gameObject.SetActive(hiding);
        // Tell player controller
        _playerController.SetHiding(hiding); 
        // Change transparent door/box meshes
        if (usingCabinet)
        { 
            _cabinetManager.DoorTransparency(hiding);
        }
        else
        {
            _cbboxManager.ToggleBoxTransparency(hiding);
        }
    } */

         public void hideCameras()
          {
             // CinemachineCamera hidingCamera;
              
        

              if (usingCabinet)
              {
                  hidingCamera = _cabinetCamera;
              } else
              {
                  hidingCamera = _cbboxCamera;
              }

               bool enteringHidingPlace = !hidingCamera.gameObject.activeSelf;
              _FPCamera.gameObject.SetActive(!enteringHidingPlace);
              hidingCamera.gameObject.SetActive(enteringHidingPlace);  // create a mthod for trans doors in cabinet managert that also takes this bool ? 

              _playerController.SetHiding(enteringHidingPlace);  // bool for player hiding ? 

              if (usingCabinet)
              {
                  _cabinetManager.DoorTransparency(enteringHidingPlace);
              }
              else
              {
                  _cbboxManager.ToggleBoxTransparency(enteringHidingPlace);
              }


          }

       public void lightOn()
        {
          _FPLight.gameObject.SetActive(true);
        }
        public void lightOff()
        { 
          _FPLight.gameObject.SetActive(false);
    } 

    IEnumerator DoorsToggle()
          {
              yield return new WaitForSeconds(1f);
              _cabinetManager.ToggleDoors();
          } 

    }
