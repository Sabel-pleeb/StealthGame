using UnityEngine;
using Unity.Cinemachine;


//on camera holder, switch between FPCam and top down, 
public class MoveCam : MonoBehaviour  
{
    public Transform cameraPosition;

    public CinemachineCamera FPCam;
    public CinemachineCamera TopDownCam;

    private void Start()
    {
        FPCam.gameObject.SetActive(true);
        TopDownCam.gameObject.SetActive(false); // for chescking scene, wont be in full game 
    }
    void Update()
    {

        if (FPCam.isActiveAndEnabled)  
        {
            FPCam.transform.position = cameraPosition.position; // TopDownCam changes position with this, need it to be just one position
        } 

    }
}
