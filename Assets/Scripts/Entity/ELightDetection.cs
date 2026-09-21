using UnityEngine;
using static EntityVision;

public class ELightDetection : MonoBehaviour
{
    [Header("Light Detection")]
    public GameObject light; // {  get; set; }
    [SerializeField] private float lightDetectionTime = 3f;
    private bool detectionSoundPlaying;
    private float _lightVisibleTimer;
    private bool _lightDetected;
    public bool LightLimit;
    public Transform LastKnownLightPosition { get; private set; }

    public LayerMask obstructionMask;

    public EntityVision vision;

    void Awake()
    {
        LightLimit = false;
        vision = GetComponent<EntityVision>();
        light = GameObject.FindGameObjectWithTag("Light");
    }


    private bool HasClearPathToLight() //-------------------------LIGHT checks if there are any obstructions betweenentity and player light
    {
        if (light == null || !light.activeInHierarchy)
            return false;

        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Vector3 direction =
            (light.transform.position - origin).normalized;

        float distance =
            Vector3.Distance(origin, light.transform.position);

        return !Physics.Raycast(
            origin,
            direction,
            distance,
            obstructionMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public void InvestigationComplete()
    {
        LightLimit = false;
    }

    public void CheckLightDetection(float delay) //-----------------------------------LIGHT if there is light in any cone and a clear path, start counting how long ai sees light, could change light detection time 
    {
        if (light == null)
            return;

        bool lightInCone = vision.IsLightInsideCone();
        bool clearPath = HasClearPathToLight();

        if (lightInCone && clearPath)
        {
            _lightDetected = true;

            _lightVisibleTimer += delay;

            // Keep track of where the light currently is
            LastKnownLightPosition = light.transform;

            if (_lightVisibleTimer >= lightDetectionTime)
            {
                LightLimit = true;
                _lightVisibleTimer = 0f;
            }
        }
        else
        {
            _lightDetected = false;
            _lightVisibleTimer = 0f;
        }
    }

    public bool LightLimitMethod()
    {
        if (LightLimit) return true;
        else return false;
    }
}
