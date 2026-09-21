using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Pathfinding.BehaviourTrees  // file for executing strategies in game

{
    public interface IStrategy
    {
        Nodes.Status Process();  // returns a node status eg if strategy succeeds node will return 'Success'
        void Reset()   // incase the strategy has a state, reset this (strat linked to leaf and leaf means reset BHT after leaf behaviour completed ?)
        {
            // can override if neccassary, noop = no operation
        } 
    }

    public class HasStoppedStrategy : IStrategy
    {
        readonly NavMeshAgent agent;
        readonly float stopDuration;

        float stopStartTime;
        bool timerStarted;

        public HasStoppedStrategy(NavMeshAgent agent, float stopDuration = 2f)
        {
            this.agent = agent;
            this.stopDuration = stopDuration;
        }

        public Nodes.Status Process()
        {
            // Haven't reached the destination yet.
            if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                timerStarted = false;
                return Nodes.Status.Running;
            }

            // Start the timer once  arrived.
            if (!timerStarted)
            {
                stopStartTime = Time.time;
                timerStarted = true;
            }

            // Wait until enough time has passed.
            if (Time.time - stopStartTime >= stopDuration)
            {
                return Nodes.Status.Success;
            }

            return Nodes.Status.Running;
        }

        public void Reset()
        {
            timerStarted = false;
        }
    }

    public class ReturnToClosestPatrolPointStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly List<Transform> patrolPoints;

        Transform targetPoint;

        public ReturnToClosestPatrolPointStrategy(Transform entity,NavMeshAgent agent,List<Transform> patrolPoints)
        {
            this.entity = entity;
            this.agent = agent;
            this.patrolPoints = patrolPoints;
        }

        public Nodes.Status Process()
        {
            if (patrolPoints.Count == 0)
            {
                return Nodes.Status.Failure;
            }

            if (targetPoint == null)
            {
                float closestDistance = Mathf.Infinity;

                foreach (Transform point in patrolPoints)
                {
                    float distance = Vector3.Distance(
                        entity.position,
                        point.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        targetPoint = point;
                    }
                }

                if (targetPoint == null)
                    return Nodes.Status.Failure;

                agent.isStopped = false;
                agent.SetDestination(targetPoint.position);
            }

            if (!agent.pathPending && agent.remainingDistance <= 0.2f)
            {
                return Nodes.Status.Success;
            }
            return Nodes.Status.Running;
        }

        public void Reset()
        {
           targetPoint = null;
        }
    }

    public class ChasePlayerStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Transform player;
        readonly float stoppingDistance;
        readonly float chaseSpeed;
        readonly Func<bool> canSeePlayer;
        EAnimStates animState;
        EntityStates state1;
        EntityStates state2;
        readonly Func<bool> isCaught;


        public ChasePlayerStrategy(Transform entity, NavMeshAgent agent, Transform player, EAnimStates animState, Func<bool> canSeePlayer, Func<bool> isCaught, EntityStates state1, EntityStates state2, float stoppingDistance = 1.5f, float chaseSpeed = 7)
        {
            this.entity = entity;
            this.agent = agent;
            this.player = player;
            this.stoppingDistance = stoppingDistance;
            this.chaseSpeed = chaseSpeed;
            this.canSeePlayer = canSeePlayer;
            this.isCaught = isCaught;
            this.state1 = state1;
            this.state2 = state2;
            this.animState = animState;

        }


        public Nodes.Status Process()
        {
            
            if (!canSeePlayer())
            {
                return Nodes.Status.Failure;
            }

            agent.SetDestination(player.position);
            agent.speed = chaseSpeed;

            if (isCaught())// (!agent.pathPending &&
             //   agent.remainingDistance <= 0.3f)  // change to isCaught
            {
                agent.isStopped = true;
                agent.speed = 0;
                animState.SetState(state2);
                return Nodes.Status.Success;
            }
            animState.SetState(state1);
            return Nodes.Status.Running;
        }

        public void Reset()
        {

        }


    }

    public class ActionStrategy : IStrategy  //simple fire and forget 
    {
        readonly Action doSomething;

        public ActionStrategy(Action doSomething)
        {
            this.doSomething = doSomething;
        }

        public Nodes.Status Process()
        {
            doSomething();
            return Nodes.Status.Success;
        }
    }

    public class Condition : IStrategy  //Logical IF
    {
        readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)  //evaluate if Func is true or false - whether met condition or not
        {
            this.predicate = predicate;
        }

        public Nodes.Status Process() => predicate() ? Nodes.Status.Success : Nodes.Status.Failure;
    }

    public class PatrolStrategy : IStrategy
    {
        readonly Transform entity;  
        readonly NavMeshAgent agent;
        readonly List<Transform> patrolPoints;  //waypoints manually set ?
        readonly float patrolSpeed;
        int currentIndex;  //note of patrol point index?
        bool isPathCalculated;
        EAnimStates animState;
        EntityStates state;
        EntityMovement movement;
       // public EntityStates State => state;


        public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints, EntityStates state, EAnimStates animState, EntityMovement movement, float patrolSpeed = 5f)  //constructor
        {
            this.entity = entity;
            this.agent = agent;
            this.patrolPoints = patrolPoints;
            this.patrolSpeed = patrolSpeed;
            this.state = state;
            this.animState = animState;
            this.movement = movement;

        }

        public Nodes.Status Process()
        {
            
            if (patrolPoints.Count == 0) return Nodes.Status.Failure;

            Transform target = patrolPoints[currentIndex];
            agent.speed = patrolSpeed;

            if (!isPathCalculated)
            {
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[currentIndex].position);

                isPathCalculated = true;
            }

             if(!agent.pathPending && agent.remainingDistance < 0.2f)  // distance check to next point, if close enough go to next patrol point index
            {
                currentIndex++;
                if (currentIndex >= patrolPoints.Count)
                {
                    currentIndex = 0;
                    movement.ShufflePatrolPoints();
                }
                isPathCalculated = false;  //reset path bool

            }
            animState.SetState(state);
            return Nodes.Status.Running;  //agent is still moving so process is running
        }

        public void Reset()
        {
            isPathCalculated = false;
            agent.ResetPath();
        }
    }

    public class MoveToTarget : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Func<Transform> getTarget;

        public MoveToTarget(Transform entity, NavMeshAgent agent, Func<Transform> getTarget)
        {
            this.entity = entity;
            this.agent = agent;
            this.getTarget = getTarget;
        }

        public Nodes.Status Process()
        {
            Transform target = getTarget();

            if (target == null)
            {
                return Nodes.Status.Failure;
            }

            agent.isStopped = false;

            agent.SetDestination(target.position);

            if (agent.pathPending)
                return Nodes.Status.Running;

            if (agent.remainingDistance > 1f)
                return Nodes.Status.Running;

            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            return Nodes.Status.Success;
        }

    } 


    public class HideAndSeekStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Func<Transform> getTarget;
        readonly UIManager ui;

        readonly Func<bool> IsHidingPlaceInCone;
        readonly Func<bool> canSeePlayer;

        readonly EAnimStates animState;
        readonly EntityInteract interact;
        readonly EntityDetection detection;
        readonly PlayerHiding playerHiding;

        readonly EntityStates state1;
        readonly EntityStates state2;

        readonly Func<bool> isCaught;

        private bool reachedCabinet;
        private bool openedCabinet;

        private float rotationSpeed = 180f;

        public HideAndSeekStrategy(Transform entity,NavMeshAgent agent,Func<Transform> getTarget, UIManager ui, Func<bool> isHidingPlaceInCone, Func<bool> canSeePlayer,
            EAnimStates animState,EntityInteract interact, EntityDetection detection, PlayerHiding playerHiding, EntityStates state1,EntityStates state2,Func<bool> isCaught)
        {
            this.entity = entity;
            this.agent = agent;
            this.getTarget = getTarget;
            this.ui = ui;
            this.IsHidingPlaceInCone = isHidingPlaceInCone;
            this.canSeePlayer = canSeePlayer;
            this.animState = animState;
            this.interact = interact;
            this.detection = detection;
            this.playerHiding = playerHiding;
            this.state1 = state1;
            this.state2 = state2;
            this.isCaught = isCaught;
        }

        public Nodes.Status Process()
        {
            Transform target = getTarget();
            if (target == null)
                return Nodes.Status.Failure;


            if (!reachedCabinet)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);

                if (agent.pathPending)
                    return Nodes.Status.Running;

                if (agent.remainingDistance > 1.8f)
                {
                    animState.SetState(state1);

                    return Nodes.Status.Running;
                }

                //  reached the cabinet
                reachedCabinet = true;

                agent.isStopped = true;
                agent.velocity = Vector3.zero;

            }


            Vector3 direction = target.position - entity.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                entity.rotation = Quaternion.RotateTowards(
                    entity.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                float angle = Quaternion.Angle(
                    entity.rotation,
                    targetRotation
                );

                if (angle > 2f)
                {
                    return Nodes.Status.Running;
                }
            } 

            if (!openedCabinet)
            {

                if (interact.CheckForHiding())
                {
                    openedCabinet = true;
                    interact.Opening();
                    playerHiding.hideCount = 0;
                }
                else
                {

                      interact.ClearCabinetTarget();
                    openedCabinet = true;
                    agent.isStopped = false;
                    reachedCabinet = false;
                    openedCabinet = false;
                    agent.velocity = Vector3.zero;
                    playerHiding.hideCount = 0;

                    return Nodes.Status.Success;
                }
            }
            if (isCaught())
            {
                agent.speed = 0;
                animState.SetState(state2);
                detection.playerCaught();
                animState.SetState(state2);

                return Nodes.Status.Running;
            }

             if (openedCabinet)
            {
                agent.isStopped = false;
                reachedCabinet = false;
                openedCabinet = false;
                agent.velocity = Vector3.zero;

                return Nodes.Status.Success;
            }

            return Nodes.Status.Running;
        }
    }

    public class InvestigateStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;

        readonly float investigateTime;
        readonly float rotationSpeed;

        float timer;
        float targetRotation;

        EAnimStates animState;
        readonly System.Action onInvestigationComplete;
        EntityStates state;

        bool started;


        public InvestigateStrategy(Transform entity, NavMeshAgent agent, EAnimStates animStates, EntityStates state, float investigateTime = 3f,  float rotationSpeed = 90f, System.Action onInvestigationComplete = null)
        {
            this.entity = entity;
            this.agent = agent;
            this.animState = animStates;
            this.state = state;
            this.onInvestigationComplete = onInvestigationComplete;
            this.investigateTime = investigateTime;
            this.rotationSpeed = rotationSpeed;
        }


        public Nodes.Status Process()
        {
            // Start investigating
            if (!started)
            {
                started = true;
                timer = 0f;
                animState.SetState(state);

                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                targetRotation = entity.eulerAngles.y + 90f;

            }


            timer += Time.deltaTime;


            // Rotate toward target
            float newY = Mathf.MoveTowardsAngle(
                entity.eulerAngles.y,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            entity.rotation = Quaternion.Euler(
                0f,
                newY,
                0f
            );

            // Finished this look
            if (timer >= investigateTime)
            {
                agent.isStopped = false;

                started = false;
                onInvestigationComplete?.Invoke(); 
                return Nodes.Status.Success;
            }


            return Nodes.Status.Running;
        }
    }


}
