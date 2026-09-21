using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerCamFP : MonoBehaviour
{
    public float senseX;
    public float senseY;

    public Transform orientation;

    private float xRotation; 
    private float yRotation;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senseX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senseY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // stop 360 when looking up and down

        //rotate camera and orientation 
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        Debug.DrawRay(
    transform.position,
    transform.forward * 10f,
    Color.red
);

    }

    public void TurnAround()
    {
        StartCoroutine(CameraTurn((yRotation += 180), 3f));
    }

    private IEnumerator CameraTurn(float targetYRotation, float duration)
    {
      //  _isTurning = true;

        float startYRotation = yRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            yRotation = Mathf.Lerp(
                startYRotation,
                targetYRotation,
                elapsed / duration
            );

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }

        yRotation = targetYRotation;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

      //  _isTurning = false;
    }
}
