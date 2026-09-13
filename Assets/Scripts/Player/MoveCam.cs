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
        TopDownCam.gameObject.SetActive(false);
    }
    void Update()
    {
      /*  if (Input.GetKeyDown(KeyCode.C))
        {
            FPCam.gameObject.SetActive(!FPCam.gameObject.activeSelf);
            TopDownCam.gameObject.SetActive(!TopDownCam.gameObject.activeSelf);
           // Debug.Log(Camera.current + " is active ");
        } */

        if (FPCam.isActiveAndEnabled)  
        {
            FPCam.transform.position = cameraPosition.position; // TopDownCam changes position with this, need it to be just one position
        } 
     /*   if (TopDownCam.isActiveAndEnabled)  
        {
            FPCam.transform.position = cameraPosition.position; // TopDownCam changes position with this, need it to be just one position
        } */
    }
}
