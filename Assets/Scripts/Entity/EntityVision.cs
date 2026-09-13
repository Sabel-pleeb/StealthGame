using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class EntityVision : MonoBehaviour
{
       public enum VisionConeType // all vision cone types 
       {
           Normal,
           Wide,
           WideLight,
           Close,
           HidingPlaces
         //  None
       }  

    [Header("PlayerRefs")]
    public GameObject player; // {  get; set; }
  //  public Transform playerTransform; // {  get; set; }
    public PlayerController _playerController;

    public EntityDetection entityDetection;
    public ELightDetection lightDetection;
    public EntityInteract entityInteract;
    public Vector3 LastKnownPlayerPosition { get; private set; }

    [Header("Settings")]
    [SerializeField] private float detectionRange = 15f;  //radius
    [SerializeField] private float viewAngle = 90f;

    
    [SerializeField] private VisionCone[] visionCones;


    private void Awake()
    {
        entityDetection = GetComponent<EntityDetection>();
        player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<PlayerController>();

    }



    [System.Serializable] //----------------------------------------------------------- VISION cone types to be set/assigned in inspector
    public class VisionCone  
    {
        public VisionConeType type;

        public float detectionRange = 15f;
        public float viewAngle = 90f;

        // Direction relative to the entity
        public Vector3 localDirection = Vector3.forward;
    }


    public List<VisionConeType> GetPlayerVisionCones() //---------------------------------------- VISION called by bool in player detection, lists the cones player is currently inside
    {
        List<VisionConeType> conesPlayerIsInside = new List<VisionConeType>();

        if (_playerController.isHiding)
            return conesPlayerIsInside;

        foreach (VisionCone cone in visionCones)
        {
            if (entityDetection.IsPlayerInsideCone(cone) && entityDetection.HasClearPathToPlayer())
            {
                conesPlayerIsInside.Add(cone.type);

            } 
        }

        return conesPlayerIsInside;
    }

    public bool CheckForHidingPlaces() // askes EntityInteract to check if there is a cabinet inside the hidingplaces cone type, then asks it to open the cabdoor with raycast
    {
        foreach (VisionCone cone in visionCones)
        {
            if (cone.type != VisionConeType.HidingPlaces)
                continue;
            if (entityInteract.IsHidingPlaceInsideCone(cone))
            {
             //   entityInteract.CheckForHiding();
              //  Debug.Log($"Hiding place detected in {cone.type} cone");  // this works
                return true;
            }
           // return false;
        }
        return false;
    }

    public bool IsLightInsideCone() //---------------------------LIGHT checks if light object attatched to playerCamFP is inside any cone, should i change this to just wide ?
    {
        if (lightDetection.light == null)
            return false;

        Vector3 directionToLight =
            lightDetection.light.transform.position - transform.position;

        float distanceToLight = directionToLight.magnitude;

        foreach (VisionCone cone in visionCones)
        {
            if (cone.type != VisionConeType.Wide) // || 
                continue;
            // Range check
            if (distanceToLight > cone.detectionRange)
                continue;

            // Convert cone's local direction into world space
            Vector3 coneDirection =
                transform.TransformDirection(cone.localDirection).normalized;

            float angle =
                Vector3.Angle(
                    coneDirection,
                    directionToLight.normalized
                );

            if (angle <= cone.viewAngle / 2f)
            {
                return true;
            }
        }

        return false;
    }


    private void OnDrawGizmosSelected() //------------------------------VISION EDITOR
    {
        if (visionCones == null)
            return;

        foreach (VisionCone cone in visionCones)
        {

            Gizmos.color = Color.yellow;

            Vector3 origin = transform.position;
            float halfAngle = cone.viewAngle / 2f;

            // Centre line
            Gizmos.DrawRay(origin, transform.forward * cone.detectionRange);

            // Left and right edges
            Vector3 leftDirection =
                Quaternion.Euler(0f, -halfAngle, 0f) * transform.forward;

            Vector3 rightDirection =
                Quaternion.Euler(0f, halfAngle, 0f) * transform.forward;

            Gizmos.DrawRay(origin, leftDirection * cone.detectionRange);
            Gizmos.DrawRay(origin, rightDirection * cone.detectionRange);

            // Draw the arc
            int segments = 30;

            Vector3 previousPoint =
                origin + leftDirection * cone.detectionRange;

            for (int i = 1; i <= segments; i++)
            {
                float angle = Mathf.Lerp(-halfAngle, halfAngle, i / (float)segments);

                Vector3 direction =
                    Quaternion.Euler(0f, angle, 0f) * transform.forward;

                Vector3 nextPoint =
                    origin + direction * cone.detectionRange;

                Gizmos.DrawLine(previousPoint, nextPoint);

                previousPoint = nextPoint;
            }
        }
    }

}
