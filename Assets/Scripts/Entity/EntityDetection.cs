using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using static EntityVision;
using UnityEngine.AI;

public class EntityDetection : MonoBehaviour
{
    [Header("PlayerRefs")]
    public GameObject player; // {  get; set; }
    public Transform playerTransform; // {  get; set; }
    public PlayerController _playerController;
    public Vector3 LastKnownPlayerPosition { get; private set; }
    [SerializeField] private CinemachineCamera jumpscareCam;
    private float _timeSinceLostPlayer;
    [SerializeField] private float losePlayerTime = 4f;
    public Transform jumpscarePosition;

    public EntityVision vision;
    public ELightDetection lightDetection;
    public EntityInteract interact;
    public EAnimStates animStates;
    private bool detectionSoundPlaying;
    [SerializeField] UIManager uiManager;
    private NavMeshAgent navMeshAgent;
    public LayerMask obstructionMask;



    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<PlayerController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = player.transform; // need to assign this in insepctor ? 
        vision = GetComponent<EntityVision>();
        lightDetection = GetComponent<ELightDetection>();
        playerTransform = player.transform;
        StartCoroutine(Routine());
    }

    public bool isCaught() //------------------------------------------DETECTION checks if player is close enough to be caught
    {
        float catchDistance = 1.5f;

        return Vector3.Distance(transform.position, playerTransform.position) <= catchDistance;
    }

    public void playerCaught() //-------------------------------------DETECTION handles what to do if player caught
    {
        if (isCaught()) // do something here usuing a raycast to make the ai move to somewhere where the jumpscare cam is not near a wall ? or change location of entity 
        {
            //  gameObject.transform.position = jumpscarePosition.transform.position;

            //   collider.gameObject.SetActive(true);
         //   Debug.Log("PlayerCaughtMethod");
            _playerController.isCaught = true;
            _playerController.Jumpscare(jumpscareCam);
          //  navMeshAgent.isStopped = true;  
            uiManager.WidgetsOff();
           // animStates.SetState(EntityStates.jumpscare);
        }
    }

    public bool HasClearPathToPlayer() // --------------------- VISION checks obstructions between entity and player
    {

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction =
            (playerTransform.position - origin).normalized;

        float distance =
            Vector3.Distance(origin, playerTransform.position);

        return !Physics.Raycast(
            origin,
            direction,
            distance,
            obstructionMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public bool IsPlayerinCones() // --------------------------DETECTION checks if player is hiding, if not checks if player is in any cones 
    {
        if (_playerController.isHiding)
        {
            return false;
        }
        return vision.GetPlayerVisionCones().Count > 0;
        // List<VisionConeType> detectedCones = GetPlayerVisionCones();
        // return detectedCones.Contains(VisionConeType.Normal);
    }


    public bool IsPlayerInsideCone(VisionCone cone) //------------------------DETECTION checks which cone player is inside ?
    {
        Vector3 directionToPlayer =
            playerTransform.position - transform.position;

        float distanceToPlayer = directionToPlayer.magnitude;

        // Range check
        if (distanceToPlayer > cone.detectionRange)
            return false;

        // Convert the cone's local direction into world space
        Vector3 coneDirection =
            transform.TransformDirection(cone.localDirection).normalized;

        float angle =
            Vector3.Angle(
                coneDirection,
                directionToPlayer.normalized
            );

        // Angle check
        return angle <= cone.viewAngle / 2f;
    }

    public bool HasLostPlayer() //----------------------DETECTION not in use currently
    {
        return _timeSinceLostPlayer >= losePlayerTime;
    }


    private IEnumerator Routine() // ----------------------DETECTION acts on which cone player is in, adjusts UI and audio, checks for hiding places and light
    {
        float delay = 0.2f;  // 5x per second instead of every frame 
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
          //  List<VisionConeType> cones = vision.GetPlayerVisionCones();

            if (IsPlayerinCones())
            {
                playerCaught();
                List<VisionConeType> cones = vision.GetPlayerVisionCones();

                    if (cones.Contains(VisionConeType.Normal))
                    {
                   //     Debug.Log("Normal vision");
                    if (isCaught()) detectionSoundPlaying = false;
                        if (detectionSoundPlaying)
                        {
                            detectionSoundPlaying = false;
                          //  AudioManager.Instance.PlayAtPosition("DetectionSound", transform.position);
                        }
                        uiManager.VisibleUI(VisionConeType.Normal); 
                    }
                    if (cones.Contains(VisionConeType.Wide))  // player or light is in wide, make same method (but light is 4secs and player is 3secs)
                    {
                   //     Debug.Log("Wide vision");

                    if (!detectionSoundPlaying && !isCaught())
                        {
                            detectionSoundPlaying = true;
                           // AudioManager.Instance.PlayAtPosition("DetectionSound", transform.position);
                        }
                        uiManager.VisibleUI(VisionConeType.Wide); 
                    }
                    if (cones.Contains(VisionConeType.Close))
                    {
                    //    Debug.Log("Blindspot vision");
                        uiManager.VisibleUI(VisionConeType.Close); 

                    }

                LastKnownPlayerPosition = playerTransform.position;
            //    Debug.Log("PLAYER VISIBLE");
            //    uiManager.VisibleUI(VisionConeType.None); 

                //  _timeSinceLostPlayer = 0f;

                if (isCaught())
                {
                     //   Debug.Log("PLAYER IS CAUGHT MWAHAHA");
                   // uiManager.teleport();

                }
            }
            else if (vision.IsLightInsideCone())
            {
                uiManager.VisibleUI(VisionConeType.WideLight);
            }
            else
            {
                uiManager.VisibleUI(VisionConeType.HidingPlaces);
                detectionSoundPlaying = false;
            }

            lightDetection.CheckLightDetection(delay);
         //   vision.CheckForHidingPlaces();
           // uiManager.VisibleUI(VisionConeType.HidingPlaces); 

        }
    }

}
