using UnityEngine;
using System.Collections.Generic;
using Pathfinding.BehaviourTrees;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EntityMovement : MonoBehaviour
{
    [Header("Patrolling")]
    [SerializeField] List<Transform> waypoints; 

    [Header("Generators")]
    public Generator generator;
    [SerializeField] GeneratorManager generatorManager;
    [SerializeField] List<Generator> generators;

    [SerializeField] bool playerSpotted; 
    

    public NavMeshAgent agent {  get; private set; }
    public Animator animator { get; private set; }

    private void Awake()
    {
        generator = GetComponent<Generator>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {

    }

    public void ShufflePatrolPoints()
    {
        for (int i = waypoints.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            Transform temp = waypoints[i];
            waypoints[i] = waypoints[randomIndex];
            waypoints[randomIndex] = temp;
        }
    }

    void Update()
    {
    }


    public bool isSpotted()
    {
        if (!playerSpotted)

            return false;


        return true;
    }

    public bool IsGeneratorRepaired()
    {
       return generatorManager.LastRepairedGenerator != null;   
    }


    public Transform GetGeneratorLocation()
    {
        Generator target = generatorManager.LastRepairedGenerator;

        if (target == null)
        {
            return null;
        }

        return target.transform;
    }

    public List<Transform> GetPatrolPoints()
    {
        return waypoints;
    }

}
