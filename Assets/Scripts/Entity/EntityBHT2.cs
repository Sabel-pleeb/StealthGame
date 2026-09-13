using Pathfinding.BehaviourTrees;
using UnityEngine;


public class EntityBHT2 : MonoBehaviour
{
    public Entity context;
    BehaviourTree tree;
    void Awake()
    {
        context = new Entity
        {
            Movement = GetComponent<EntityMovement>(),
            Vision = GetComponent<EntityVision>(),
            Anims = GetComponent<EAnimStates>(),
            Interact = GetComponent<EntityInteract>(),
            Detection = GetComponent<EntityDetection>(),
            lightDetection = GetComponent<ELightDetection>(),
            Generators = FindAnyObjectByType<GeneratorManager>(),
            playerHiding = FindAnyObjectByType<PlayerHiding>(),
            uiManager = FindAnyObjectByType<UIManager>(),
            Transform = transform
        };

        //--------------------------------------------------------------------------------------------------------------------
        tree = new BehaviourTree("Entity2");  // instantiate a new behaviour tree named after this class ?
        PrioritySelector agentLogic = new PrioritySelector("AgentLogic2");


        //---------------------------------------------------------------------------------------------------------------------
        Sequence Chasing = new Sequence("Chasing2", 80);

        Chasing.AddChild(new Leaf("Player Visible2", new Condition(() => context.Vision.GetPlayerVisionCones().Contains(EntityVision.VisionConeType.Normal))));  //CHANGE CONDITION BACK
        Chasing.AddChild(new Leaf("Chase Player2", new ChasePlayerStrategy(transform, context.Movement.agent, context.Detection.playerTransform, context.Anims, context.Detection.IsPlayerinCones, context.Detection.isCaught, EntityStates.running, EntityStates.jumpscare, default, 6)));  // this isnt working 
                                                                                                                                                                                                                                                                                            //  Chasing.AddChild(new Leaf("Player Caught", new Condition(context.Detection.isCaught)));  
        agentLogic.AddChild(Chasing);

        //-----------------------------------------------------------------------------------------------------------------------
        Sequence InvestigateLight = new Sequence("Investigating Light2", 40);

        InvestigateLight.AddChild(new Leaf("Light Visible2", new Condition(context.lightDetection.LightLimitMethod)));
        InvestigateLight.AddChild(new Leaf("Move to Light2", new MoveToTarget(transform, context.Movement.agent, () => context.lightDetection.LastKnownLightPosition)));
        InvestigateLight.AddChild(new Leaf("investigate lightArea2", new InvestigateStrategy(transform, context.Movement.agent, context.Anims, EntityStates.sniffing, 3f, 90f, () => context.lightDetection.InvestigationComplete())));
        agentLogic.AddChild(InvestigateLight);

        //------------------------------------------------------------------------------------------------------------------------
        Sequence InvestigateGenerator = new Sequence("Investigating Gen2", 30);

        InvestigateGenerator.AddChild(new Leaf("Generator Completed2", new Condition(() => context.Generators.JustRepaired())));
        InvestigateGenerator.AddChild(new Leaf("Move to Generator2", new MoveToTarget(transform, context.Movement.agent, () => context.Generators.LastRepairedGenerator.transform)));
        InvestigateGenerator.AddChild(new Leaf("investigate generatorArea2", new InvestigateStrategy(transform, context.Movement.agent, context.Anims, EntityStates.sniffing, 3f, 90f, () => context.Generators.InvestigationComplete())));
        agentLogic.AddChild(InvestigateGenerator);

        //---------------------------------------------------------------------------------------------------------------------------
        Sequence CheckHiding = new Sequence("Hide and Seek2", 35);

        CheckHiding.AddChild(new Leaf("Hiding Nearby2", new Condition(() => context.Vision.CheckForHidingPlaces() && context.playerHiding.hideCount >= 2))); //&& chcek hiding count intCheckHiding.AddChild(
        CheckHiding.AddChild(new Leaf("Move to Hiding2", new MoveToTarget(transform, context.Movement.agent, () => context.Interact.ClosestCabinet)));
        CheckHiding.AddChild(new Leaf("Check Hiding2", new HideAndSeekStrategy(transform, context.Movement.agent, () => context.Interact.ClosestCabinet, context.uiManager, () => context.Vision.CheckForHidingPlaces(), context.Detection.IsPlayerinCones, context.Anims, context.Interact, context.Detection, context.playerHiding, EntityStates.walking, EntityStates.jumpscare, context.Detection.isCaught)));
        agentLogic.AddChild(CheckHiding);

        //----------------------------------------------------------------------------------------------------------------------
        Sequence Patrol = new Sequence("Patrol2", 20);

        Patrol.AddChild(new Leaf("Patrol Movement2", new PatrolStrategy(transform, context.Movement.agent, context.Movement.GetPatrolPoints(), EntityStates.walking, context.Anims, context.Movement, 2.5f)));
        agentLogic.AddChild(Patrol);

        //-----------------------------------------------------------------------------------------------------------------------
        tree.AddChild(agentLogic);

    }


    void Update()
    {
        //  AnimationState.SetSpeed(agent.velocity.magnitude);
        tree.Process();
        // context.Movement.agent.SetDestination(context.Vision.player.position);
    }
}
