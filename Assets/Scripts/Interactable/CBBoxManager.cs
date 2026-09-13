using DG.Tweening;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine;

public class CBBoxManager : MonoBehaviour
{

    [SerializeField] private GameObject Box_Norm;
    [SerializeField] private GameObject Box_Transf;


    [SerializeField] private CinemachineCamera boxCamera;
  //  [SerializeField] private Transform hidingSpot;

    public CinemachineCamera BoxCamera => boxCamera;
  //  public Transform HidingSpot => hidingSpot;


    [SerializeField] private float rotationSpeed = 0.5f;

    public GameObject _player;
    public PlayerHiding _playerHiding;

    public bool isInBox; // { get; private set; }


    private void Awake()
    {
        // PERFECTTTT
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        _playerHiding = _player.GetComponent<PlayerHiding>();

        Box_Norm.SetActive(true);
        Box_Transf.SetActive(false);
    }


    public void inBox()
    {
        _playerHiding.SetBox(this);  // add script for box to player hding 
        _playerHiding.Hide();
    //    AudioManager.Instance.PlayAtPosition("CabinetDoor", transform.position);


    }


    public void ToggleBoxTransparency(bool transparent)
    {

        Box_Norm.SetActive(!transparent);
        Box_Transf.SetActive(transparent);

        if (!transparent) StartCoroutine(routine());
    }

    IEnumerator routine()
    {
        //  yield return new WaitForSeconds(0.5f);
        _playerHiding.lightOff();
        yield return new WaitForSeconds(0.8f);
        _playerHiding.lightOn();
    }
}

