using UnityEngine;
using System.Collections.Generic;
using Pathfinding.BehaviourTrees;
using UnityEngine.AI;


//can create action for opening lockers and use 'safe/danger zone'. +animations 
//seperate script per enemy but all using BHTrees ? 
//instead of waypoints for patrol, try and use pathfinding AI ?

[RequireComponent(typeof(NavMeshAgent))]
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

    [Header("Player")]
    [SerializeField] Transform playerPos;
    [SerializeField] bool playerSpotted;
    //  public PlayerController playerController; 

    public NavMeshAgent agent {  get; private set; }

    private void Awake()
    {
        generator = GetComponent<Generator>();
        agent = GetComponent<NavMeshAgent>();
        //animations = GetComponent<AnimationController>();
    }
    void Start()
    {

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
            Debug.Log("NOP");
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
