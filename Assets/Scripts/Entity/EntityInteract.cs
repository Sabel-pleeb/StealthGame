using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static EntityVision;

[RequireComponent(typeof(NavMeshAgent))]
public class EntityInteract : MonoBehaviour
{
    [SerializeField] private float doorCheckDistance = 1.6f;
    [SerializeField] private LayerMask doorMask;
    public GameObject[] doors;
    public TestInteraction currentDoor;

    public GameObject[] Cabinets;


    [SerializeField] private float cabCheckDistance = 6f;
    [SerializeField] private LayerMask cabMask;
    public GameObject[] cabinets;
    public CabinetManager currentCabinet;
    public Transform ClosestCabinet {  get; private set; }

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        Cabinets = GameObject.FindGameObjectsWithTag("Cabinet");
    }

    private void Start()
    {
        StartCoroutine(Routine());
    }

    private void CheckForDoor() //-------------- checks if raycast hits door then opens 
    {
        Vector3 origin = transform.position + Vector3.up * 1f;

        if (Physics.Raycast(
            origin,
            transform.forward,
            out RaycastHit hit,
            doorCheckDistance,
            doorMask))  //, QueryTriggerInteraction.Ignore))
        {
            TestInteraction door = hit.collider.GetComponentInParent<TestInteraction>();  // was in parent

            if (door != null)
            {

                if (!door._isOpen)
                {
                    currentDoor = door;
                    door.open(transform);
                }
            }
        }
    }

    public bool CheckForHiding()  // -------checks if raycast hits cabinet then opens cabinet door
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(
            origin,
            transform.forward,
            out RaycastHit hit,
            cabCheckDistance,
            cabMask))  //, QueryTriggerInteraction.Ignore))
        {
            CabinetManager cab = hit.collider.GetComponentInParent<CabinetManager>();  

            if (cab != null)
            {
                if (!cab._isOpen)
                {

                    currentCabinet = cab;
                }
                return true;
            }
        }
        return false;
    } 


    public void Opening()
    {
        StartCoroutine(EntityToggleCabinet());

    }

    public bool IsHidingPlaceInsideCone(VisionCone cone) // -----------------------------INTERACTION checks if there is a cabinet inside the entity hidingplaces cone 
    {
        ClosestCabinet = null;

        float closestDistance = Mathf.Infinity;

        foreach (GameObject cabinet in Cabinets)
        {
            Vector3 directionToCabinet =
            cabinet.transform.position - transform.position;

            float distanceToCabinet = directionToCabinet.magnitude;

            // Range check
            if (distanceToCabinet > cone.detectionRange)
                continue;

            Vector3 coneDirection =
                transform.TransformDirection(cone.localDirection).normalized;

            float angle =
                Vector3.Angle(
                    coneDirection,
                    directionToCabinet.normalized
                );

            // Angle check
            if (angle > cone.viewAngle / 2f)
            {
                 continue;
            }

            // This cabinet is inside the cone, is it closer than the previous one?
            if (distanceToCabinet < closestDistance)
            {
                closestDistance = distanceToCabinet;
                ClosestCabinet = cabinet.transform;
            }
        }
        return ClosestCabinet !=null; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position;

        Gizmos.DrawRay(origin, transform.forward * doorCheckDistance);
    }

    private IEnumerator Routine() // Checks for door 5x a second 
    {
        float delay = 0.2f;
        while (true)
        {
            yield return new WaitForSeconds(delay);
            CheckForDoor();
        }
    }

    public void ClearCabinetTarget()
    {
        ClosestCabinet = null;
        currentCabinet = null;
    }

    private IEnumerator EntityToggleCabinet() 
    {
        currentCabinet.isAI = true;
        currentCabinet.OpenDoorsAI(); // then close 
        yield return new WaitForSeconds(1f);
        currentCabinet.CloseDoors();
        currentCabinet.isAI = false;
    }
}