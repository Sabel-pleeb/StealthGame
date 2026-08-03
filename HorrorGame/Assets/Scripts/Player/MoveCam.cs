using UnityEditor.Rendering;
using UnityEngine;

//on camera holder, switch between FPCam and top down, 
public class MoveCam : MonoBehaviour  
{
    public Transform cameraPosition;

    public Camera MainCam;
    public Camera TopDownCam;

    private void Start()
    {
        MainCam.gameObject.SetActive(true);
        TopDownCam.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            MainCam.gameObject.SetActive(!MainCam.gameObject.activeSelf);
            TopDownCam.gameObject.SetActive(!TopDownCam.gameObject.activeSelf);
           // Debug.Log(Camera.current + " is active ");
        }

        if (MainCam.isActiveAndEnabled)  
        {
            MainCam.transform.position = cameraPosition.position; // TopDownCam changes position with this, need it to be just one position
        } 
    }
}
