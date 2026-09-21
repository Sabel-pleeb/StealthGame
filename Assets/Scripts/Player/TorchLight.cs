using UnityEngine;
using UnityEngine.InputSystem; 

public class TorchController : MonoBehaviour
{
    [Header("Torch Settings")]
    public Light torchLight;           
    public Key toggleKey = Key.F;      

    private bool isOn = true;

    void Start()
    {
        
        if (torchLight != null)
            torchLight.enabled = isOn;
        
    }

    void Update()
    {
 
        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            ToggleTorch();
        }
    }

    private void ToggleTorch()
    {
        isOn = !isOn;

        if (torchLight != null)
            torchLight.enabled = isOn;

    }
}
