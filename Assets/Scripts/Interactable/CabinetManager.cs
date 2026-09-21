using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CabinetManager : MonoBehaviour
{
    [SerializeField] private Transform door1;
    [SerializeField] private Transform door2;

    [SerializeField] private GameObject door1_Norm;
    [SerializeField] private GameObject door1_Transf;
    [SerializeField] private GameObject door2_Norm;
    [SerializeField] private GameObject door2_Transf;

    [SerializeField] private Vector3 door1OpenRotation = new Vector3(0, -90f, 0f);
    [SerializeField] private Vector3 door2OpenRotation = new Vector3(0, -90f, 0f);

    [SerializeField] private CinemachineCamera cabinetCamera;
    private Outline outline;
    public bool isAI;
    public AudioSource audioSource;

    public CinemachineCamera CabinetCamera => cabinetCamera;


    [SerializeField] private float rotationSpeed = 0.5f;

    public GameObject _player;
    public PlayerHiding _playerHiding;

    public bool _isOpen {  get; private set; }

    private void Awake()
    {
        // PERFECTTTT
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerHiding = _player.GetComponent<PlayerHiding>();
        audioSource = GetComponent<AudioSource>();

        door1_Norm.SetActive(true);
        door2_Norm.SetActive(true);
        door1_Transf.SetActive(false);
        door2_Transf.SetActive(false);

        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 1f;
        outline.enabled = false;
    }

    public void Start()
    {
        isAI = false;
        _isOpen = false;
    }
    public void ToggleDoors()
    {
        if (_isOpen)
        {
            CloseDoors();
        }
        else
        {
            OpenDoors();
        }

    }

    public void OpenDoors()
    {      
            door1.GetComponent<BoxCollider>().enabled = false;
            door2.GetComponent<BoxCollider>().enabled = false;
            door1.DORotate(-door1OpenRotation, rotationSpeed, RotateMode.WorldAxisAdd);
            door2.DORotate(-door2OpenRotation, rotationSpeed, RotateMode.WorldAxisAdd);


            if (_playerHiding != null)
            {
            AudioManager.Instance.Play("CabinetDoor", audioSource);
            _playerHiding.SetCabinet(this);
                _playerHiding.Hide();
            }

        _isOpen = true;
        
    }

    public void CloseDoors()
    {
        door1.GetComponent<BoxCollider>().enabled = true;
        door2.GetComponent<BoxCollider>().enabled = true;
        door1.DORotate(door1OpenRotation, rotationSpeed, RotateMode.WorldAxisAdd);
        door2.DORotate(door2OpenRotation, rotationSpeed, RotateMode.WorldAxisAdd);

        _isOpen = false;
    }


    public void OpenDoorsAI()
    {
        if (_isOpen)
            return;

        isAI = true;
        AudioManager.Instance.Play("CabinetDoor", audioSource);

        door1.DOKill();
        door2.DOKill();
        door1_Transf.transform.DOKill();
        door2_Transf.transform.DOKill();
        door1.GetComponent<BoxCollider>().enabled = false;
        door2.GetComponent<BoxCollider>().enabled = false;

        if (door1.gameObject.activeSelf && door2.gameObject.activeSelf)
        {
            door1.DORotate(
                -door1OpenRotation,
                rotationSpeed,
                RotateMode.WorldAxisAdd
            );

            door2.DORotate(
                -door2OpenRotation,
                rotationSpeed,
                RotateMode.WorldAxisAdd
            );
        }
        else
        {
            door1_Transf.GetComponent<BoxCollider>().enabled = false;
            door2_Transf.GetComponent<BoxCollider>().enabled = false;
            door1_Transf.transform.DORotate(
                -door1OpenRotation,
                rotationSpeed,
                RotateMode.WorldAxisAdd
            );

            door2_Transf.transform.DORotate(
                -door2OpenRotation,
                rotationSpeed,
                RotateMode.WorldAxisAdd
            );
        }
        _isOpen = true;
    }

    public void DoorTransparency(bool inCabinet)
    {
        if (inCabinet)
        {
            door1_Transf.SetActive(true);
            door2_Transf.SetActive(true);
            door1_Norm.SetActive(false);
            door2_Norm.SetActive(false);
        } else
        {
            door1_Transf.SetActive(false);
            door2_Transf.SetActive(false);
            door1_Norm.SetActive(true);
            door2_Norm.SetActive(true);
            StartCoroutine(routine());
        }
    }

    IEnumerator routine()
    {
        _playerHiding.lightOff();
        yield return new WaitForSeconds(0.8f);
        _playerHiding.lightOn();
    }
}