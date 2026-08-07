using UnityEngine;

public class EntityVision : MonoBehaviour
{
  /*     public enum AwarenessState // seperate file ?
       {
           Patrol,
           Chasing,
           Returning
       }  */

    [Header("Refs")]
    [SerializeField] public Transform player; // {  get; set; }

    [Header("Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float losePlayerTime = 3f;

    private float _timeSinceLostPlayer;
   // public bool hasSeenPlayer = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        
    }


    void Update()
    {
        //  var distanceToPlayer = Vector3.Distance(player.position, transform.position);
        hasSeenPlayer();
        if (IsPlayerVisible())
        {
            Debug.Log("PLAYER VISIBLE");
            _timeSinceLostPlayer = 0f;
        }
        else
        {
            _timeSinceLostPlayer += Time.deltaTime;
            HasLostPlayer();
          //  Debug.Log("PLAYER NO LONGER VISIBLE, time since lost: " + _timeSinceLostPlayer);
        }
    }


    public bool IsPlayerVisible()
    {
        return IsFacingPlayer() && HasClearPathToPlayer() && IsWithinRange();
    }

    private bool IsFacingPlayer()
    {
        var dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);
        Debug.Log("IsFacingPlayer");
        return angle <= viewAngle / 2f;
    }

    private bool HasClearPathToPlayer()
    {
        var dirToPlayer = player.position - transform.position;
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude, Physics.DefaultRaycastLayers,
    QueryTriggerInteraction.Ignore))
        {
            return hit.transform == player;
        }
        Debug.Log("HasClearPathToPlayer");
        return true;
    }

    public bool HasLostPlayer()
    {
          if (IsPlayerVisible()) return false;
          Debug.Log("Has Lost Player");
          return true; 
       // return _timeSinceLostPlayer >= losePlayerTime;
    }

    public bool hasSeenPlayer()
    {
        if (IsPlayerVisible())
        {
            Debug.Log("HAS SEEN PLAYER");
            return true;
        } return false;
    }

    private bool IsWithinRange()
    {
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }


}
