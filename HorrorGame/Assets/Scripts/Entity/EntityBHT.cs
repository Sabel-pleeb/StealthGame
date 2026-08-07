using Pathfinding.BehaviourTrees;
using UnityEngine;


public class EntityBHT : MonoBehaviour
{
    public Entity context;
    BehaviourTree tree;
    void Awake()
    {
        context = new Entity
        {
            Movement = GetComponent<EntityMovement>(),
            Vision = GetComponent<EntityVision>(),
          //  generators = GetComponent<Generators>(),
          //  Animation = GetComponent<EntityAnimation>(),
            Transform = transform
        };

        tree = new BehaviourTree("Entity");  // instantiate a new behaviour tree named after this class ?
        PrioritySelector actions = new PrioritySelector("AgentLogic");

        // Sequence RunToGenerator = new Sequence("RunToGenerator", 50);
        // RunToGenerator.AddChild(new Leaf("IsGeneratorOn", new Condition(context.Movement.IsGeneratorRepaired)));
        // RunToGenerator.AddChild(new Leaf("MoveToGenerator", new MoveToTarget(transform, context.Movement.agent, context.Movement.GetGeneratorLocation())));
        // actions.AddChild(RunToGenerator);  // selector for generator ??

     //   Selector willChase = new Selector("willChase", 100);
      //  willChase.AddChild( new Leaf("Player Visible",new Condition(context.Vision.IsPlayerVisible)));


        Sequence Chasing = new Sequence("Chasing", 100);

        Chasing.AddChild(new Leaf("Player Visible", new Condition(context.Vision.IsPlayerVisible)));  //CHANGE CONDITION BACK
        Chasing.AddChild( new Leaf("Chase Player", new ChasePlayerStrategy(transform, context.Movement.agent, context.Vision.player, context.Vision.IsPlayerVisible, default , 7)));
        actions.AddChild(Chasing);


     /*   Sequence Returning = new Sequence("Returning", 50);
        // ADD HASSEENPLAYER OPTION ?
        Returning.AddChild(new Leaf("Has Seen Player", new Condition(context.Vision.hasSeenPlayer)));
        Returning.AddChild(new Leaf("Player Not Visible", new Condition(context.Vision.HasLostPlayer)));
        Returning.AddChild(new Leaf("Patrol After Chase", new ReturnToClosestPatrolPointStrategy(transform, context.Movement.agent, context.Movement.GetPatrolPoints())));
        actions.AddChild(Returning);  */

      //  actions.AddChild(Chasing);

        Sequence Patrol = new Sequence("Patrol", 20);
        Patrol.AddChild(new Leaf("PatrolMovement", new PatrolStrategy(transform, context.Movement.agent, context.Movement.GetPatrolPoints())));
        //  Patrol.AddChild(new Leaf("WaitAtPatrol", new HasStoppedStrategy(context.Movement.agent)));
        //  tree.AddChild(Patrol);
        actions.AddChild(Patrol);
        /*  Sequence returnToPatrol = new Sequence("Return", 50);
          returnToPatrol.AddChild(new Leaf("Lost Player",new Condition(context.Vision.HasLostPlayer)));
          returnToPatrol.AddChild(new Leaf("Return", new ReturnToClosestPatrolPointStrategy( transform, context.Movement.agent, context.Movement.GetPatrolPoints())));
       //   returnToPatrol.AddChild( new Leaf("Clear Lost", new ActionStrategy(context.Vision.ClearLostPlayer)));
          actions.AddChild(returnToPatrol); */


        tree.AddChild(actions);

    }


    void Update()
    {
        //  AnimationState.SetSpeed(agent.velocity.magnitude);
        tree.Process();
       // context.Movement.agent.SetDestination(context.Vision.player.position);
    }
}
