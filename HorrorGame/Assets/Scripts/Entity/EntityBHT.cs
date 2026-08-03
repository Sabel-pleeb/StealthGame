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

        Sequence Patrol = new Sequence("Patrol");
        Patrol.AddChild(new Leaf("PatrolMovement", new PatrolStrategy(transform, context.Movement.agent, context.Movement.GetPatrolPoints())));
        Patrol.AddChild(new Leaf("WaitAtPatrol", new HasStoppedStrategy(context.Movement.agent)));
        actions.AddChild( Patrol );


        // Sequence RunToGenerator = new Sequence("RunToGenerator", 50);
        // RunToGenerator.AddChild(new Leaf("IsGeneratorOn", new Condition(context.Movement.IsGeneratorRepaired)));
        // RunToGenerator.AddChild(new Leaf("MoveToGenerator", new MoveToTarget(transform, context.Movement.agent, context.Movement.GetGeneratorLocation())));
        // actions.AddChild(RunToGenerator);  // selector for generator ??


        Sequence chase = new Sequence("Chase", 100);
        chase.AddChild( new Leaf("Player Visible",new Condition(context.Vision.IsPlayerVisible)));
        chase.AddChild( new Leaf("Chase Player", new ChasePlayerStrategy(transform, context.Movement.agent, context.Vision.player)));
        actions.AddChild(chase);


        Sequence returnToPatrol = new Sequence("Return", 50);
        returnToPatrol.AddChild(new Leaf("Lost Player",new Condition(context.Vision.HasLostPlayer)));
        returnToPatrol.AddChild(new Leaf("Return", new ReturnToClosestPatrolPointStrategy( transform, context.Movement.agent, context.Movement.GetPatrolPoints())));
     //   returnToPatrol.AddChild( new Leaf("Clear Lost", new ActionStrategy(context.Vision.ClearLostPlayer)));
        actions.AddChild(returnToPatrol);


        tree.AddChild(actions);

    }


    void Update()
    {
        //  AnimationState.SetSpeed(agent.velocity.magnitude);
        tree.Process();
    }
}
