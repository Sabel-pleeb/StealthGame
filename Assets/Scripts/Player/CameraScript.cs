using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

// not currently in use (in scene) because im not using cinemachine as of right now 
public class CameraScript : MonoBehaviour  //Add to a camera manager and add a top down and player camera 
{

    public CinemachineCamera camera1;
    public CinemachineCamera camera2;


    private void Start()
    {

        camera1.gameObject.SetActive(true);
        camera2.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            camera1.gameObject.SetActive(!camera1.gameObject.activeSelf);
            camera2.gameObject.SetActive(!camera2.gameObject.activeSelf);
        }
    }
}