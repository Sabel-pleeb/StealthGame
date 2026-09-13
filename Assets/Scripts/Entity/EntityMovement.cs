using UnityEngine;
using System.Collections.Generic;
using Pathfinding.BehaviourTrees;
using UnityEngine.AI;


//can create action for opening lockers and use 'safe/danger zone'. +animations 
//seperate script per enemy but all using BHTrees ? 
//instead of waypoints for patrol, try and use pathfinding AI ?

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(EntityAnimationController))]  // not needed rn 
public class EntityMovement : MonoBehaviour
{
    [Header("Patrolling")]
    [SerializeField] List<Transform> waypoints; //= new();

    [Header("Generators")]
//    [SerializeField] List<Transform> generator1Waypoints = new();
    //   [SerializeField] GameObject generator1;
//    [SerializeField] List<Transform> generator2Waypoints = new();
    //   [SerializeField] GameObject generator2;
    public Generator generator;
    [SerializeField] GeneratorManager generatorManager;
    [SerializeField] List<Generator> generators;

   /* [Header("Player")]
    [SerializeField] Transform playerPos;*/
    [SerializeField] bool playerSpotted; 
    
    //  public PlayerController playerController; 

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
       /*  if (!generator.repaired)
         {
              return false;
         }
       //  GetGeneratorLocation();
         return true; */
       // return GetCurrentGenerator() != null;
       return generatorManager.LastRepairedGenerator != null;   
    }

  /*  public Generator GetCurrentGenerator()
    {
        foreach (Generator generator in generators)
        {
            if (generator.repaired)
                return generator;
        }

        return null;
    } */

    public Transform GetGeneratorLocation()
    {
        Generator target = generatorManager.LastRepairedGenerator;

        if (target == null)
        {
           // Debug.Log("NOP");
            return null;
        }

        return target.transform;
   //  return generatorManager.LastRepairedGenerator.transform;
    }

    public List<Transform> GetPatrolPoints()
    {
        return waypoints;
    }

}
